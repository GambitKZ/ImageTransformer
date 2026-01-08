using Microsoft.Extensions.Configuration;
using Spectre.Console;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>()
    .Build();

// To manage UserSecrets:
// - Set a secret: dotnet user-secrets set "Key" "Value"
// - List secrets: dotnet user-secrets list
// - Clear secrets: dotnet user-secrets clear

// Example usage: var inputFolder = configuration["InputFolder"];
Console.WriteLine("Configuration loaded successfully.");

// Load list of image files from input folder
var inputFolderPath = Path.Combine(Directory.GetCurrentDirectory(), configuration["InputFolder"] ?? "Input");
if (!Directory.Exists(inputFolderPath))
{
    throw new DirectoryNotFoundException($"Input folder '{inputFolderPath}' does not exist.");
}
var imageFiles = Directory.GetFiles(inputFolderPath)
    .Where(f => Path.GetExtension(f).ToLowerInvariant() is ".png" or ".jpg" or ".jpeg")
    .ToList();

// Wrap processing loop with progress bar
AnsiConsole.Progress()
    .Start(ctx =>
    {
        var task = ctx.AddTask("Processing images", maxValue: imageFiles.Count);
        foreach (var file in imageFiles)
        {
            AnsiConsole.MarkupLine($"[blue]Processing {Markup.Escape(Path.GetFileName(file))}...[/]");
            // TODO: Implement image processing logic here
            task.Increment(1);
        }
    });
