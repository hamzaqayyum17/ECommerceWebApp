namespace ECommerceApp.Services
{
    public class ImageService
    {
        private readonly IWebHostEnvironment _env;

        public ImageService(IWebHostEnvironment env) => _env = env;

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            // wwwroot/images/products/ folder mein save karo
            string folder = Path.Combine(_env.WebRootPath,
                                           "images", "products");
            Directory.CreateDirectory(folder);

            // Unique file name banao
            string fileName = Guid.NewGuid().ToString()
                              + Path.GetExtension(file.FileName);
            string filePath = Path.Combine(folder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return "/images/products/" + fileName;
        }

        public void DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return;
            string filePath = Path.Combine(_env.WebRootPath,
                                           imageUrl.TrimStart('/'));
            if (File.Exists(filePath))
                File.Delete(filePath);
        }
    }
}