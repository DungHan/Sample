using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;

namespace Sinyi.SuperAgent.Realties.Services
{
    public partial class FileController : Controller
    {
        // Fields
        private IWebHostEnvironment _webHostEnvironment = null;


        // Constructors
        public FileController(IWebHostEnvironment webHostEnvironment)
        {
            #region Contracts

            if (webHostEnvironment == null) throw new ArgumentException(nameof(webHostEnvironment));

            #endregion

            // Default
            _webHostEnvironment = webHostEnvironment;
        }
    }

    public partial class FileController : Controller
    {
        // Methods
        public ActionResult<UploadResultModel> Upload([FromForm] UploadActionModel actionModel)
        {
            #region Contracts

            if (actionModel == null) throw new ArgumentException(nameof(actionModel));

            #endregion

            // Requirement
            if (actionModel.Files.Count == 0) return NotFound();

            // Check dir
            if (!Directory.Exists($"{_webHostEnvironment.WebRootPath}/Upload"))
                Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}/Upload/");

            // Write
            foreach (var file in actionModel.Files)
            {
                using (var filestream = System.IO.File.Create($"{_webHostEnvironment.WebRootPath}/Upload/{file.FileName}"))
                {
                    file.CopyTo(filestream);
                    filestream.Flush();
                }
            }

            // Return
            return Ok();
        }


        // Class
        public class UploadActionModel
        {
            public List<IFormFile> Files { get; set; }
        }

        public class UploadResultModel
        {

        }
    }
}
