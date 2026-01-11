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

    /// <summary>
    /// Copies the specified file to the target directory without modifying its contents or format.
    /// </summary>
    /// <param name="inputFilePath">The full path to the source file to copy.</param>
    /// <param name="outputDirectory">The directory where the file will be copied. The directory is expected to exist or be creatable by the caller.</param>
    /// <remarks>
    /// This method performs a straightforward file copy and overwrites any existing file with the same name in the destination directory.
    /// It also writes a confirmation message to the console using <see cref="Spectre.Console.AnsiConsole"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">Thrown when <paramref name="inputFilePath"/> or <paramref name="outputDirectory"/> is null, empty, or whitespace.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the output directory path is invalid.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when the caller does not have the required permission.</exception>
    /// <exception cref="IOException">Thrown when an I/O error occurs during the copy operation.</exception>
    /// <example>
    /// <code>
    /// // Copy a file as-is to the 'out' directory
    /// ImageProcessor.CopyAsIs(@"C:\images\photo.png", @"C:\images\out");
    /// </code>
    /// </example>
    void CopyAsIs(string inputFilePath, string outputDirectory);
}
