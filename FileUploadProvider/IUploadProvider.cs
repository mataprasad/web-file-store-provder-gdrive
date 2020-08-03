using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FileUploadProvider
{
    public interface IUploadProvider
    {
        string ProviderName { get; }
        UploadInfo Upload(HttpPostedFileBase file);
        UploadInfo GetFile(string provider,string storedPath);
    }
}