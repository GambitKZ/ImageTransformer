# Task 002: Implement ProcessJpegImage in ImageProcessor Class

Implement the ProcessJpegImage method in the ImageProcessor class. First, add a MaxResizeIterations configuration value to appsettings.json (default to 10). Then, use NetVips to load the JPEG, check its size against JpegTargetSizeMB, and iteratively resize by reducing dimensions by ResizeStepPercentage until the size is below the target or the maximum iterations is reached. Save the final image to the output directory.

Deliverable: Updated appsettings.json with MaxResizeIterations, and updated ImageProcessor.cs with the iterative resizing logic, including error handling and comments explaining the design. (Depends on Task 001.)