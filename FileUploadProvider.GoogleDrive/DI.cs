using Autofac;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using System;
using System.IO;

namespace FileUploadProvider.GoogleDrive
{
    public class DI : Module
    {
        public const string ApplicationName = "FileUploadProvider.GoogleDrive";
        static string[] Scopes = { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
        string gcreds = Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~/gcreds"));

        protected override void Load(ContainerBuilder builder)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", gcreds);
            builder.RegisterInstance<DriveService>(AuthorizeDriveService()).SingleInstance();
            builder.RegisterType<GDrive>().As<IUploadProvider>();
        }

        private DriveService AuthorizeDriveService()
        {
            var googleCredential = GoogleCredential.FromJson(File.ReadAllText(gcreds));
            var scopes = new string[] { DriveService.Scope.Drive, DriveService.Scope.DriveFile };
            googleCredential = googleCredential.CreateScoped(scopes);
            DriveService service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = googleCredential,
                ApplicationName = ApplicationName
            });
            service.HttpClient.Timeout = TimeSpan.FromMinutes(100);
            return service;
        }
    }
}
