using System.Collections.Generic;

namespace KrpanoCMS.Administration.Models
{
    public class TourModel
    {

        public Tour Tour { get; set; }

        public List<int> PanoramaListId { get; set; }

        //public TourModel()
        //{
        //    var tourModelEntity = new TourModel()
        //    {
        //       Tour = new Tour(),
        //       PanoramaList = new List<Panorama>()
        //    };
        //}
    }
}