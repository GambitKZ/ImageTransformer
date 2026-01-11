using ImageTransformer.Console.Interfaces;
using ImageTransformer.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Spectre.Console;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

// Configure configuration
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddUserSecrets<Program>();

// Register services
builder.Services.AddScoped<IImageScanner, ImageScanner>();
builder.Services.AddScoped<IImageProcessor, ImageProcessor>();

using IHost host = builder.Build();

// Get services
var configuration = host.Services.GetRequiredService<IConfiguration>();
var imageScanner = host.Services.GetRequiredService<IImageScanner>();
var imageProcessor = host.Services.GetRequiredService<IImageProcessor>();

// Display welcome message
AnsiConsole.MarkupLine("[green]ImageTransformer Console Application[/]");
AnsiConsole.MarkupLine("[dim]Configuration loaded successfully.[/]");

// Validate input folder
string inputFolderPath;
if (IsInputFolderInvalid(configuration["InputFolder"], out inputFolderPath))
{
    return;
}

AnsiConsole.WriteLine();

// Scan for images
AnsiConsole.MarkupLine($"[blue]Scanning input folder: {Markup.Escape(inputFolderPath)}[/]");
IEnumerable<string> imageFiles;

try
{
    imageFiles = await imageScanner.ScanAsync(inputFolderPath);
}
catch (Exception ex)
{
    AnsiConsole.MarkupLine($"[red]Error scanning input folder: {Markup.Escape(ex.Message)}[/]");
    return;
}

// Display results
var imageCount = imageFiles.Count();
if (imageCount == 0)
{
    AnsiConsole.MarkupLine("[yellow]No image files found in the input folder.[/]");
    AnsiConsole.MarkupLine("[dim]Supported formats: .png, .jpg, .jpeg[/]");
    return;
}

AnsiConsole.MarkupLine($"[green]Found {imageCount} image file{(imageCount == 1 ? "" : "s")} to process:[/]");

foreach (var file in imageFiles)
{
    AnsiConsole.MarkupLine($"  [dim]• {Markup.Escape(Path.GetFileName(file))}[/]");
}

AnsiConsole.WriteLine();

// Get output folder path from configuration
var outputDirectory = Path.Combine(Directory.GetCurrentDirectory(), configuration["OutputFolder"] ?? "Output");
Directory.CreateDirectory(outputDirectory);

// Wrap processing loop with progress bar
await AnsiConsole.Progress()
    .StartAsync(async ctx =>
    {
        var task = ctx.AddTask("Processing images", maxValue: imageCount);
        foreach (var file in imageFiles)
        {
            AnsiConsole.MarkupLine($"[blue]Processing {Markup.Escape(Path.GetFileName(file))}...[/]");
            try
            {
                switch (Path.GetExtension(file).ToLowerInvariant())
                {
                    case ".png":
                        await imageProcessor.ProcessPngImage(file, outputDirectory);
                        AnsiConsole.MarkupLine($"[green]Converted {Markup.Escape(Path.GetFileName(file))} to JPEG.[/]");
                        break;
                    case ".jpg":
                    case ".jpeg":
                        await imageProcessor.ProcessJpegImage(file, outputDirectory);
                        AnsiConsole.MarkupLine($"[green]Processed JPEG {Markup.Escape(Path.GetFileName(file))}.[/]");
                        break;
                    default:
                        imageProcessor.CopyAsIs(file, outputDirectory);
                        break;
                }
            }
            catch (Exception ex)
            {
                AnsiConsole.MarkupLine($"[red]Error processing {Markup.Escape(Path.GetFileName(file))}: {Markup.Escape(ex.Message)}[/]");
            }
            task.Increment(1);
        }
    });

AnsiConsole.MarkupLine("[green]Processing completed![/]");

static bool IsInputFolderInvalid(string? inputFolder, out string inputFolderPath)
{
    inputFolderPath = Path.Combine(Directory.GetCurrentDirectory(), inputFolder ?? "Input");
    if (!Directory.Exists(inputFolderPath))
    {
        AnsiConsole.MarkupLine($"[red]Input folder not found: {Markup.Escape(inputFolderPath)}[/]");
        AnsiConsole.MarkupLine("[dim]Please ensure the input folder exists and try again.[/]");
        return true;
    }

    // Check if folder is accessible
    try
    {
        // Attempt to enumerate files to verify read access
        _ = Directory.EnumerateFileSystemEntries(inputFolderPath).FirstOrDefault();
    }
    catch (UnauthorizedAccessException)
    {
        AnsiConsole.MarkupLine($"[red]Input folder is not accessible: {Markup.Escape(inputFolderPath)}[/]");
        AnsiConsole.MarkupLine("[dim]Please check folder permissions and try again.[/]");
        return true;
    }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine($"[red]Error accessing input folder: {Markup.Escape(ex.Message)}[/]");
        return true;
    }

    return false;
}
