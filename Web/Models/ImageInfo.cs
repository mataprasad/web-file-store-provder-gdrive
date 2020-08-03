using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ImageInfo : LocalImageInfo
    {
        public HttpPostedFileBase File { get; set; }
    }
}