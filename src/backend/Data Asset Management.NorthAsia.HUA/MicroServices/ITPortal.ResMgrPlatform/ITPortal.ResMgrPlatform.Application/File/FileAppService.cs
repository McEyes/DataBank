using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.MachineLearning;

using ITPortal.Core.Services;
using ITPortal.ResMgrPlatform.Application.Dtos;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

using SMBLibrary;
using SMBLibrary.Client;

using System.Buffers;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ITPortal.ResMgrPlatform.Application
{
    /// <summary>
    /// 系统服务接口
    /// </summary>
    [AppAuthorize]
    [Route("api/file/", Name = "数据资产 Feedback服务")]
    public class FileAppService : IDynamicApiController
    {
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<FileAppService> _logger;
        private readonly FileStorageOptions options;
        private const long MaxFileSize = 200 * 1024 * 1024; // 100MB
        public FileAppService(IWebHostEnvironment environment, IOptions<FileStorageOptions> storageOptions, ILogger<FileAppService> logger)
        {
            _environment = environment;
            _logger = logger;
            options = storageOptions.Value;
        }

        [HttpPost("upload"), NonUnify]
        [RequestSizeLimit(MaxFileSize)]
        //[RequestSizeLimit(100_000_000)] // 通常设置为 [RequestSizeLimit(long.MaxValue)]
        [RequestFormLimits(MultipartBodyLengthLimit = long.MaxValue)]
        public async Task<IResult<FileAttachment>> UploadFileAsync(IFormFile file, string category = "", bool keepName = false)
        {
            var result = new Result<FileAttachment>();
            //var category = "DataAsset";
            category = category?.ToLower();
            if (file == null || file.Length == 0)
            {
                result.SetError("The file cannot be empty");
                return result;
            }

            if (file.Length > MaxFileSize)
            {
                result.SetError($"The file size exceeds the limit, the maximum supported{MaxFileSize / (1024 * 1024)}MB");
            }
            var filePath = string.Empty;
            try
            {
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var safeFileName = Path.GetFileNameWithoutExtension(
                    ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Value.Trim('"'));

                var uploadDate = DateTimeOffset.Now;
                var folderPath = Path.Combine(
                    _environment.ContentRootPath,
                    "uploads",
                    category);

                Directory.CreateDirectory(folderPath);

                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                if (keepName) uniqueFileName = file.FileName;
                filePath = Path.Combine(folderPath, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var upLoadResult = await UploadToSMBShareDir(category, uniqueFileName, filePath);
                if (!upLoadResult.Success)
                {
                    return upLoadResult;
                }

                var fileInfo = new FileInfo(filePath);
                //var downloadUrl = $"{uniqueFileName}?category={category}";
                result.Data = new FileAttachment
                {
                    FileName = file.FileName,
                    FileUrl = uniqueFileName,//downloadUrl,
                    FileSize = fileInfo.Length,
                    FileType = fileExtension,
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
            finally
            {
                // 清理临时文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
        }


        //[HttpPost, NonUnify]
        //public async Task<IResult<FileAttachment>> UploadFileAsync([FromBody] string fileBase64, string clientFileName)
        //{
        //    var result = new Result<FileAttachment>();
        //    // 将 base64 字符串转 byte[]
        //    var bytes = Convert.FromBase64String(fileBase64);

        //    var category = "DataAsset";
        //    if (bytes == null || bytes.Length == 0)
        //    {
        //        result.SetError("The file cannot be empty");
        //        return result;
        //    }

        //    if (bytes.Length > MaxFileSize)
        //    {
        //        result.SetError($"The file size exceeds the limit, the maximum supported{MaxFileSize / (1024 * 1024)}MB");
        //    }
        //    // 如：保存到网站根目录下的 uploads 目录
        //    var savePath = Path.Combine(App.HostEnvironment.ContentRootPath, "uploads", category);
        //    if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);


        //    // 这里还可以获取文件的信息
        //    // var size = bytes.Length / 1024.0;  // 文件大小 KB

        //    // 避免文件名重复，采用 GUID 生成
        //    var fileExtension = Path.GetExtension(clientFileName);
        //    var fileName = Guid.NewGuid().ToString("N") + fileExtension;
        //    var filePath = Path.Combine(savePath, fileName);

        //    // 保存到指定路径
        //    using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.Read))
        //    {
        //        await fs.WriteAsync(bytes);
        //    }

        //    //var downloadUrl = $"{fileName}?category={category}";
        //    result.Data = new FileAttachment
        //    {
        //        FileName = clientFileName,
        //        FileUrl = fileName,// downloadUrl,
        //        FileSize = bytes.Length,
        //        FileType = fileExtension,
        //        //PreviewUrl = $"{downloadUrl}?act=view"
        //    };

        //    // 返回文件名（这里可以自由返回更多信息）
        //    return result;
        //}


        [AllowAnonymous]
        [HttpGet("download/{fileName}"), NonUnify]
        public async Task<IActionResult> DownloadFile(string fileName, string downName, string category = "")
        {
            try
            {
                var result = await DownFromSMBShareDir(fileName, downName, category);
                if (result.Success)
                {
                    return result.Data;
                }

                category = category?.ToLower();
                var filePath = Path.Combine(
                    _environment.ContentRootPath,
                    "uploads",
                    category,
                    fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogError($"{fileName} file  not exist");
                    return new NotFoundResult();
                }

                var fileInfo = new FileInfo(filePath);
                var fileExtension = Path.GetExtension(fileName).ToLowerInvariant();

                if (App.HttpContext.Request.Query.ContainsKey("act") && App.HttpContext.Request.Query["act"] == "view")
                {
                    return PreviewFile(fileName, filePath, fileExtension, downName);
                }

                var contentDisposition = new ContentDispositionHeaderValue("attachment")
                {
                    FileNameStar = downName ?? fileName,
                    Size = fileInfo.Length
                };

                App.HttpContext.Response.Headers[HeaderNames.ContentDisposition] = contentDisposition.ToString();
                var fileSteam = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
                var file= new FileStreamResult(fileSteam, GetMimeType(fileExtension));
                file.FileDownloadName = downName ?? fileName; // 配置文件下载显示名
                file.EnableRangeProcessing = true;
                return file;
                //return new FileStreamResult(fileSteam, "application/octet-stream")// GetMimeType(fileExtension))// "application/octet-stream")
                //{
                //    FileDownloadName = downName ?? fileName // 配置文件下载显示名
                //};
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"down {fileName} file error:{ex.Message}");
                throw ex;
            }
            //return PhysicalFile(filePath, GetMimeType(fileExtension));

        }

        private IActionResult PreviewFile(string fileName, string filePath, string fileExtension, string downName)
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
                FileDownloadName = downName??fileName // 配置文件下载显示名
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


        private async Task<Result<FileAttachment>> UploadToSMBShareDir(string category, string uniqueFileName, string localTempFilePath)
        {
            var result = new Result<FileAttachment>();

            // 解析共享目录路径
            var (serverName, shareName, subPath) = ParseSharedPath(options.SharedDirectoryPath);

            // 构建共享目录中的完整路径
            var remoteDirectory = Path.Combine(subPath,category).Replace('\\', '/');
            var remoteFilePath = Path.Combine(remoteDirectory, uniqueFileName).Replace('\\', '/');

            // 替换原有的SMB客户端使用代码，改为手动管理连接
            // 上传到共享目录 - 手动管理SMB连接资源
            var client = new SMB2Client();
            try
            {
                bool isConnected = client.Connect(IPAddress.Parse(serverName), SMBTransportType.DirectTCPTransport);
                if (!isConnected)
                {
                    result.SetError("Failed to connect to the file server");
                    return result;
                }

                // 认证 - 处理返回NTStatus的情况
                NTStatus loginStatus = client.Login(options.Domain, options.Username, options.Password);
                if (loginStatus != NTStatus.STATUS_SUCCESS)
                {
                    result.SetError($"Failed to authenticate with the file server: {loginStatus}");
                    // 认证失败后需要断开连接
                    client.Disconnect();
                    return result;
                }

                // 连接到共享
                ISMBFileStore fileStore = client.TreeConnect(shareName, out NTStatus status);
                if (status != NTStatus.STATUS_SUCCESS)
                {
                    result.SetError($"Failed to connect to share: {status}");
                    // 连接共享失败后需要登出并断开连接
                    client.Logoff();
                    client.Disconnect();
                    return result;
                }

                try
                {
                    // 确保远程目录存在
                    // 优化目录创建 - 减少重复创建检查
                    await Task.Run(() => CreateRemoteDirectory(fileStore, remoteDirectory));

                    // 上传文件 - 使用FileStore接口的CreateFile方法
                    using (var localFileStream = File.OpenRead(localTempFilePath))
                    {
                        // 使用FileStore接口创建文件
                        status = fileStore.CreateFile(out object fileHandle, out FileStatus fileStatus,
                            remoteFilePath, AccessMask.GENERIC_WRITE | AccessMask.SYNCHRONIZE, SMBLibrary.FileAttributes.Normal,
                            ShareAccess.Read, CreateDisposition.FILE_CREATE, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);

                        if (status == NTStatus.STATUS_SUCCESS)
                        {
                            long writeOffset = 0;
                            while (localFileStream.Position < localFileStream.Length)
                            {
                                byte[] buffer = new byte[(int)client.MaxWriteSize];
                                int bytesRead = localFileStream.Read(buffer, 0, buffer.Length);
                                if (bytesRead < (int)client.MaxWriteSize)
                                {
                                    Array.Resize<byte>(ref buffer, bytesRead);
                                }
                                int numberOfBytesWritten;
                                status = fileStore.WriteFile(out numberOfBytesWritten, fileHandle, writeOffset, buffer);
                                if (status != NTStatus.STATUS_SUCCESS)
                                {
                                    throw new Exception("Failed to write to file");
                                }
                                writeOffset += bytesRead;
                            }
                            status = fileStore.CloseFile(fileHandle);
                        }
                        else
                        {
                            result.SetError($"Failed to create remote file: {status}");
                            return result;
                        }
                    }
                }
                finally
                {
                    status = fileStore.Disconnect();
                }
                // 确保客户端登出
                client.Logoff();
            }
            finally
            {
                // 手动断开客户端连接
                if (client.IsConnected)
                {
                    client.Disconnect();
                }
            }

            return result;
        }



        private async Task<Result<FileStreamResult>> DownFromSMBShareDir(string fileName, string downName, string category = "")
        {
            var result = new Result<FileStreamResult>();

            // 解析共享目录路径
            var (serverName, shareName, subPath) = ParseSharedPath(options.SharedDirectoryPath);
            // 构建共享目录中的完整路径
            var remoteDirectory = Path.Combine(subPath, category).Replace('\\', '/');
            var remoteFilePath = Path.Combine(remoteDirectory, fileName).Replace('\\', '/');
            var fileExtension = Path.GetExtension(remoteFilePath).ToLowerInvariant();
            // 替换原有的SMB客户端使用代码，改为手动管理连接
            // 上传到共享目录 - 手动管理SMB连接资源
            var client = new SMB2Client();
            try
            {
                bool isConnected = client.Connect(IPAddress.Parse(serverName), SMBTransportType.DirectTCPTransport);
                if (!isConnected)
                {
                    result.SetError("Failed to connect to the file server");
                    return result;
                }

                // 认证 - 处理返回NTStatus的情况
                NTStatus loginStatus = client.Login(options.Domain, options.Username, options.Password);
                if (loginStatus != NTStatus.STATUS_SUCCESS)
                {
                    result.SetError($"Failed to authenticate with the file server: {loginStatus}");
                    // 认证失败后需要断开连接
                    client.Disconnect();
                    return result;
                }

                // 连接到共享
                ISMBFileStore fileStore = client.TreeConnect(shareName, out NTStatus status);
                if (status != NTStatus.STATUS_SUCCESS)
                {
                    result.SetError($"Failed to connect to share: {status}");
                    // 连接共享失败后需要登出并断开连接
                    client.Logoff();
                    client.Disconnect();
                    return result;
                }

                try
                {

                    // 使用FileStore接口创建文件
                    status = fileStore.CreateFile(out object fileHandle, out FileStatus fileStatus,
                            remoteFilePath, AccessMask.GENERIC_READ | AccessMask.SYNCHRONIZE, SMBLibrary.FileAttributes.Normal,
                            ShareAccess.Read, CreateDisposition.FILE_OPEN, CreateOptions.FILE_NON_DIRECTORY_FILE | CreateOptions.FILE_SYNCHRONOUS_IO_ALERT, null);

                    if (status == NTStatus.STATUS_SUCCESS)
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        byte[] data;
                        long bytesRead = 0;
                        while (true)
                        {
                            status = fileStore.ReadFile(out data, fileHandle, bytesRead, (int)client.MaxReadSize);
                            if (status != NTStatus.STATUS_SUCCESS && status != NTStatus.STATUS_END_OF_FILE)
                            {
                                throw new Exception("Failed to read from file");
                            }

                            if (status == NTStatus.STATUS_END_OF_FILE || data.Length == 0)
                            {
                                break;
                            }
                            bytesRead += data.Length;
                            stream.Write(data, 0, data.Length);
                        }
                        // 重置流位置到起始点
                        stream.Position = 0;
                        result.Data = new FileStreamResult(stream, GetMimeType(fileExtension));
                        result.Data.FileDownloadName = downName ?? fileName; // 配置文件下载显示名
                        result.Data.EnableRangeProcessing = true;
                    }
                    else
                    {
                        result.SetError($"Failed to create remote file: {status}");
                        return result;
                    }
                    status = fileStore.CloseFile(fileHandle);
                }
                finally
                {
                    status = fileStore.Disconnect();
                }
                // 确保客户端登出
                client.Logoff();
            }
            finally
            {
                // 手动断开客户端连接
                if (client.IsConnected)
                {
                    client.Disconnect();
                }
            }

            return result;
        }



        // 辅助方法：解析共享路径
        private (string ServerName, string ShareName, string SubPath) ParseSharedPath(string path)
        {
            // 处理Windows格式: \\server\share\path
            if (path.StartsWith(@"\\", StringComparison.OrdinalIgnoreCase))
            {
                var parts = path.Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    string serverName = parts[0];
                    string shareName = parts[1];
                    string subPath = parts.Length > 2 ? string.Join("\\", parts.Skip(2)) : "";
                    return (serverName, shareName, subPath);
                }
            }
            // 处理Linux格式: //server/share/path 或 smb://server/share/path
            else if (path.StartsWith("//", StringComparison.OrdinalIgnoreCase) ||
                     path.StartsWith("smb://", StringComparison.OrdinalIgnoreCase))
            {
                string cleanedPath = path.StartsWith("smb://") ? path.Substring(6) : path;
                var parts = cleanedPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2)
                {
                    string serverName = parts[0];
                    string shareName = parts[1];
                    string subPath = parts.Length > 2 ? string.Join("/", parts.Skip(2)) : "";
                    return (serverName, shareName, subPath);
                }
            }

            throw new ArgumentException("Invalid shared path format", nameof(path));
        }

        // 辅助方法：创建远程目录
        private void CreateRemoteDirectory(ISMBFileStore fileShare, string remotePath)
        {
            string[] directories = remotePath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string currentPath = "";

            foreach (string dir in directories)
            {
                string nextPath = currentPath == "" ? dir : $"{currentPath}/{dir}";
                NTStatus status = fileShare.CreateFile(
                    out object directoryHandle,
                    out FileStatus fileStatus,
                    nextPath,
                    AccessMask.GENERIC_WRITE,
                    SMBLibrary.FileAttributes.Directory,
                    ShareAccess.Read,
                    CreateDisposition.FILE_CREATE,
                    CreateOptions.FILE_DIRECTORY_FILE,
                    null);

                if (status == NTStatus.STATUS_SUCCESS)
                {
                    fileShare.CloseFile(directoryHandle);
                }
                else if (status != NTStatus.STATUS_OBJECT_NAME_COLLISION)
                {
                    throw new IOException($"Failed to create directory {nextPath}: {status}");
                }

                currentPath = nextPath;
            }
        }
    }
}
