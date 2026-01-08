using System.Threading.Tasks;

namespace ImageTransformer.Console.Interfaces;

/// <summary>
/// Interface for processing images, handling PNG conversion and JPEG resizing based on size thresholds.
/// </summary>
public interface IImageProcessor
{
    /// <summary>
    /// Processes a PNG image by checking its size against the configured threshold.
    /// If the size is above the threshold, converts it to JPEG; otherwise, copies it as-is.
    /// </summary>
    /// <param name="inputFilePath">The path to the input PNG file.</param>
    /// <param name="outputDirectory">The directory to save the processed file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessPngImage(string inputFilePath, string outputDirectory);

    /// <summary>
    /// Processes a JPEG image by checking its size against the configured target.
    /// If the size is above the target, resizes it iteratively by the step percentage until the target is met or maximum iterations are reached.
    /// </summary>
    /// <param name="inputFilePath">The path to the input JPEG file.</param>
    /// <param name="outputDirectory">The directory to save the processed file.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task ProcessJpegImage(string inputFilePath, string outputDirectory);
}