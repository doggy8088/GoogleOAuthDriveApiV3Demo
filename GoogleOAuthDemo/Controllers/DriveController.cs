using Google.Apis.Auth.AspNetCore3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GoogleOAuthDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DriveController : ControllerBase
    {
        [HttpGet]
        [GoogleScopedAuthorize(DriveService.ScopeConstants.DriveReadonly)]
        public async Task<IActionResult> DriveFileList([FromServices] IGoogleAuthProvider auth)
        {
            GoogleCredential cred = await auth.GetCredentialAsync();

            var service = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = cred
            });

            var list = service.Files.List();
            list.PageSize = 10;
            list.Spaces = "drive";
            list.SupportsAllDrives = false;
            list.IncludeItemsFromAllDrives = false;
            list.OrderBy = "folder, modifiedTime desc";

            // 列出所有含有 MVC 名稱的資料夾
            list.Q = "mimeType = 'application/vnd.google-apps.folder' and name contains 'MVC'";

            // 列出特定 ID 資料夾下的所有檔案 ( '18EMfZ3xAlL67qNn-8eb4I0c3V5q-0HCb' 是資料夾 ID )
            list.Q = "mimeType != 'application/vnd.google-apps.folder' and '18EMfZ3xAlL67qNn-8eb4I0c3V5q-0HCb' in parents";
            
            // 列出根目錄下的所有檔案
            list.Q = "mimeType != 'application/vnd.google-apps.folder' and 'root' in parents";

            // 列出根目錄下的所有資料夾
            list.Q = "mimeType = 'application/vnd.google-apps.folder' and 'root' in parents";

            // 列出根目錄下的所有檔案與資料夾
            list.Q = "'root' in parents";

            var files = await list.ExecuteAsync();

            var fileNames = files.Files.Select(x => new { x.Id, x.Kind, x.Name, x.MimeType }).ToList();
            return Ok(fileNames);
        }
    }
}
