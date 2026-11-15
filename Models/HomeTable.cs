using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GarhwalBhawan.Models
{
    public class HomeTable
    {
        public List<BannerTbl> BannerObj { get; set; }
        public List<ImpLinkTbl> ImpLinkObj { get; set; }
        public List<SamacharTbl> NewsObj { get; set; }
        public List<TeamTbl> TeamObj { get; set; }
        public List<AdTbl> AdObj { get; set; }
    }
}