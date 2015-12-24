using AutoMapper;
using MoreLinq;
using PictureAuction.SOA.Pictures.ServiceModel.Routes;
using PictureAuction.SOA.Shared.DTOs;
using PictureAuction.SOA.Shared.ServiceModel;
using PictureAuction.SOA.Shared.ServiceModel.Types;
using ServiceStack.Common.Web;
using ServiceStack.OrmLite;
using ServiceStack.ServiceInterface;
using System;
using System.Globalization;
using System.Linq;
using System.Net;

namespace PictureAuction.SOA.Pictures.ServiceInterface.Services
{
    public class PictureService : Service
    {
        static PictureService()
        {
            Mapper.CreateMap<Picture, PicturesDTO.PictureExtendedDTO>()
                .ForMember(x => x.CreationDate, x => x.MapFrom(e => e.Creation.ToString("yyyy")));
            Mapper.CreateMap<Picture, PicturesDTO.PictureDTO>()
                .ForMember(x => x.CreationDate, x => x.MapFrom(e => e.Creation.ToString("yyyy")));
            Mapper.CreateMap<PicturesDTO.PictureExtendedDTO, Picture>()
                .ForMember(x => x.Creation,
                    x => x.MapFrom(e => DateTime.ParseExact(e.CreationDate, "yyyy", CultureInfo.CurrentCulture)));
        }

        public object Delete(PictureRoutes.DeletePicture request)
        {
            try
            {
                var deleted = Db.Delete<PicturesByGenre>(x => x.PictureId == request.Id) +
                              Db.Delete<PicturesByArtist>(x => x.PictureId == request.Id) +
                              Db.Delete<Picture>(x => x.Id == request.Id);

                return deleted == 0 ? HttpError.NotFound("") : new HttpError(HttpStatusCode.NoContent, "");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Get(PictureRoutes.GetPictures request)
        {
            try
            {
                var count = Db.GetScalar<Picture, int>(r => Sql.Count(r.Id));
                var skip = (request.PageNumber - 1)*request.PageSize;

                if (skip >= count)
                    return
                        new HttpResult(
                            new PageResult<PicturesDTO.PictureDTO>(new PicturesDTO.PictureDTO[0], 0, 1, count),
                            MimeTypes.Json);

                var pictures = Db.Select<Picture>(q => q.Limit(skip, request.PageSize));
                var minId = pictures.First().Id;
                var maxId = pictures.Last().Id;

                var pba =
                    Db.Select<PicturesByArtist>(p => p.PictureId >= minId && p.PictureId <= maxId)
                        .ToLookup(x => x.PictureId, x => x.ArtistId);

                var dto = new PageResult<PicturesDTO.PictureDTO>(
                    pictures.Select(p =>
                    {
                        var pictureDto = Mapper.Map<PicturesDTO.PictureDTO>(p);
                        pictureDto.Artists =
                            pba.Where(x => x.Key == p.Id)
                                .SelectMany(x => x)
                                .Select(x => new CustomEntity {Id = x})
                                .ToArray();
                        return pictureDto;
                    }).ToList(),
                    request.PageNumber, request.PageSize, count);

                return new HttpResult(dto, $"{MimeTypes.Json}; charset=utf-8");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Get(PictureRoutes.GetPicture request)
        {
            try
            {
                var picture = Db.GetByIdOrDefault<Picture>(request.Id);
                if (picture == null)
                    return HttpError.NotFound("Picture with this id is not found");

                var gal = Db.GetByIdOrDefault<Gallery>(picture.GalleryId);

                var pictureDto = Mapper.Map<PicturesDTO.PictureExtendedDTO>(picture);
                pictureDto.Material = Db.GetByIdOrDefault<Material>(picture.MaterialId)?.Name;
                pictureDto.Gallery = gal != null ? $"{gal.Name}, {gal.City}" : null;
                pictureDto.Technique = Db.GetByIdOrDefault<Technique>(picture.TechniqueId)?.Name;
                pictureDto.Image = picture.Id.ToString();

                pictureDto.Artists = Db.Select<PicturesByArtist>(x => x.PictureId == picture.Id)
                    .Join(Db.Select<Artist>(), pb => pb.ArtistId, a => a.Id, (_, a) => new CustomEntity {Id = a.Id})
                    .ToArray();
                pictureDto.Genres = Db.Select<PicturesByGenre>(x => x.PictureId == picture.Id)
                    .Join(Db.Select<Genre>(), pb => pb.GenreId, g => g.Id, (_, g) => g.Name).ToArray();

                return new HttpResult(pictureDto, $"{MimeTypes.Json}; charset=utf-8");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Post(PictureRoutes.CreatePicture request)
        {
            try
            {
                var pic = Mapper.Map<Picture>(request);

                var galInfo = request.Gallery.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);

                pic.MaterialId = Db.Select<Material>(x => x.Name == request.Material).First().Id;
                pic.GalleryId =
                    Db.Select<Gallery>(
                        x => x.Name == galInfo.ElementAtOrDefault(0) && x.City == galInfo.ElementAtOrDefault(1))
                        .FirstOrDefault()?
                        .Id;
                pic.TechniqueId = Db.Select<Technique>(x => x.Name == request.Technique).First().Id;

                Db.Insert(pic);
                var id = Db.GetScalar<Picture, int>(r => Sql.Max(r.Id));
                var picture = Db.GetById<Picture>(id);

                if (request.Artists?.Any() ?? false)
                {
                    var pba =
                        Db.Select<PicturesByArtist>()
                            .Join(request.Artists, x => x.ArtistId, a => a.Id, (x, _) => x)
                            .ToList();

                    Db.DeleteAll(pba);

                    Db.SaveAll(pba.Select(x => new PicturesByArtist {ArtistId = x.ArtistId, PictureId = picture.Id}));
                    Db.SaveAll(
                        request.Artists.Select(x => new PicturesByArtist {ArtistId = x.Id, PictureId = picture.Id})
                            .ExceptBy(pba, o => o.ArtistId)
                            .ToList());
                }
                if (request.Genres?.Any() ?? false)
                {
                    var abp = Db.Select<Genre>()
                        .Join(request.Genres, x => x.Name, x => x, (x, _) => x.Id)
                        .Join(Db.Select<PicturesByGenre>(), x => x, x => x.GenreId, (_, p) => p)
                        .ToList();

                    Db.DeleteAll(abp);
                    Db.SaveAll(abp.Select(x => new PicturesByGenre {GenreId = x.GenreId, PictureId = picture.Id}));
                }

                return new HttpResult(picture, $"{MimeTypes.Json}; charset=utf-8")
                {
                    StatusCode = HttpStatusCode.Created
                };
            }
            catch (InvalidOperationException)
            {
                return new HttpError(HttpStatusCode.BadRequest, "Wrong Parameters");
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }

        public object Put(PictureRoutes.UpdatePicture request)
        {
            try
            {
                var picture = Db.GetByIdOrDefault<Picture>(request.Id);
                if (picture == null)
                    return HttpError.NotFound($"Picture '{request.Id}' does not exist");

                Mapper.Map(request, picture, opts => opts.AfterMap((rq, pic) => pic.Id = rq.Id));

                var galInfo = request.Gallery.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries);

                picture.MaterialId = Db.Select<Material>(x => x.Name == request.Material).First().Id;
                picture.GalleryId =
                    Db.Select<Gallery>(
                        x => x.Name == galInfo.ElementAtOrDefault(0) && x.City == galInfo.ElementAtOrDefault(1))
                        .FirstOrDefault()?
                        .Id;
                picture.TechniqueId = Db.Select<Technique>(x => x.Name == request.Technique).First().Id;

                Db.Update(picture);

                if (request.Artists?.Any() ?? false)
                {
                    var pba =
                        Db.Select<PicturesByArtist>()
                            .Join(request.Artists, x => x.ArtistId, a => a.Id, (x, _) => x)
                            .ToList();

                    Db.DeleteAll(pba);

                    Db.SaveAll(pba.Select(x => new PicturesByArtist {ArtistId = x.ArtistId, PictureId = picture.Id}));
                    Db.SaveAll(
                        request.Artists.Select(x => new PicturesByArtist {ArtistId = x.Id, PictureId = picture.Id})
                            .ExceptBy(pba, o => o.ArtistId)
                            .ToList());
                }
                if (request.Genres?.Any() ?? false)
                {
                    var abp = Db.Select<Genre>()
                        .Join(request.Genres, x => x.Name, x => x, (x, _) => x.Id)
                        .Join(Db.Select<PicturesByGenre>(), x => x, x => x.GenreId, (_, p) => p)
                        .ToList();

                    Db.Delete(abp);
                    Db.SaveAll(abp.Select(x => new PicturesByGenre {GenreId = x.GenreId, PictureId = picture.Id}));
                }

                return new HttpResult(picture, $"{MimeTypes.Json}; charset=utf-8")
                {
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch
            {
                return new HttpError(HttpStatusCode.InternalServerError, "Internal Server Error");
            }
        }
    }
}