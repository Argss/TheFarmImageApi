using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TheFarmImageApi.Models;

namespace TheFarmImageApi.Controllers
{
    /// <summary>
    /// Controller responsible for createing ImageMetaData and Uploading Downloading imageFiles
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ImageController : ControllerBase
    {
        /// <summary>
        /// Return a list of all imageMetaDatas
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetImageMetaData()
        {
            //SELECT * FROM imageMetaData
            //use datareader iterate push into a list

            //No database so just return som play/mock data
            List<ImageMetaData> images = new List<ImageMetaData>
            {
                new ImageMetaData() {Id = 0, UserName = "Anton", ImageFileName = "selfie.jpg"},
                new ImageMetaData() {Id = 1, UserName = "Anton", ImageFileName = "test.png"},
                new ImageMetaData() {Id = 2, UserName = "Anton", ImageFileName = "test2.jpg"}

            };

            return Ok(images);
        }

        /// <summary>
        /// Create new ImageMetaData
        /// </summary>
        /// <param name="createImageMetaData"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateImageMetaData(CreateImageMetaData createImageMetaData)
        {
            //Create a entry in our database and store the metaData and get a id wich would be our primaryKey
            //INSERT INTO imageMetaData ('userName', 'fileName') VALUES(@userName, @fileName)
            //and then get last primaryKey from command
            int primaryKeyId = 0;

            ImageMetaData imageMetaData = new ImageMetaData
            {
                Id = primaryKeyId,
                UserName = createImageMetaData.UserName,
                ImageFileName = null
            };

            return Ok(imageMetaData);
        }

        /// <summary>
        /// Upload a image file
        /// </summary>
        /// <param name="image"></param>
        /// <param name="createImageMetaData"></param>
        /// <returns></returns>
        [HttpPost("ImageFile/{id}")]
        public IActionResult UploadImage(int id, [Required]IFormFile image)
        {
            string fileExt = Path.GetExtension(image.FileName);

            //Validate file endings
            if ((fileExt.Contains(".jpg", StringComparison.OrdinalIgnoreCase) ||
                 fileExt.Contains(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                 fileExt.Contains(".png", StringComparison.OrdinalIgnoreCase) ||
                 fileExt.Contains(".bmp", StringComparison.OrdinalIgnoreCase) ||
                 fileExt.Contains(".gif", StringComparison.OrdinalIgnoreCase)) == false)
            {
                throw new Exception("Unsupported file ending");
            }

            //sanity check file size
            if (image.Length > 1024*50)
            {
                throw new Exception("The image file was to large 50 MB is max");
            }

            //Find metaData based on id
            //SELECT * FROM imageMetaData WHERE id=@id LIMIT 1
            //if no imageMetaData found return a error

            ImageMetaData imageMetaData = new ImageMetaData
            {
                Id = id,
                UserName = "Anton",
                ImageFileName = $"{id}{ fileExt }"
            };

            using (FileStream fileStream = new FileStream(imageMetaData.ImageFileName, FileMode.Create))
            {
                image.CopyTo(fileStream);
            }
            
            return Ok(imageMetaData);
        }

        /// <summary>
        /// Get ImageMetaData by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetImageMetaDataById([Required] int id)
        {
            // SELECT * FROM imageMetaData WHERE id = @id something like that

            return Ok(new ImageMetaData
            {
                Id = 0,
                UserName = "Anton",
                ImageFileName = "test.png"
            });
        }

        /// <summary>
        /// Get a imageFileby id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("ImageFile/{id}")] 
        public IActionResult GetImageFile([Required] int id)
        {
            //Run a select on our database and get the ImageMetaData so we can return the correct Image
            // SELECT * FROM imageMetaData WHERE id = @id something like that
            string fileName = "test.png";

            FileStream fileStream = new FileStream(fileName, FileMode.Open);

            if (new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider().TryGetContentType(Path.GetFileName(fileStream.Name), out string mimeType) == false)
            {
                mimeType = "application/octet-stream";
            }

            return File(fileStream, mimeType);
        }
    }
}
