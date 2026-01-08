# ImageTransformer

## Primary Goal / Business Domain
The ImageTransformer is a console application designed to process all images from an input folder and transform them into an output folder. PNG images are converted to JPEG format, except for files below a configurable size threshold, which are copied as-is. All JPEG images (including those converted from PNG) undergo additional size checks: if the file size exceeds 5 MB, it is resized by reducing its dimensions by a configurable step percentage (e.g., 3%). If the resized file still exceeds the configured target size, the process repeats using the original image, reducing by an additional step percentage until the target size is met.

## Technology Stack
* **Primary Technology**: .NET 10
* **Backend**: Console Application
* **Database**: None
* **Frontend (if applicable)**: No dedicated frontend - Console only
* **Cloud Platform (if applicable)**: On-premise
* **Other Key Technologies**: appsettings.json for configuration, UserSecrets for sensitive settings, Spectre.Console for console interactions, Image processing libraries such as NetVips or ImageMagick.NET (Magic.Net)

## Executive Summary
ImageTransformer is a lightweight console application built with .NET 10 that automates comprehensive image processing tasks locally. It processes all images from a specified input directory, mandatorily converting PNG to JPEG (with size-based exceptions), and applies iterative resizing to JPEGs based on configurable size thresholds and reduction steps. The application uses Spectre.Console for enhanced user interaction, configuration files for settings, and UserSecrets for sensitive data. On startup, it prompts the user to initiate processing, ensuring controlled execution of the transformation workflow.

## Key Architectural Drivers
- **Simplicity and Ease of Use**: Designed as a straightforward console app with Spectre.Console for better UX, minimizing complexity.
- **Local File Processing**: Focus on on-premise operation without cloud or database integration for portability.
- **Configurability**: Extensive use of appsettings.json and UserSecrets for paths, thresholds, and resizing parameters.
- **Performance**: Rule-based processing with iterative resizing to optimize file sizes efficiently.
- **User Interaction**: Spectre.Console-based prompts for action confirmation and progress display.

## Folder Structure
The solution follows a simple folder layout:
- `ImageTransformer/`: Root project folder containing the .NET console application code, including Program.cs and supporting classes.
- `Input/`: Directory for source images (configured via appsettings.json).
- `Output/`: Directory for processed images (configured via appsettings.json).
- `.spec-workflow/`: Contains specification and documentation files, including this architecture.md.

## Core Components
* **Console Application**: The main executable that handles user input via Spectre.Console, configuration loading, and image processing logic.
  * Primary responsibility: Orchestrate the image transformation workflow based on rules and configurations.
* **Input Folder**: File system directory containing source images.
  * Primary responsibility: Provide source images for processing.
* **Output Folder**: File system directory for storing transformed images.
  * Primary responsibility: Store the results of image transformations.
* **Configuration System**: appsettings.json and UserSecrets.
  * Primary responsibility: Manage application settings, including folder paths, size thresholds, and resizing parameters.

## Key Features / Modules
* Mandatory PNG to JPEG Conversion: Converts all PNG images to JPEG, except those below the size threshold.
* Iterative JPEG Resizing: Resizes JPEGs if >5 MB, reducing by the step percentage each iteration until target size is reached.
* Size Threshold Handling: Files below threshold are copied as-is without conversion or resizing.
* User Prompting: Spectre.Console prompts for processing initiation and displays progress.
* Configuration Management: Load settings from appsettings.json with UserSecrets support, including target size and reduction step.

## Component Interactions
The console application uses Spectre.Console for user prompts and progress indicators. It interacts with the file system to read from the Input folder and write to the Output folder. Configuration is loaded from appsettings.json and UserSecrets at startup. All interactions are local; no external services or protocols are used.

## Architectural Patterns and Principles
* **Procedural Architecture**: Sequential processing flow with rule-based decisions.
* **Configuration-Driven Design**: Externalized settings for flexibility.
* **SOLID Principles**: Single Responsibility for conversion and resizing modules, Open/Closed for extensible logic.
* **Fail-Safe Processing**: Error handling and iterative resizing to ensure size compliance.

## Data Flow (High-Level)
1. Application starts, loads configuration from appsettings.json and UserSecrets.
2. User is prompted via Spectre.Console to initiate processing.
3. Application scans all images in the Input folder.
4. For each image:
   - If PNG and size >= threshold, convert to JPEG.
   - If PNG and size < threshold, copy as-is to Output.
   - If JPEG (original or converted):
     - If size <= 5 MB, save to Output.
     - If size > 5 MB, resize by reducing dimensions by the step percentage.
     - Check result size; if > target size, resize original again with an additional step reduction, repeat until <= target size.
5. Process completes with Spectre.Console summary output.