using System.Diagnostics;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace Sqllineage.Controllers
{
    [Route("[controller]")]
    public class SqllineageController : Controller
    {
        [HttpPost]
        public IActionResult Index([FromBody] SqllineageCommandDto dto)
        {
            ArgumentNullException.ThrowIfNull(dto);
            ArgumentNullException.ThrowIfNullOrWhiteSpace(dto.Base64CmdText);

            var result = new ResponseResult { };
            try
            {
                var sqllineage = "sqllineage ";
                var byteCmdText = Convert.FromBase64String(dto.Base64CmdText);
                var cmdText = Encoding.UTF8.GetString(byteCmdText);

                if (!cmdText.Trim().ToLower().StartsWith("sqllineage "))
                {
                    throw new ArgumentException("The execution command is illegal and must begin with the sqllineage command.");
                }

                var cmd = $"/myenv/bin/sqllineage {cmdText.Substring(sqllineage.Length)}";
                var execResult = ExecuteLinuxCommand(cmd);
                result.Succeeded = true;
                result.Data = execResult;
                result.StatusCode = 0;
            }
            catch (Exception ex)
            {
                result.Succeeded = false;
                result.StatusCode = 500;
                result.Errors = ex.Message;
            }

            return Json(result);
        }

        public static string ExecuteLinuxCommand(string command)
        {
            var escapedArgs = command.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.WaitForExit();

            if (!string.IsNullOrEmpty(error))
            {
                throw new Exception($"Command failed: {error}");
            }

            return result;
        }
    }
}
