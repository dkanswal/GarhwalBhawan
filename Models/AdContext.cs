using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace GarhwalBhawan.Models
{
    public class AdContext:DbContext
    {
        public DbSet<AdDescTbl> AdDescDet { get; set; }
        public DbSet<AdImgTbl> AdImgDet { get; set; }
    }
}