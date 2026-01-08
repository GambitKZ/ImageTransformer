# Task 003: Integrate Image Scanning into Program.cs

## Objective
Wire up the ImageScanner service in dependency injection and call it during the application workflow to discover images.

## Deliverables
- Register `IImageScanner` in the service collection
- Call image scanning after configuration is loaded and user confirms
- Store discovered images for downstream processing
- Display result to user via Spectre.Console

## Implementation Details
1. In `Program.cs`:
   - Add `services.AddScoped<IImageScanner, ImageScanner>()` to DI container
2. After user confirms processing start (FR-002):
   - Call `imageScanner.ScanAsync(inputPath)` where inputPath comes from configuration
   - Store result (list of image paths) for use in subsequent processing steps
   - Display to user: "Found X images in input folder"
   - If no images found, inform user but allow continuation

## Acceptance Criteria
- [ ] IImageScanner registered in dependency injection
- [ ] ScanAsync called after user confirmation to process
- [ ] Input path retrieved from IConfiguration
- [ ] Discovery result displayed to user
- [ ] Handles scenario where no images found gracefully
- [ ] Image list available for downstream processing
