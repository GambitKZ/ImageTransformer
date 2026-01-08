# Task 003: Implement ImageProcessor Class

Implement the ImageProcessor class in the Services folder that realizes the IImageProcessor interface. Use NetVips to load PNG images, check file size against PngSizeThresholdMB from configuration, convert to JPEG if above threshold, or copy the file as-is otherwise. Ensure proper error handling and output to the configured output folder.

Deliverable: New ImageProcessor.cs file created in the Services folder. (Depends on Task 001 for the library.)