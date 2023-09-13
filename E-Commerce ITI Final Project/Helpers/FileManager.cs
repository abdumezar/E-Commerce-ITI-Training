namespace E_Commerce_ITI_Final_Project.Helpers
{
    public class FileManager
    {
        public static async Task<string> UploadImageAsync(IWebHostEnvironment environment, IFormFile file, string mainFolder, string FileName)
        {
            try
            {
                // Check if the file is not null and the content length is greater than 0
                if (file != null && file.Length > 0)
                {
                    // Validate the file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        return "Only image files (jpg, jpeg, png, gif) are allowed.";
                    }

                    // Validate file size (e.g., limit to 5MB)
                    var maxFileSize = 5 * 1024 * 1024; // 5MB
                    if (file.Length > maxFileSize)
                    {
                        return "The image file size exceeds the allowed limit.";
                    }

                    // Create a more meaningful filename using the original filename and timestamp;
                    var fileName = $"{FileName}.{Path.GetFileName(file.ContentType)}";

                    // Determine the storage path (you can use configuration settings for this)
                    var storagePath = Path.Combine(environment.WebRootPath, "files", "images", mainFolder, fileName);

                    // if image exists [replace it]
                    var imagesPath = Path.Combine(environment.WebRootPath, "files", "images", mainFolder);
                    string[] filesToDelete = Directory.GetFiles(imagesPath, $"{FileName}.*");

                    if (filesToDelete is not null && filesToDelete.Length > 0)
                        foreach (string filePath in filesToDelete)
                            File.Delete(filePath);

                    // If you choose cloud-based storage, use the respective SDK to upload the file here
                    // Save the file to the specified path
                    using (var fs = new FileStream(storagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fs);
                    }

                    //Process the image(e.g., resize the image)
                    using (var image = Image.Load(storagePath))
                    {
                        // Resize the image to a specific width while preserving the aspect ratio
                        var maxWidth = 800;
                        if (image.Width > maxWidth)
                        {
                            var scaleFactor = (double)maxWidth / image.Width;
                            var newHeight = (int)(image.Height * scaleFactor);
                            image.Mutate(x => x.Resize(maxWidth, newHeight));
                        }

                        // Save the processed image back to the same file path
                        image.Save(storagePath);
                    }

                    // Return the URL of the processed image
                    return $"/files/images/{mainFolder}/{fileName}";
                }

                return "No file uploaded";
            }
            catch (Exception ex)
            {
                return "Error uploading the file";
            }
        }

    }
}
