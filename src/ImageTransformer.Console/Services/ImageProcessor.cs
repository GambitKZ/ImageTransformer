using ImageTransformer.Console.Interfaces;
using Microsoft.Extensions.Configuration;
using NetVips;
using Spectre.Console;

namespace ImageTransformer.Console.Services;

/// <summary>
/// Service for processing PNG images, converting to JPEG based on size threshold.
/// Uses NetVips library for efficient image handling.
/// </summary>
public class ImageProcessor : IImageProcessor
{
    private readonly IConfiguration _configuration;

    private double _pngSizeThresholdMB;
    private double _jpegTargetSizeMB;
    private double _resizeStepPercentage;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageProcessor"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration for accessing size thresholds.</param>
    public ImageProcessor(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        if (!double.TryParse(_configuration["PngSizeThresholdMB"], out _pngSizeThresholdMB))
        {
            _pngSizeThresholdMB = 5.0;
        }

        if (!double.TryParse(_configuration["JpegTargetSizeMB"], out _jpegTargetSizeMB))
        {
            _jpegTargetSizeMB = 5.0;
        }

        if (!double.TryParse(_configuration["ResizeStepPercentage"], out _resizeStepPercentage))
        {
            _resizeStepPercentage = 3.0; // Default resize step
        }
    }

    /// <summary>
    /// Processes a PNG image by checking its size against the configured threshold.
    /// If the size is above the threshold, converts it to JPEG using NetVips; otherwise, copies it as-is.
    /// This design optimizes storage by converting large PNGs to JPEG while preserving small ones.
    /// </summary>
    /// <param name="inputFilePath">The path to the input PNG file.</param>
    /// <param name="outputDirectory">The directory to save the processed file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessPngImage(string inputFilePath, string outputDirectory)
    {
        ValidateFolders(inputFilePath, outputDirectory);

        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);

        var sizeMB = GetFileSizeInMB(inputFilePath);

        if (sizeMB < _pngSizeThresholdMB)
        {
            CopyAsIs(inputFilePath, outputDirectory);
            return;
        }

        // Convert to JPEG using NetVips for efficient processing
        using var image = Image.NewFromFile(inputFilePath);
        string outputFilePath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFilePath) + ".jpg");

        var jpegBytes = SaveJpegToMemory(image);
        var currentSizeMB = jpegBytes.Length / (1024.0 * 1024.0);

        // if size is correct, save and return
        if (currentSizeMB < _jpegTargetSizeMB)
        {
            File.WriteAllBytes(outputFilePath, jpegBytes);
            return;
        }

        // If it is still large, process further as JPEG
        await ProcessJpegImage(inputFilePath, outputDirectory);
    }



    /// <summary>
    /// Processes a JPEG image by checking its size against the configured target.
    /// If the size is above the target, resizes it iteratively by the step percentage until the target is met or maximum iterations are reached.
    /// </summary>
    /// <param name="inputFilePath">The path to the input JPEG file.</param>
    /// <param name="outputDirectory">The directory to save the processed file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task ProcessJpegImage(string inputFilePath, string outputDirectory)
    {
        ValidateFolders(inputFilePath, outputDirectory);

        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);

        var originalSizeMB = GetFileSizeInMB(inputFilePath);

        string outputFilePath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFilePath) + ".jpg");

        if (originalSizeMB <= _jpegTargetSizeMB)
        {
            // Copy as-is if already within target size
            File.Copy(inputFilePath, outputFilePath, true);
            return;
        }

        // Load the image using NetVips
        using var image = Image.NewFromFile(inputFilePath);

        byte[] bestFileBytes = [];
        double bestPercent = 0.0;

        int iteration = 0;
        Image currentImage = image;

        double leftRange = 0;
        double rightRange = 100;

        // Alway takes 6 steps
        while ((rightRange - leftRange) >= _resizeStepPercentage)
        {
            iteration++;

            double percent = (leftRange + rightRange) / 2.0;
            double scaleFactor = 1.0 - (percent / 100.0);

            // Resize the image
            currentImage = image.Resize(scaleFactor);

            var jpegBytes = SaveJpegToMemory(currentImage);
            var currentSizeMB = jpegBytes.Length / (1024.0 * 1024.0);

            var isFit = currentSizeMB <= _jpegTargetSizeMB;

            if (isFit)
            {
                bestPercent = percent;
                bestFileBytes = jpegBytes;

                // Change Right range
                rightRange = percent;
            }
            else
            {
                // Change Left range
                leftRange = percent;
            }
        }

        File.WriteAllBytes(outputFilePath, bestFileBytes);

        AnsiConsole.MarkupLine($"[green]Saved resized image at {Markup.Escape(outputFilePath)} (percent={bestPercent:F2}). Attempt number #{iteration} [/].");
    }

    public void CopyAsIs(string inputFilePath, string outputDirectory)
    {
        ValidateFolders(inputFilePath, outputDirectory);

        var fileName = Path.GetFileName(inputFilePath);
        string destFile = Path.Combine(outputDirectory, fileName);

        File.Copy(inputFilePath, destFile, true);
        AnsiConsole.MarkupLine($"[green]Copied {Markup.Escape(fileName)} as-is.[/]");
    }

    private static void ValidateFolders(string inputFilePath, string outputDirectory)
    {
        if (string.IsNullOrWhiteSpace(inputFilePath))
        {
            throw new ArgumentException("Input file path cannot be null or empty.", nameof(inputFilePath));
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new ArgumentException("Output directory cannot be null or empty.", nameof(outputDirectory));
        }
    }

    private static double GetFileSizeInMB(string inputFilePath)
    {
        var fileInfo = new FileInfo(inputFilePath);
        return fileInfo.Length / (1024.0 * 1024.0);
    }

    private static byte[] SaveJpegToMemory(Image image)
    {
        // Best-effort settings to minimize loss:
        // - Q = 100 -> maximum quantizer quality
        // - subsample_mode = disable chroma subsampling (keeps full chroma resolution)
        // - optimizeCoding = true -> improved Huffman tables
        // - trellisQuant = true, overshootDeringing = true -> improved visual quality
        // - strip = true -> remove metadata (does not affect pixels, just reduces size)
        // - subsampleMode -> disable chroma subsampling to keep color detail:
        return image.JpegsaveBuffer(
            q: 100,
            optimizeCoding: true,
            trellisQuant: true,
            overshootDeringing: true,
            subsampleMode: Enums.ForeignSubsample.Off
        );
    }
}
