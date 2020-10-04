using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileUploadAPI.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileUploadAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private IHostingEnvironment _env;

        public UploadController(IHostingEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public IActionResult Upload()
        {

            DirectoryClean();

            var file = Request.Form.Files[0];
            long size = 0;

            var filename = CleanFileName(file.FileName);

            var webRoot = _env.ContentRootPath;
            var filePath = webRoot + "/Uploads" + $@"/{DateTime.Now.ToFileTime()}_{ filename}";

            size += file.Length;
            using (FileStream fs = System.IO.File.Create(filePath))
            {
                file.CopyTo(fs);
                fs.Flush();
            }

            UploadResult result = new UploadResult
            {
                FilePath = filePath
            };
            return Ok(result);
        }

        public string CleanFileName(string filename)
        {
            return Regex.Replace(filename, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
        }

        public void DirectoryClean()
        {

            var webRoot = _env.ContentRootPath;
            var dir = webRoot + "/Uploads";

            try
            {

                DirectoryInfo info = new DirectoryInfo(dir);
                FileInfo[] files = info.GetFiles();
                foreach (FileInfo file in files)
                {
                    DateTime dt = file.LastWriteTime;
                    TimeSpan ts = DateTime.Now - dt;

                    if (ts.TotalMinutes > 10)
                    {
                        System.IO.File.Delete(file.FullName);
                    }

                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        [HttpDelete]
        public IActionResult Delete()
        {

            var webRoot = _env.ContentRootPath;
            var dir = webRoot + "/Uploads";

            try
            {

                DirectoryInfo info = new DirectoryInfo(dir);
                FileInfo[] files = info.GetFiles();
                foreach (FileInfo file in files)
                {
                    DateTime dt = file.LastWriteTime;
                    System.IO.File.Delete(file.FullName);

                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return Ok();
        }
    }
}