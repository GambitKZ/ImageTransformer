using ImageTransformer.Console.Interfaces;

/// <summary>
/// Implementation of IImageScanner that discovers image files in a specified folder.
/// </summary>
public class ImageScanner : IImageScanner
{
    /// <summary>
    /// Asynchronously scans the specified input folder for image files.
    /// </summary>
    /// <param name="inputPath">The full path to the input folder to scan.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection of full file paths for discovered image files.</returns>
    /// <exception cref="ArgumentException">Thrown when inputPath is null or empty.</exception>
    /// <exception cref="DirectoryNotFoundException">Thrown when the input folder does not exist.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the input folder is denied.</exception>
    public async Task<IEnumerable<string>> ScanAsync(string inputPath)
    {
        if (string.IsNullOrWhiteSpace(inputPath))
        {
            throw new ArgumentException("Input path cannot be null or empty.", nameof(inputPath));
        }

        try
        {
            // Use Directory.EnumerateFiles for efficient enumeration
            var imageFiles = Directory.EnumerateFiles(inputPath)
                .Where(file => Path.GetExtension(file).ToLowerInvariant() is ".png" or ".jpg" or ".jpeg")
                .OrderBy(file => file); // Ensure consistent ordering

            // Convert to list to materialize the results
            return await Task.FromResult(imageFiles.ToList());
        }
        catch (DirectoryNotFoundException)
        {
            throw; // Re-throw directory not found exceptions
        }
        catch (UnauthorizedAccessException)
        {
            throw; // Re-throw access denied exceptions
        }
        catch (Exception ex)
        {
            // For any other unexpected exceptions, wrap in a more descriptive exception
            throw new InvalidOperationException($"Failed to scan input folder '{inputPath}': {ex.Message}", ex);
        }
    }
}