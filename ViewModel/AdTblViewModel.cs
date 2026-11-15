using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using GarhwalBhawan.Models;

namespace GarhwalBhawan.ViewModel
{
    public class AdTblViewModel
    {
        public IQueryable<AdDescTbl> AdDescObj { get; set; }
        public IQueryable<AdImgTbl> AdImgObj { get; set; }

        public AdTblViewModel(IQueryable<AdDescTbl> ad,IQueryable<AdImgTbl> ai)
        {
            this.AdDescObj = ad;
            this.AdImgObj = ai;
        }
    }
}