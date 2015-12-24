using PictureAuction.SOA.Shared.DTOs;
using System.Collections.Generic;

namespace PictureAuction.SOA.Frontend.Models
{
    public class PictureModel : PicturesDTO.PictureExtendedDTO
    {
        public List<ArtistsDTO.ArtistDTO> ArtistsInfo { get; set; }
        public bool Authorized { get; set; }
    }
}