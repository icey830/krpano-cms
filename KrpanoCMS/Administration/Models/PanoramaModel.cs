using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using KrpanoCMS.Administration.Utils;

namespace KrpanoCMS.Administration.Models
{
    public class PanoramaModel
    {
        [HttpPostedFileExtensions(Extensions = "jpg")]
        [DataType(DataType.Upload)]
        [DisplayName("Medium Picture: ")]
        public HttpPostedFileBase MediumPicture { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public DateTime AddedOn { get; set; }
        public string PictureUrl { get; set; }

        public PanoramaModel()
        {
            var panoramaEntity = new Panorama()
            {
                Id = Id,
                AddedOn = AddedOn,
                Name = Name,
                PictureUrl = PictureUrl,
                UserId = UserId
            };
        }
        public PanoramaModel(Panorama panoramaEntity)
        {

            Id = panoramaEntity.Id;
            AddedOn = panoramaEntity.AddedOn;
            Name = panoramaEntity.Name;
            PictureUrl = panoramaEntity.PictureUrl;
            UserId = panoramaEntity.UserId;
        }

        public Panorama MapToEntity()
        {
            var panoramaEntity = new Panorama()
            {
                Id = Id,
                UserId = UserId,
                AddedOn = AddedOn,
                Name = Name,
                PictureUrl = PictureUrl
            };

            return panoramaEntity;
        }

        public Panorama MapToEntity(Panorama panoramaEntity)
        {
            panoramaEntity.Id = Id;
            panoramaEntity.AddedOn = AddedOn;
            panoramaEntity.Name = Name;
            panoramaEntity.PictureUrl = PictureUrl;
            panoramaEntity.UserId = UserId;

            return panoramaEntity;
        }
    }
}