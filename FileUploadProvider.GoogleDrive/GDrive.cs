using Google.Apis.Drive.v3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace FileUploadProvider.GoogleDrive
{
    public class GDrive : IUploadProvider
    {
        private DriveService driveService;
        public GDrive(DriveService driveService)
        {
            this.driveService = driveService;
        }

        public string ProviderName => DI.ApplicationName;

        public UploadInfo GetFile(string provider, string storedPath)
        {
            return new UploadInfo()
            {
                Name = Path.GetFileNameWithoutExtension(storedPath),
                Ext = Path.GetExtension(storedPath),
                WebUrl = storedPath
            };
        }

        public UploadInfo Upload(HttpPostedFileBase file)
        {
            Google.Apis.Drive.v3.Data.File body = new Google.Apis.Drive.v3.Data.File();
            body.Name = Path.GetFileName(file.FileName);
            body.Description = file.FileName;
            body.MimeType = file.ContentType;
            body.Parents = new List<String>() { "1sAcTWk6hydJPQjNatQZFvBot_WZrlHlJ" };
            FilesResource.CreateMediaUpload request = this.driveService.Files.Create(body, file.InputStream, body.MimeType);
            request.SupportsAllDrives = true;
            request.Upload();
            var response = request.ResponseBody;
            var storedPath = String.Concat("https://drive.google.com/uc?export=view&id=", response.Id);
            return new UploadInfo()
            {
                Name = Path.GetFileNameWithoutExtension(body.Name),
                Ext = Path.GetExtension(body.Name),
                WebUrl = storedPath,
                RealtiveUrl = storedPath
            };
        }
    }
}
