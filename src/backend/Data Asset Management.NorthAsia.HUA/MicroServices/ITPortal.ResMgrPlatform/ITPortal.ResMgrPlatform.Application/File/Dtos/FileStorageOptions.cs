using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITPortal.ResMgrPlatform.Application.Dtos
{
    // 在配置文件中添加共享目录设置
    public class FileStorageOptions
    {
        // 共享目录路径，例如：
        // Windows: \\server\share
        // Linux: //server/share 或 smb://server/share
        public string SharedDirectoryPath { get; set; }

        // 访问共享目录的用户名
        public string Username { get; set; }

        // 访问共享目录的密码
        public string Password { get; set; }

        // 共享目录所在的域（可选）
        public string Domain { get; set; } = "";

        // 本地临时目录（用于中转）
        public string LocalTempDirectory { get; set; } = Path.Combine(Path.GetTempPath(), "FileUploadTemp");
    }

}
