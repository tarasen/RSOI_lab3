﻿using PictureAuction.SOA.Images.ServiceModel.Routes;
using ServiceStack.Common.Web;
using ServiceStack.ServiceInterface;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace PictureAuction.SOA.Images.ServiceInterface.Services
{
    public class ImageService : Service
    {
        [AddHeader(ContentType = "image/jpeg")]
        public object Get(ImageRoutes.GetImage request)
        {
            try
            {
                var img = Image.FromFile($"Images/{request.Name}.jpg");
                var ms = new MemoryStream();
                img.Save(ms, ImageFormat.Jpeg);
                return new HttpResult(ms, "image/jpeg");
            }
            catch (FileNotFoundException)
            {
                return HttpError.NotFound("Picture does not exist");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}