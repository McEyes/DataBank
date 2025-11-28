using Elastic.Clients.Elasticsearch;

using ITPortal.Core.Services;
using ITPortal.ResMgrPlatform.Application.File.Dtos;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

using System.IO;

namespace ITPortal.ResMgrPlatform.Application
{
    public class FileService : IFileService, ITransient
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileService> _logger;
        private const long MaxFileSize = 200 * 1024 * 1024; // 100MB

        public FileService(IWebHostEnvironment environment, ILogger<FileService> logger)
        {
            _environment = environment;
            _logger = logger;
        }

        [HttpPost("upload")]
        [RequestSizeLimit(MaxFileSize)]
        //[RequestSizeLimit(100_000_000)] // 通常设置为 [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IResult<FileAttachment>> UploadFileAsync(IFormFile file)
        {
            var result = new Result<FileAttachment>();
            var category = "DataAsset";
            if (file == null || file.Length == 0)
            {
                result.SetError("The file cannot be empty");
                return result;
            }

            if (file.Length > MaxFileSize)
            {
                result.SetError($"The file size exceeds the limit, the maximum supported{MaxFileSize / (1024 * 1024)}MB");
            }

            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var safeFileName = Path.GetFileNameWithoutExtension(
                    ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim('"'));

                var uploadDate = DateTime.Now;
                var folderPath = Path.Combine(
                    _environment.ContentRootPath,
                    "Uploads",
                    category);

                Directory.CreateDirectory(folderPath);

                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var fileInfo = new FileInfo(filePath);
                var downloadUrl = $"/File/download/{category}/{uniqueFileName}";

                result.Data = new FileAttachment
                {
                    FileName = file.FileName,
                    FilePath = downloadUrl,
                    FileSize = fileInfo.Length,
                    FileExtension = fileExtension,
                    //PreviewUrl = $"{downloadUrl}?act=view"
                };

                return result;
            }
            catch (Exception ex)
            {
                result.SetError("File upload failed");
                _logger.LogError(ex, "File upload failed,\r\n" + ex.Message);
                return result;
            }
        }
        [HttpPost]
        public async Task<IResult<FileAttachment>> UploadFileAsync([FromBody] string fileBase64, string clientFileName)
        {
            var result = new Result<FileAttachment>();
            // 将 base64 字符串转 byte[]
            var bytes = Convert.FromBase64String(fileBase64);

            var category = "DataAsset";
            if (bytes == null || bytes.Length == 0)
            {
                result.SetError("The file cannot be empty");
                return result;
            }

            if (bytes.Length > MaxFileSize)
            {
                result.SetError($"The file size exceeds the limit, the maximum supported{MaxFileSize / (1024 * 1024)}MB");
            }
            // 如：保存到网站根目录下的 uploads 目录
            var savePath = Path.Combine(App.HostEnvironment.ContentRootPath, "uploads", category);
            if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);


            // 这里还可以获取文件的信息
            // var size = bytes.Length / 1024.0;  // 文件大小 KB

            // 避免文件名重复，采用 GUID 生成
            var fileExtension = Path.GetExtension(clientFileName);
            var fileName = Guid.NewGuid().ToString("N") + fileExtension;
            var filePath = Path.Combine(savePath, fileName);

            // 保存到指定路径
            using (var fs = new FileStream(filePath, FileMode.Create))
            {
                await fs.WriteAsync(bytes);
            }

            result.Data = new FileAttachment
            {
                FileName = clientFileName,
                FilePath = Path.Combine(category, fileName),
                FileSize = bytes.Length,
                FileExtension = fileExtension,
                //PreviewUrl = $"{downloadUrl}?act=view"
            };

            // 返回文件名（这里可以自由返回更多信息）
            return result;
        }


        [HttpGet("download/{category}/{fileName}"), NonUnify]
        public IActionResult DownloadFile(string category, string fileName)
        {
            var result = new Result<FileAttachment>();
            var filePath = Path.Combine(
                _environment.ContentRootPath,
                "Uploads",
                category,
                fileName);

            if (!System.IO.File.Exists(filePath))
            {
                result.SetError("File does not exist", 404);
                return new NotFoundResult();
            }

            var fileInfo = new FileInfo(filePath);
            var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

            if (App.HttpContext.Request.Query.ContainsKey("act") && App.HttpContext.Request.Query["act"] == "view")
            {
                return PreviewFile(fileName,filePath, fileExtension);
            }

            var contentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileNameStar = fileName,
                Size = fileInfo.Length
            };

            App.HttpContext.Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            return new FileStreamResult(new FileStream(filePath, FileMode.Open), GetMimeType(fileExtension))// "application/octet-stream")
            {
                FileDownloadName = fileName // 配置文件下载显示名
            };

            //return PhysicalFile(filePath, GetMimeType(fileExtension));

        }

        private IActionResult PreviewFile(string fileName,string filePath, string fileExtension)
        {
            var previewableExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf", ".txt", ".html", ".htm" };

            if (!previewableExtensions.Contains(fileExtension))
                throw new AppFriendlyException("此文件类型不支持预览", 500);

            var contentDisposition = new ContentDispositionHeaderValue("inline")
            {
                FileNameStar = Path.GetFileName(filePath)
            };

            App.HttpContext.Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
            return new FileStreamResult(new FileStream(filePath, FileMode.Open), GetMimeType(fileExtension))// "application/octet-stream")
            {
                FileDownloadName = fileName // 配置文件下载显示名
            };

            //return PhysicalFile(filePath, GetMimeType(fileExtension));
        }


        private string GetMimeType(string extension)
        {
            var mimeTypes = new Dictionary<string, string>
            {
                { ".jpg", "image/jpeg" },
                { ".jpeg", "image/jpeg" },
                { ".png", "image/png" },
                { ".gif", "image/gif" },
                { ".pdf", "application/pdf" },
                { ".txt", "text/plain" },
                { ".html", "text/html" },
                { ".htm", "text/html" },
                { ".doc", "application/msword" },
                { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
                { ".xls", "application/vnd.ms-excel" },
                { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
                { ".ppt", "application/vnd.ms-powerpoint" },
                { ".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation" },
                { ".mp4", "video/mp4" },
                { ".mp3", "audio/mpeg" },
                { ".zip", "application/zip" },
                { ".rar", "application/x-rar-compressed" },
                { ".7z", "application/x-7z-compressed" }
            };

            return mimeTypes.TryGetValue(extension.ToLowerInvariant(), out var mimeType)
                ? mimeType
                : "application/octet-stream";
        }
    }
}
