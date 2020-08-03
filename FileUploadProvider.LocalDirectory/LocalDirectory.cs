using System;
using System.IO;
using System.Web;
using System.Web.Hosting;

namespace FileUploadProvider.LocalDirectory
{
    public class LocalDirectory : IUploadProvider
    {
        private Func<string, string> webUrlGenerator;
        public LocalDirectory(Func<string, string> webUrlGenerator)
        {
            this.webUrlGenerator = webUrlGenerator;
        }

        public string ProviderName => "LOCAL_DIRECTORY_UPLOAD_PROVIDER";

        public UploadInfo GetFile(string provider, string storedPath)
        {
            return new UploadInfo()
            {
                Name = Path.GetFileNameWithoutExtension(storedPath),
                Ext = Path.GetExtension(storedPath),
                WebUrl = webUrlGenerator?.Invoke(storedPath)
            };
        }

        public UploadInfo Upload(HttpPostedFileBase file)
        {
            var fileName = String.Concat(Guid.NewGuid().ToString().ToLower(), Path.GetExtension(file.FileName));
            var relativeUploadPath = String.Concat("~/Content/Uploads/", fileName.ToLower());
            var uploadPath = HostingEnvironment.MapPath(relativeUploadPath);
            file.SaveAs(uploadPath);
            return new UploadInfo()
            {
                Name = Path.GetFileNameWithoutExtension(uploadPath),
                Ext = Path.GetExtension(uploadPath),
                WebUrl = webUrlGenerator?.Invoke(relativeUploadPath),
                RealtiveUrl = relativeUploadPath
            };
        }
    }
}
