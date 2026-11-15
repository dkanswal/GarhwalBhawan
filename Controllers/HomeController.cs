using GarhwalBhawan.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using GarhwalBhawan.ViewModel;


namespace GarhwalBhawan.Controllers
{
    public class HomeController : Controller
    {
        DBEntities DbContext = new DBEntities();
        string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public ActionResult Index()
        {
            ViewBag.Title = "मुख्य पृष्ठ";

            List<BannerTbl> BTforGBList = new List<BannerTbl>();
            List<BannerTbl> BannerList = new List<BannerTbl>();
            List<ImpLinkTbl> ImpLinkList = new List<ImpLinkTbl>();
            List<SamacharTbl> NewsList = new List<SamacharTbl>();
            List<TeamTbl> TeamList = new List<TeamTbl>();
            List<AdTbl> AdList = new List<AdTbl>();

            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                SqlCommand SqlCmdBforGB = new SqlCommand("Select top 1 Heading,Description,Image,Alt from BannerTbl", con);
                SqlDataReader SqlDrBforGB = SqlCmdBforGB.ExecuteReader();
                BannerTbl BTforGB = new BannerTbl();
                while (SqlDrBforGB.Read())
                {
                    BTforGB.Heading = SqlDrBforGB["Heading"].ToString();
                    BTforGB.Description = SqlDrBforGB["Description"].ToString();
                    BTforGB.Image = SqlDrBforGB["Image"].ToString();
                    BTforGB.Alt = SqlDrBforGB["Alt"].ToString();
                    BTforGBList.Add(BTforGB);
                }
                SqlDrBforGB.Close();


                SqlCommand SqlCmdBanner = new SqlCommand("Select Heading,Description,Image,Alt from BannerTbl", con);
                SqlDataReader SqlDrBanner = SqlCmdBanner.ExecuteReader();

                while (SqlDrBanner.Read())
                {
                    BannerTbl BannerObj = new BannerTbl();
                    BannerObj.Heading = SqlDrBanner["Heading"].ToString();
                    BannerObj.Description = SqlDrBanner["Description"].ToString();
                    BannerObj.Image = SqlDrBanner["Image"].ToString();
                    BannerObj.Alt = SqlDrBanner["Alt"].ToString();
                    BannerList.Add(BannerObj);
                }
                BannerList.RemoveAt(0);
                SqlDrBanner.Close();


                SqlCommand SqlCmdImpLink = new SqlCommand("Select Id,Heading,Description from ImpLinkTbl", con);
                SqlDataReader SqlDrImpLink = SqlCmdImpLink.ExecuteReader();

                while (SqlDrImpLink.Read())
                {
                    ImpLinkTbl ImpLinkObj = new ImpLinkTbl();
                    ImpLinkObj.Heading = SqlDrImpLink["Heading"].ToString();
                    ImpLinkObj.Description = SqlDrImpLink["Description"].ToString();
                    ImpLinkList.Add(ImpLinkObj);
                }
                SqlDrImpLink.Close();

                SqlCommand SqlCmdNews = new SqlCommand("Select ID,Heading,Description,Date,Image,Alt from SamacharTbl", con);
                SqlDataReader SqlDrNews = SqlCmdNews.ExecuteReader();

                while (SqlDrNews.Read())
                {
                    SamacharTbl NewsObj = new SamacharTbl();
                    NewsObj.Heading = SqlDrNews["Heading"].ToString();
                    NewsObj.Description = SqlDrNews["Description"].ToString();
                    NewsObj.Date = Convert.ToDateTime(SqlDrNews["Date"]);
                    NewsObj.Image = SqlDrNews["Image"].ToString();
                    NewsObj.Alt = SqlDrNews["Alt"].ToString();
                    NewsList.Add(NewsObj);
                }
                SqlDrNews.Close();

                SqlCommand SqlCmdTeam = new SqlCommand("Select Id,Name,Description,Image,Alt from TeamTbl", con);
                SqlDataReader SqlDrTeam = SqlCmdTeam.ExecuteReader();

                while (SqlDrTeam.Read())
                {
                    TeamTbl TeamObj = new TeamTbl();
                    TeamObj.ID = Convert.ToInt32(SqlDrTeam["ID"]);
                    TeamObj.Name = SqlDrTeam["Name"].ToString();
                    TeamObj.Description = SqlDrTeam["Description"].ToString();
                    TeamObj.Image = SqlDrTeam["Image"].ToString();
                    TeamObj.Alt = SqlDrTeam["Alt"].ToString();
                    TeamList.Add(TeamObj);
                }
                SqlDrTeam.Close();

                SqlCommand SqlCmdAd = new SqlCommand("Select Id,StartDate,EndDate,Link,Image from AdTbl where Status='A' order by Amount desc ", con);
                SqlDataReader SqlDrAd = SqlCmdAd.ExecuteReader();

                while (SqlDrAd.Read())
                {
                    AdTbl AdObj = new AdTbl();
                    AdObj.ID = Convert.ToInt32(SqlDrAd["ID"]);
                    AdObj.StartDate = Convert.ToDateTime(SqlDrAd["StartDate"]);
                    AdObj.EndDate = Convert.ToDateTime(SqlDrAd["EndDate"]);
                    AdObj.Link = SqlDrAd["Link"].ToString();
                    AdObj.Image = SqlDrAd["Image"].ToString();
                    AdList.Add(AdObj);
                }
                SqlDrAd.Close();

            }

            HomeTable HomeTblCl = new HomeTable();
            HomeTblCl.BTforGB = BTforGBList;
            HomeTblCl.BannerObj = BannerList;
            HomeTblCl.ImpLinkObj = ImpLinkList;
            HomeTblCl.NewsObj = NewsList;
            HomeTblCl.TeamObj = TeamList;
            HomeTblCl.AdObj = AdList;

            return View(HomeTblCl);
        }

        public ActionResult EkParichay()
        {
            ViewBag.Title = "एक परिचय";

            return View(DbContext.EkParichays.ToList());
        }

        public ActionResult Purpose()
        {
            ViewBag.Title = "उद्देश्य";

            return View();
        }



        //present-executive Starts
        [ActionName("present-executive")]
        public ActionResult executive()
        {
            ViewBag.Title = "वर्त्तमान कार्यकारिणी";

            //return View(DbContext.MemberTbls.OrderBy(x => x.Rank).ToList());

            IEnumerable<MemberTbl> mt = from a in DbContext.MemberTbls
                                        orderby a.Rank
                                        select a;

            IEnumerable<NomMember> nm = from k in DbContext.NomMembers
                                        orderby k.Rank
                                        select k;

            MemberTblVM model = new MemberTblVM(mt, nm);
            return View(model);

        }
        //present-executive Ends


        [ActionName("house-of-the-constitution")]
        public ActionResult constitution()
        {
            ViewBag.Title = "सभा का सविधान";

            return View();
        }


        public FileResult DownloadFile(int id)
        {
            string Path = "";

            var result = (from db in DbContext.DocTbls
                          where db.Id == id
                          select db.DocPath).FirstOrDefault();

            Path = result.ToString();

            return File(Path, "application/pdf");

        }

        [ActionName("garhwal-bhawan-history")]
        public ActionResult history()
        {
            ViewBag.Title = "गढ़वाल भवन - इतिहास";
            return View();
        }

        [ActionName("support-uttarakhand-disaster")]
        public ActionResult Disaster()
        {
            ViewBag.Title = "प्राकृतिक आपदा सहयोग";
            return View(DbContext.DisasterTbls.ToList());
        }

        [HttpGet]
        [ActionName("Contact")]
        public ActionResult Contact_Get()
        {
            ViewBag.Title = "संपर्क करें";
            return View();
        }

        [HttpPost]
        [ActionName("Contact")]
        public ActionResult Contact_Post(ContactTbl Contact)
        {
            ViewBag.Title = "संपर्क करें";

            if (ModelState.IsValid)
            {
                Contact.Date = DateTime.Now;
                DbContext.ContactTbls.Add(Contact);
                DbContext.SaveChanges();
                ViewBag.Message = "Message Sent Successfully";
                return View();
            }
            else
            {
                return View();
            }
        }

        //PhotoGallery Starts
        public ActionResult PhotoGallery()
        {
            ViewBag.Title = "फोटो गैलरी";
            return View();
        }
        //PhotoGallery Ends

        //BhawanHistory Starts
        public ActionResult BhawanHistory()
        {
            ViewBag.Title = "भवन का इतिहास";
            return View(DbContext.ItihasTbls.ToList());
        }
        //BhawanHistory Ends

        public JsonResult getImage() //JSON Collection
        {
            var resultgetImage = (from r in DbContext.ImageTbls
                                  select new { r.ImgGroup }).Distinct().ToList();
            return Json(resultgetImage, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getImageYear(string grp)
        {
            var result = (from r in DbContext.ImageTbls
                          where r.ImgGroup == grp
                          select new { r.Year }).Distinct().ToList();
            return Json(result);
        }

        [HttpPost]
        public JsonResult getImageClick(string grp, string grpYr)
        {
            Thread.Sleep(3000);
            var result = (from r in DbContext.ImageTbls
                          where r.ImgGroup == grp && r.Year == grpYr
                          select new { r.ID, r.Image, r.Description, r.Alt }).ToList();
            return Json(result);
        }
        //PhotoGallery Ends

        //VideoGallery Starts
        public ActionResult VideoGallery()
        {
            ViewBag.Title = "video गैलरी";
            return View();
        }


        public JsonResult getVideo() //JSON Collection
        {
            var resultgetVideo = (from r in DbContext.VideoTbls
                                  select new { r.VideoGroup }).Distinct().ToList();
            return Json(resultgetVideo, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getVideoYear(string grp)
        {
            var result = (from r in DbContext.VideoTbls
                          where r.VideoGroup == grp
                          select new { r.Year }).Distinct().ToList();
            return Json(result);
        }

        [HttpPost]
        public JsonResult getVideoClick(string grp, string grpYr)
        {
            Thread.Sleep(2000);
            var result = (from r in DbContext.VideoTbls
                          where r.VideoGroup == grp && r.Year == grpYr
                          select new { r.Video, r.Description, r.Alt }).Distinct().ToList();
            return Json(result);
        }
        //VideoGallery Ends

        //Sandesh Starts
        [HttpGet]
        public PartialViewResult Sandesh(int id)
        {
            return PartialView(DbContext.TeamTbls.Where(x => x.ID == id).FirstOrDefault());
        }
        //Sandesh Ends

        //HallDetail Starts
        [HttpGet]
        public ViewResult Hall(string id)
        {
            return View(id);
        }
        //Hall Details Ends

        //AdDetail Starts
        [HttpGet]
        public ActionResult AdDetail(int id)
        {
            IQueryable<AdDescTbl> j = from a in DbContext.AdDescTbls
                                      where a.AdID == id
                                      select a;

            IQueryable<AdImgTbl> l = from k in DbContext.AdImgTbls
                                     where k.AdID == id
                                     select k;

            AdTblViewModel model = new AdTblViewModel(j, l);
            return View(model);

            //return View(DbContext.AdImgTbls.Where(x => x.AdID == id).ToList());
        }
        //AdDetail Ends

        //AdImage Details Starts
        public ViewResult AdImageDetail(int id)
        {
            ViewBag.Title = "Image Details";
            return View(DbContext.AdTbls.Where(x => x.ID == id).FirstOrDefault());
        }
        //AdImage Details Ends

        //GarhwalHistory Starts
        public ActionResult GarhwalHistory()
        {
            ViewBag.Title = "गढ़वाल का इतिहास";
            return View();
        }
        //GarhwalHistory Ends

        //Sanskriti Starts
        public ViewResult Sanskriti()
        {
            ViewBag.Title = "उत्तराखण्ड की संस्कृति";
            return View();
        }
        //Sanskriti Ends

        //FruitsAndVeg Starts
        public ViewResult FruitsAndVeg()
        {
            ViewBag.Title = "फल व सब्जियाँ";
            return View(DbContext.FruitVegTbls.ToList());
        }
        //FruitsAndVeg Ends

        //Uplabdhiyan Starts
        public ViewResult Uplabdhiyan()
        {
            ViewBag.Title = "उपलब्धियाँ व भविष्य की योजनाए";
            return View();
        }
        //Uplabdhiyan Ends

        //samajik-sahyog Starts
        [ActionName("samajik-sahyog")]
        public ViewResult sahyog()
        {
            ViewBag.Title = "सामाजिक सहयोग";
            return View();
        }
        //samajik-sahyog Ends

        //Uplabdhiyan Starts
        public ViewResult Vidyarthi()
        {
            ViewBag.Title = "विद्यार्थियों को प्रोत्साहन";
            return View();
        }
        //Uplabdhiyan Ends

        //Donation Starts
        public ViewResult DonateUs()
        {
            ViewBag.Title = "डोनेट करें";
            return View();
        }
        //Donation Ends


        //Search Image Starts
        [HttpGet]
        public ActionResult getNewsSearch()
        {
            List<SamacharTbl> NewsData = DbContext.SamacharTbls.OrderByDescending(x => x.Date).ToList<SamacharTbl>();
            return View("NewsSearch", NewsData);
        }

        [HttpPost]
        public JsonResult getNewsSearch(string searchTxt)
        {
            Thread.Sleep(2000);

            var result = (from r in DbContext.SamacharTbls
                          where r.Description.Contains(searchTxt) || r.Heading.Contains(searchTxt)
                          select new { r.ID, r.Image, r.Description, r.Heading, r.Alt, r.Date }).OrderByDescending(r => r.Date).ToList();
            return Json(result);

        }

        [HttpGet]
        public PartialViewResult NewsPartial(int id)
        {
            return PartialView(DbContext.SamacharTbls.Where(x => x.ID == id).FirstOrDefault());
        }
        //Search Image Ends

        //Ad End Date Check Starts
        [HttpPost]
        public JsonResult EndDateCheck()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();
                SqlCommand SqlCmdEndDateCheck = new SqlCommand("EndDateCheck", con);
                SqlCmdEndDateCheck.CommandType = CommandType.StoredProcedure;
                int _count = SqlCmdEndDateCheck.ExecuteNonQuery();
                if (_count > 0)
                {
                    return Json("Success");
                }
                else
                {
                    return Json("Something Went Wrong. Contact Support");
                }
            }

        }

        public ActionResult TechTeam()
        {
            return View();
        }

        //Ad End Date Check Ends
    }
}
