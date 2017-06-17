using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KrpanoCMS.Administration.Models
{
    public class TourModel
    {

        public Tour Tour { get; set; }

        public List<Panorama> PanoramaList { get; set; }

        public TourModel() { }
    }
}