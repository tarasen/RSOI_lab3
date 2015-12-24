using PictureAuction.SOA.Shared.DTOs;
using PictureAuction.SOA.Shared.ServiceModel;
using ServiceStack.Common.Web;
using ServiceStack.ServiceHost;
using System.Runtime.Serialization;

namespace PictureAuction.SOA.Pictures.ServiceModel.Routes
{
    public static class PictureRoutes
    {
        [Route("/pictures", HttpMethods.Post)]
        public class CreatePicture : PicturesDTO.PictureExtendedDTO, IReturn<PicturesDTO.PictureExtendedDTO>
        {
        }

        [Route("/pictures/{Id}", HttpMethods.Delete)]
        public class DeletePicture : IReturnVoid
        {
            public int Id { get; set; }
        }

        [Route("/pictures/{Id}", HttpMethods.Get)]
        public class GetPicture : IReturn<PicturesDTO.PictureExtendedDTO>
        {
            public int Id { get; set; }
        }

        [Route("/pictures", HttpMethods.Get)]
        [DataContract]
        public class GetPictures : IReturn<PageResult<PicturesDTO.PictureDTO>>
        {
            [DataMember(Name = "page")]
            public int PageNumber { get; set; } = 1;

            [DataMember(Name = "page_size")]
            public int PageSize { get; set; } = 15;
        }

        [Route("/pictures/{Id}", HttpMethods.Put)]
        public class UpdatePicture : PicturesDTO.PictureExtendedDTO, IReturn<PicturesDTO.PictureExtendedDTO>
        {
        }
    }
}