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
    private int _maxResizeIterations;

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

        if (!int.TryParse(_configuration["MaxResizeIterations"], out _maxResizeIterations))
        {
            _maxResizeIterations = 10; // Default max iterations
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
        SaveJpeg(image, outputFilePath);

        // If it is still large, process further as JPEG
        var currentSizeMB = GetFileSizeInMB(outputFilePath);
        if (currentSizeMB > _jpegTargetSizeMB)
        {
            File.Delete(outputFilePath);
            await ProcessJpegImage(inputFilePath, outputDirectory);
        }
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

        string outputFilePath = Path.Combine(outputDirectory, Path.GetFileName(inputFilePath));

        if (originalSizeMB <= _jpegTargetSizeMB)
        {
            // Copy as-is if already within target size
            File.Copy(inputFilePath, outputFilePath, true);
            return;
        }

        // Load the image using NetVips
        using var image = Image.NewFromFile(inputFilePath);

        // Iterative resizing loop
        // Design choice: Resize iteratively to avoid over-compression in one step, allowing gradual size reduction.
        // This approach balances quality and size, stopping when target is met or max iterations reached.
        int iteration = 1;
        double currentSizeMB = originalSizeMB;
        Image currentImage = image;

        //List<FileState> attempedStates = new();
        //var percent = 50;

        while (currentSizeMB > _jpegTargetSizeMB && iteration < _maxResizeIterations)
        {
            // Calculate scale factor (reduce dimensions by _resizeStepPercentage)
            //double scaleFactor = 1.0 - (_resizeStepPercentage / 100.0);
            var resizePercentage = _resizeStepPercentage * iteration;
            double scaleFactor = 1.0 - (resizePercentage / 100.0);

            //double scaleFactor = 1.0 - (percent / 100.0);

            // Resize the image
            //currentImage = currentImage.Resize(scaleFactor);
            currentImage = image.Resize(scaleFactor);

            // Save temporarily to check size (using a temp file to avoid overwriting output prematurely)
            string tempFilePath = Path.Combine(outputDirectory, Guid.NewGuid().ToString() + ".jpg");
            try
            {
                SaveJpeg(currentImage, tempFilePath);

                // Check the new size
                currentSizeMB = GetFileSizeInMB(tempFilePath);

                // If size is now acceptable or we've reached max iterations, move to final output
                if (currentSizeMB <= _jpegTargetSizeMB || iteration == _maxResizeIterations - 1)
                {
                    File.Move(tempFilePath, outputFilePath, true);
                    break;
                }
                else
                {
                    // Clean up temp file and continue
                    File.Delete(tempFilePath);
                }
            }
            catch (Exception ex)
            {
                // Clean up temp file on error
                if (File.Exists(tempFilePath))
                {
                    File.Delete(tempFilePath);
                }
                throw new InvalidOperationException("Error during JPEG resizing operation.", ex);
            }

            iteration++;
        }

        // If loop exited without saving, copy original (shouldn't happen, but safety net)
        if (!File.Exists(outputFilePath))
        {
            File.Copy(inputFilePath, outputFilePath, true);
        }
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

    private static void SaveJpeg(Image image, string outputFilePath)
    {
        // Best-effort settings to minimize loss:
        // - Q = 100 -> maximum quantizer quality
        // - subsample_mode = disable chroma subsampling (keeps full chroma resolution)
        // - optimizeCoding = true -> improved Huffman tables
        // - trellisQuant = true, overshootDeringing = true -> improved visual quality
        // - strip = true -> remove metadata (does not affect pixels, just reduces size)
        // - subsampleMode -> disable chroma subsampling to keep color detail:
        image.Jpegsave(
                    outputFilePath,
                    q: 100,
                    optimizeCoding: true,
                    trellisQuant: true,
                    overshootDeringing: true,
                    subsampleMode: Enums.ForeignSubsample.Off
                );
    }
}
