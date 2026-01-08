# REQ-20260108-001:FR-002 — Display Processing Progress with File Details

# User Story
As a user, I want to see the overall progress via a progress bar, the current file being processed, and results per file so that I have real-time feedback during image transformation.

# Acceptance Criteria (Given / When / Then)
- Given processing has started
- When images are being processed
- Then a progress bar displays overall completion
- Then the current file name is shown
- Then after each file, a result message is displayed (e.g., "File1.png with size xxxx KB was changed to jpg and resized to XX percent. Result size xxxxx KB")

# Test Outline
1. Initiate processing.
2. Observe the progress bar updating as files are processed.
3. Verify the current file is displayed.
4. Check that a detailed result message appears for each file.

# Rationale
This provides comprehensive feedback, allowing users to monitor the transformation process in real-time.