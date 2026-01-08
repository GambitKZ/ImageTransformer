# ImageTransformer

## Overview

ImageTransformer is a lightweight console application built with .NET 10 that automates comprehensive image processing tasks locally. It processes all images from a specified input directory, mandatorily converting PNG to JPEG (with size-based exceptions), and applies iterative resizing to JPEGs based on configurable size thresholds and reduction steps. The application uses Spectre.Console for enhanced user interaction, configuration files for settings, and UserSecrets for sensitive data. On startup, it prompts the user to initiate processing, ensuring controlled execution of the transformation workflow.

## Primary Goal

The ImageTransformer is a console application designed to process all images from an input folder and transform them into an output folder. PNG images are converted to JPEG format, except for files below a configurable size threshold, which are copied as-is. All JPEG images (including those converted from PNG) undergo additional size checks: if the file size exceeds 5 MB, it is resized by reducing its dimensions by a configurable step percentage (e.g., 3%). If the resized file still exceeds the configured target size, the process repeats using the original image, reducing by an additional step percentage until the target size is met.

## Technology Stack

- **Primary Technology**: .NET 10
- **Backend**: Console Application
- **Database**: None
- **Frontend**: No dedicated frontend - Console only
- **Cloud Platform**: On-premise
- **Other Key Technologies**: appsettings.json for configuration, UserSecrets for sensitive settings, Spectre.Console for console interactions, Image processing libraries such as NetVips or ImageMagick.NET (Magic.Net)

## Key Features

- Mandatory PNG to JPEG Conversion: Converts all PNG images to JPEG, except those below the size threshold.
- Iterative JPEG Resizing: Resizes JPEGs if >5 MB, reducing by the step percentage each iteration until target size is reached.
- Size Threshold Handling: Files below threshold are copied as-is without conversion or resizing.
- User Prompting: Spectre.Console prompts for processing initiation and displays progress.
- Configuration Management: Load settings from appsettings.json with UserSecrets support, including target size and reduction step.

## Installation

1. Ensure you have .NET 10 installed.
2. Clone or download the project.
3. Navigate to the project root.
4. Run dotnet build to build the application.

## Usage

1. Configure the input and output folders, size thresholds, etc., in appsettings.json.
2. Run the application: dotnet run --project src/ImageTransformer.Console/ImageTransformer.Console.csproj
3. Follow the prompts to initiate processing.

## Configuration

The application uses appsettings.json for configuration. Key settings include:

- Input folder path
- Output folder path
- Size threshold for PNG conversion
- Target size for JPEG resizing (5 MB)
- Reduction step percentage

Sensitive settings can be stored in UserSecrets.

## Architecture

### Folder Structure

- ImageTransformer/: Root project folder containing the .NET console application code.
- Input/: Directory for source images.
- Output/: Directory for processed images.
- .spec-workflow/: Contains specification and documentation files.

### Core Components

- **Console Application**: Orchestrates the workflow.
- **Input Folder**: Source images.
- **Output Folder**: Processed images.
- **Configuration System**: appsettings.json and UserSecrets.

### Data Flow

1. Load configuration.
2. Prompt user to start.
3. Scan input folder.
4. Process each image: convert PNG if needed, resize JPEG if too large.
5. Save to output.

For more details, see .spec-workflow/architecture.md.
