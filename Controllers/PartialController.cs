using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GarhwalBhawan.Models;
using System.Configuration;
using System.Threading;

namespace GarhwalBhawan.Controllers
{
    public class PartialController : Controller
    {
        string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public PartialViewResult Index()
        {
            using (DBEntities Db=new DBEntities())
            {
                //Thread.Sleep(2000);
                List<SamacharTbl> News = Db.SamacharTbls.OrderByDescending(d=>d.Date).Take(5).ToList();
                return PartialView("NewsPartialView",News);
            }
        }


    }
}
