using ImageTransformer.Console.Interfaces;
using Microsoft.Extensions.Configuration;
using NetVips;
using System.IO;
using System.Threading.Tasks;

namespace ImageTransformer.Console.Services;

/// <summary>
/// Service for processing PNG images, converting to JPEG based on size threshold.
/// Uses NetVips library for efficient image handling.
/// </summary>
public class ImageProcessor : IImageProcessor
{
    private readonly IConfiguration _configuration;

    /// <summary>
    /// Initializes a new instance of the <see cref="ImageProcessor"/> class.
    /// </summary>
    /// <param name="configuration">The application configuration for accessing size thresholds.</param>
    public ImageProcessor(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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
        if (string.IsNullOrWhiteSpace(inputFilePath))
        {
            throw new ArgumentException("Input file path cannot be null or empty.", nameof(inputFilePath));
        }

        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new ArgumentException("Output directory cannot be null or empty.", nameof(outputDirectory));
        }

        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);

        // Get file size in MB
        var fileInfo = new FileInfo(inputFilePath);
        double sizeMB = fileInfo.Length / (1024.0 * 1024.0);

        // Get threshold from configuration, with default fallback
        if (!double.TryParse(_configuration["PngSizeThresholdMB"], out double thresholdMB))
        {
            thresholdMB = 5.0; // Default threshold
        }

        string outputFilePath;
        if (sizeMB > thresholdMB)
        {
            // Convert to JPEG using NetVips for efficient processing
            using var image = Image.NewFromFile(inputFilePath);
            outputFilePath = Path.Combine(outputDirectory, Path.GetFileNameWithoutExtension(inputFilePath) + ".jpg");
            image.Jpegsave(outputFilePath);
        }
        else
        {
            // Copy as-is to preserve original format
            string fileName = Path.GetFileName(inputFilePath);
            outputFilePath = Path.Combine(outputDirectory, fileName);
            File.Copy(inputFilePath, outputFilePath, true);
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
        // TODO: Implement JPEG resizing logic
        await Task.CompletedTask;
    }
}