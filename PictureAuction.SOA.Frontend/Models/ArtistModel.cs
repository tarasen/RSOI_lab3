using PictureAuction.SOA.Shared.DTOs;
using System.Collections.Generic;

namespace PictureAuction.SOA.Frontend.Models
{
    public class ArtistModel : ArtistsDTO.ArtistExtendedDTO
    {
        public bool Authorized { get; set; }
        public List<PicturesDTO.PictureDTO> PicturesInfo { get; set; }
    }
}