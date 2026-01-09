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

        // Get configuration values with defaults
        if (!double.TryParse(_configuration["JpegTargetSizeMB"], out double targetSizeMB))
        {
            targetSizeMB = 5.0; // Default target size
        }

        if (!double.TryParse(_configuration["ResizeStepPercentage"], out double resizeStepPercentage))
        {
            resizeStepPercentage = 3.0; // Default resize step
        }

        if (!int.TryParse(_configuration["MaxResizeIterations"], out int maxIterations))
        {
            maxIterations = 10; // Default max iterations
        }

        string outputFilePath = Path.Combine(outputDirectory, Path.GetFileName(inputFilePath));

        if (sizeMB <= targetSizeMB)
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
        int iteration = 0;
        double currentSizeMB = sizeMB;
        Image currentImage = image;

        while (currentSizeMB > targetSizeMB && iteration < maxIterations)
        {
            // Calculate scale factor (reduce dimensions by resizeStepPercentage)
            double scaleFactor = 1.0 - (resizeStepPercentage / 100.0);

            // Resize the image
            currentImage = currentImage.Resize(scaleFactor);

            // Save temporarily to check size (using a temp file to avoid overwriting output prematurely)
            string tempFilePath = Path.Combine(outputDirectory, Guid.NewGuid().ToString() + ".jpg");
            try
            {
                currentImage.Jpegsave(tempFilePath);

                // Check the new size
                var tempFileInfo = new FileInfo(tempFilePath);
                currentSizeMB = tempFileInfo.Length / (1024.0 * 1024.0);

                // If size is now acceptable or we've reached max iterations, move to final output
                if (currentSizeMB <= targetSizeMB || iteration == maxIterations - 1)
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
}