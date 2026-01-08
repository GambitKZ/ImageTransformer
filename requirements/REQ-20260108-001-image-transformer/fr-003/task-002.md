# Task 002: Implement ImageScanner Service

## Objective
Implement the IImageScanner interface to provide actual image discovery functionality using System.IO APIs.

## Deliverables
- Create `ImageScanner` class implementing `IImageScanner`
- Enumerate files in input folder with supported image extensions
- Handle exceptions gracefully

## Implementation Details
1. Create `src/ImageTransformer.Console/Services/ImageScanner.cs`
2. Implement ScanAsync method:
   - Use `Directory.EnumerateFiles()` to discover files
   - Filter for .png, .jpg, .jpeg extensions (case-insensitive)
   - Return full file paths as ordered collection
3. Exception handling:
   - Catch `DirectoryNotFoundException` and re-throw or log appropriately
   - Catch `UnauthorizedAccessException` for permission issues
   - Return empty collection if no images found (not an error condition)
4. Use modern C# patterns (async/await, LINQ)

## Acceptance Criteria
- [ ] ImageScanner class created implementing IImageScanner
- [ ] ScanAsync discovers .png, .jpg, .jpeg files (case-insensitive)
- [ ] Returns full file paths
- [ ] Handles directory access exceptions
- [ ] Returns empty collection if no images found
- [ ] Method is async and properly awaitable
