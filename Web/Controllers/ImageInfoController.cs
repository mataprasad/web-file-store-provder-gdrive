using FileUploadProvider;
using FileUploadProvider.LocalDirectory;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class ImageInfoController : Controller
    {
        private FileUploadTestEntities db = new FileUploadTestEntities();
        private IUploadProvider uploadProvider;

        public ImageInfoController(IUploadProvider uploadProvider)
        {
            this.uploadProvider = uploadProvider;
        }

        public ActionResult Index()
        {
            return View(db.LocalImageInfoes.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalImageInfo localImageInfo = db.LocalImageInfoes.Find(id);
            if (localImageInfo == null)
            {
                return HttpNotFound();
            }
            return View(localImageInfo);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Title,Description,Path,File")] ImageInfo imageInfo)
        {
            if (ModelState.IsValid)
            {
                LocalImageInfo localImageInfo = new LocalImageInfo()
                {
                    Description = imageInfo.Description,
                    Title = imageInfo.Title,
                    Provider = uploadProvider?.ProviderName
                };
                var info = uploadProvider?.Upload(imageInfo.File);
                localImageInfo.Path = info?.RealtiveUrl;
                db.LocalImageInfoes.Add(localImageInfo);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(imageInfo);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LocalImageInfo localImageInfo = db.LocalImageInfoes.Find(id);
            if (localImageInfo == null)
            {
                return HttpNotFound();
            }
            return View(localImageInfo);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LocalImageInfo localImageInfo = db.LocalImageInfoes.Find(id);
            db.LocalImageInfoes.Remove(localImageInfo);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
