using AutoMapper;
using Nancy;
using Nancy.ModelBinding;
using Nancy.Responses.Negotiation;
using Nancy.Security;
using PictureAuction.SOA.Frontend.Models;
using PictureAuction.SOA.Frontend.Networks;
using PictureAuction.SOA.Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PictureAuction.SOA.Frontend.Modules
{
    public class PicturesModule : NancyModule, IDisposable
    {
        private readonly PictureAuctionClient _client = new PictureAuctionClient();

        static PicturesModule()
        {
            Mapper.CreateMap<PicturesDTO.PictureExtendedDTO, PictureModel>();
        }

        public PicturesModule() : base("/pictures")
        {
            Get["/id{id:int}", true] = GetPictureAsync;
            Get["/id{id:int}/edit", true] = EditPictureAsync;
            Get["/add"] = AddPicture;
            Post["/id{id:int}/edit", true] = UpdatePictureAsync;
            Get["/id{id:int}/delete", true] = DeletePictureAsync;
            Get["/", true] = GetPicturesAsync;
        }

        public void Dispose()
        {
            _client.Dispose();
        }

        private Negotiator AddPicture(object _)
        {
            this.RequiresAuthentication();

            return View["Views/Edit/picture.sshtml", new PicturesDTO.PictureExtendedDTO()];
        }

        private async Task<dynamic> CreatePictureAsync(PicturesDTO.PictureExtendedDTO picture)
        {
            var i = await _client.CreatePictureAsync(picture);
            if (i > 0)
                return Response.AsRedirect($"~/pictures/id{i}");
            return new Response {StatusCode = HttpStatusCode.BadRequest};
        }

        private async Task<dynamic> DeletePictureAsync(dynamic arg, CancellationToken _)
        {
            this.RequiresAuthentication();
            int id = Convert.ToInt32(arg.id.Value);

            if (await _client.DeletePictureAsync(id))
                return Response.AsRedirect("~/pictures");
            return new Response {StatusCode = HttpStatusCode.BadRequest};
        }

        private async Task<dynamic> EditPictureAsync(dynamic arg, CancellationToken _)
        {
            this.RequiresAuthentication();

            int id = Convert.ToInt32(arg.id.Value);
            var pictureDto = await _client.GetPictureAsync(id);
            if (pictureDto == null)
                return new Response {StatusCode = HttpStatusCode.NotFound};

            return View["Views/Edit/picture.sshtml", Mapper.Map<PictureModel>(pictureDto)];
        }

        private async Task<dynamic> GetPictureAsync(dynamic arg, CancellationToken _)
        {
            int id = Convert.ToInt32(arg.id.Value);
            var pictureDto = await _client.GetPictureAsync(id);
            if (pictureDto == null)
                return new Response {StatusCode = HttpStatusCode.NotFound};

            var task = await _client.GetPhotoAsync(pictureDto.Id + ".jpg");

            var pictureEx = Mapper.Map<PictureModel>(pictureDto);
            pictureEx.ArtistsInfo = new List<ArtistsDTO.ArtistDTO>();
            foreach (var artist in pictureDto.Artists)
            {
                var artistExtendedDto = await _client.GetArtistAsync(artist.Id);
                pictureEx.ArtistsInfo.Add(artistExtendedDto);
            }
            pictureEx.Image = task;
            pictureEx.Authorized = Context.CurrentUser != null;
            return View["Views/picture.sshtml", pictureEx];
        }

        private async Task<dynamic> GetPicturesAsync(dynamic arg, CancellationToken _)
        {
            DynamicDictionaryValue pageParam = Request.Query["page"];
            var page = pageParam.HasValue ? Convert.ToInt32(pageParam.Value) : 1;

            var items = await _client.GetPicturesAsync(page, 15);
            var collection = new PageResultModel<PicturesDTO.PictureDTO>(items)
            {
                Authorized = Context.CurrentUser != null
            };

            return View["Views/pictures.sshtml", collection];
        }

        private async Task<dynamic> UpdatePictureAsync(dynamic arg, CancellationToken _)
        {
            this.RequiresAuthentication();
            var picture = this.Bind<PicturesDTO.PictureExtendedDTO>();

            if (picture.Id == 0)
                return await CreatePictureAsync(picture);
            return await UpdatePictureAsync(picture);
        }

        private async Task<dynamic> UpdatePictureAsync(PicturesDTO.PictureExtendedDTO picture)
        {
            if (await _client.UpdatePictureAsync(picture))
                return Response.AsRedirect($"~/pictures/id{picture.Id}");
            return new Response {StatusCode = HttpStatusCode.BadRequest};
        }
    }
}