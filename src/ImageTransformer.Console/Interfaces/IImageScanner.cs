namespace ImageTransformer.Console.Interfaces;

/// <summary>
/// Defines a contract for scanning input folders to discover image files.
/// </summary>
public interface IImageScanner
{
    /// <summary>
    /// Asynchronously scans the specified input folder for image files.
    /// </summary>
    /// <param name="inputPath">The full path to the input folder to scan.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of full file paths for discovered image files.</returns>
    /// <remarks>
    /// Supported image file extensions: .png, .jpg, .jpeg (case-insensitive).
    /// Returns an empty collection if no images are found or if the folder is empty.
    /// </remarks>
    Task<IEnumerable<string>> ScanAsync(string inputPath);
}