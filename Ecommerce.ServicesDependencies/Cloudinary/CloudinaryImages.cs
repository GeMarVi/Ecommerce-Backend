using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Ecommerce.ServicesDependencies.CloudinaryImages
{
    public class CloudinaryResult
    {
        public List<string> ImageUrls { get; set; }
        public bool IsSuccess { get; set; }
        public string ErrorMessage { get; set; }
    }
    public class CloudinaryImplementation
    {
        private Cloudinary _cloudinary;

        public string CLOUD_NAME = "dnsfb2er0";
        public string API_KEY = "154192982173965";
        public string API_SECRET = "58MDM4p8_FeLNTAGJmrn1WQt8qU";

        public CloudinaryImplementation()
        {
            Account account = new Account(CLOUD_NAME, API_KEY, API_SECRET);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<CloudinaryResult> UploadImages(List<IFormFile> images, string publicId)
        {
            CloudinaryResult cloudinaryUpload = new CloudinaryResult
            {
                ImageUrls = new List<string>(),
                IsSuccess = true,
                ErrorMessage = null,
            };

            var count = 1;
            foreach (var image in images)
            {
                using (var stream = image.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(image.FileName, stream),
                        PublicId = publicId.Replace(" ", "") + count,
                        Folder = "ecommerce/producto/"
                    };

                    try
                    {
                        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                        cloudinaryUpload.ImageUrls.Add(uploadResult.SecureUrl.ToString());
                        count++;
                    }
                    catch (Exception ex)
                    {
                        cloudinaryUpload.IsSuccess = false;
                        cloudinaryUpload.ErrorMessage = ex.Message;
                    }
                }
            }
            return cloudinaryUpload;
        }

        public async Task<CloudinaryResult> UploadImage(IFormFile image, string public_Id)
        {
            CloudinaryResult cloudinaryUpload = new CloudinaryResult
            {
                ImageUrls = new List<string>(),
                IsSuccess = true,
                ErrorMessage = null,
            };

            using (var stream = image.OpenReadStream())
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(image.FileName, stream),
                    PublicId = public_Id,
                };

                try
                {
                    var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                    cloudinaryUpload.ImageUrls.Add(uploadResult.SecureUrl.ToString());

                }
                catch (Exception ex)
                {
                    cloudinaryUpload.IsSuccess = false;
                    cloudinaryUpload.ErrorMessage = ex.Message;
                }
            }

            return cloudinaryUpload;
        }

        public async Task<bool> DeleteImage(List<string> urls)
        {
            List<bool> response = new List<bool>();

            foreach (var url in urls)
            {
                string imageId = $"ecommerce/producto/{ExtractImageId(url)}";

                var deletionParams = new DeletionParams(imageId);

                var result = await _cloudinary.DestroyAsync(deletionParams);

                if (result.Result == "ok")
                {
                    response.Add(true);
                }
                else
                {
                    response.Add(false);
                }
            }

            return !response.Contains(false);
        }

        public string ExtractImageId(string url)
        {
            string pattern = @"/([^/]+)\.\w+$";
            Match match = Regex.Match(url, pattern);

            return match.Success ? match.Groups[1].Value : null;
        }
    }
}

