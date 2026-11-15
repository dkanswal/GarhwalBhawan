using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using GarhwalBhawan.Models;
using GarhwalBhawan.ViewModel;
using System.Threading;
using System.IO;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Web.UI.WebControls;
using System.Data.Linq.SqlClient;
using System.Data.Objects;

namespace GarhwalBhawan.Controllers
{
    public class AdminController : Controller
    {
        private DBEntities db = new DBEntities();
        string myMessage = "";

        //Index Starts
        [LogInFilter]
        public ActionResult Index()
        {
            if (Session["UserName"] == null || Session["UserName"].ToString() == "")
            {
                return Redirect("~/Admin/LogIn");
            }
            else
            {
                return View(db.AdminTbls.ToList());
                //return Content("Welcome " + Session["UserName"]);
            }
        }
        //Index Ends

        //Login View Starts
        [HttpGet]
        public ActionResult LogIn()
        {
            return View("LogIn");
        }
        //Login View Ends

        //Login Access Details Starts
        [HttpPost]
        public ActionResult LogIn(Models.AdminTbl Userr)
        {
            if (IsValid(Userr.AdminUserName, Userr.AdminPassword))
            {
                FormsAuthentication.SetAuthCookie(Userr.AdminUserName, false);
                Session["UserName"] = Userr.AdminUserName;
                Session.Timeout = 30;
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                ModelState.AddModelError("", "UserName or Password is Wrong");
            }
            return View(Userr);
        }
        //Login Access Details Ends

        //LogOut Starts
        public ActionResult LogOut()
        {
            Session.Remove("UserName");
            Session.Timeout = 30;
            FormsAuthentication.SignOut();
            return RedirectToAction("LogIn", "Admin");
        }
        //LogOut Ends

        //Admin Details Starts
        private bool IsValid(string UserName, string Password)
        {
            bool IsValid = false;

            using (var db = new GarhwalBhawan.Models.DBEntities())
            {
                var user = db.AdminTbls.FirstOrDefault(u => u.AdminUserName == UserName);

                if (user != null)
                {
                    if (user.AdminPassword == Password)
                    {
                        IsValid = true;
                    }
                }
            }

            return IsValid;
        }
        //Admin Details Ends

        //MemberTbl Starts
        [LogInFilter]
        public ActionResult MemberView()
        {
            List<MemberTbl> MemberData = db.MemberTbls.OrderBy(x => x.Rank).ToList<MemberTbl>();
            return View(MemberData);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult MemberCreate()   //Insert PartialView  
        {
            return PartialView(new GarhwalBhawan.Models.MemberTbl());
        }


        [HttpPost]
        [LogInFilter]
        public JsonResult MemberCreate(GarhwalBhawan.Models.MemberTbl Mem, HttpPostedFileBase file) // Record Insert  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/Cabinet/") + FileString);
                        Mem.Image = FileString;
                    }

                    db.MemberTbls.Add(Mem);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);

                }

                else
                {
                    return Json(new { success = false, responseText = "Can't Save. Contact Support!" }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return Json(myMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult MemberEdit(Int32 MemID)  // Update PartialView  
        {
            MemberTbl Mem = db.MemberTbls.Where(x => x.ID == MemID).FirstOrDefault();
            MemberTbl MemInfo = new MemberTbl();

            MemInfo.ID = Mem.ID;
            MemInfo.Name = Mem.Name;
            MemInfo.Image = Mem.Image;
            MemInfo.Post = Mem.Post;
            MemInfo.Rank = Mem.Rank;

            return PartialView(MemInfo);

        }

        [HttpPost]
        [LogInFilter]
        public JsonResult MemberEdit(GarhwalBhawan.Models.MemberTbl MemID, HttpPostedFileBase file)  // Record Update 
        {

            if (ModelState.IsValid)
            {
                MemberTbl MemDt = db.MemberTbls.Where(x => x.ID == MemID.ID).FirstOrDefault();

                if (file != null)
                {
                    MemberTbl MyMemID = db.MemberTbls.Where(x => x.ID == MemID.ID).FirstOrDefault();

                    string completFileName = HttpContext.Server.MapPath("~/Image/Cabinet/" + MyMemID.Image);
                    if (System.IO.File.Exists(completFileName))
                    {
                        System.IO.File.Delete(completFileName);

                    }

                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Image/Cabinet/") + fileName);
                    MemID.Image = fileName;

                    MemDt.Image = MemID.Image;

                }

                MemDt.ID = MemID.ID;
                MemDt.Name = MemID.Name;
                MemDt.Post = MemID.Post;
                MemDt.Rank = MemID.Rank;
                db.SaveChanges();
                return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);


            }

            else
            {
                return Json(new { success = false, responseText = "Cannot Save! Fill Details or Contact Support." }, JsonRequestBehavior.AllowGet);
            }
        }

        [LogInFilter]
        public JsonResult MemberDelete(Int32 MemID)
        {
            MemberTbl Mem = db.MemberTbls.Where(x => x.ID == MemID).FirstOrDefault();
            db.MemberTbls.Remove(Mem);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }

        //MemberTbl Ends

        //ContactTbl Starts
        [LogInFilter]
        public ActionResult ContactView()
        {
            List<ContactTbl> ContactData = db.ContactTbls.OrderBy(x => x.ID).ToList<ContactTbl>();
            return View(ContactData);
        }

        [LogInFilter]
        public JsonResult ContactDelete(Int32 CntctID)
        {
            ContactTbl Contact = db.ContactTbls.Where(x => x.ID == CntctID).FirstOrDefault();
            db.ContactTbls.Remove(Contact);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //ContactTbl Ends


        //VideoTbl Starts
        [LogInFilter]
        public ActionResult VideoView()
        {
            List<VideoTbl> VideoData = db.VideoTbls.OrderByDescending(x => x.Date).ToList<VideoTbl>();
            return View(VideoData);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult VideoCreate()   //Insert PartialView  
        {
            return PartialView(new GarhwalBhawan.Models.VideoTbl());
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult VideoCreate(GarhwalBhawan.Models.VideoTbl VidObj) // Record Insert  
        {
            if (ModelState.IsValid)
            {
                db.VideoTbls.Add(VidObj);
                db.SaveChanges();
                myMessage = "Success";
            }

            else
            {
                myMessage = "Failed";
            }
            return Json(myMessage, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult VideoEdit(Int32 VidID)  // Update PartialView  
        {
            VideoTbl Vid = db.VideoTbls.Where(x => x.ID == VidID).FirstOrDefault();
            VideoTbl VidInfo = new VideoTbl();

            VidInfo.ID = Vid.ID;
            VidInfo.Description = Vid.Description;
            VidInfo.Video = Vid.Video;
            VidInfo.Alt = Vid.Alt;
            VidInfo.Year = Vid.Year;
            VidInfo.VideoGroup = Vid.VideoGroup;

            return PartialView(VidInfo);
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult VideoEdit(GarhwalBhawan.Models.VideoTbl VidID)  // Record Update 
        {

            VideoTbl VidDt = db.VideoTbls.Where(x => x.ID == VidID.ID).FirstOrDefault();

            if (ModelState.IsValid)
            {
                VidDt.ID = VidID.ID;
                VidDt.Description = VidID.Description;
                VidDt.Video = VidID.Video;
                VidDt.Alt = VidID.Alt;
                VidDt.Year = VidID.Year;
                VidDt.VideoGroup = VidID.VideoGroup;
                db.SaveChanges();
                myMessage = "Success";
            }

            else
            {
                myMessage = "Failed";
            }
            return Json(myMessage, JsonRequestBehavior.AllowGet);
        }

        [LogInFilter]
        public JsonResult VideoDelete(Int32 VidID)
        {
            VideoTbl Vid = db.VideoTbls.Where(x => x.ID == VidID).FirstOrDefault();
            db.VideoTbls.Remove(Vid);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //VideoTbl Ends


        //ImageTbl Starts
        [LogInFilter]
        public ActionResult ImageView()
        {
            List<ImageTbl> ImageData = db.ImageTbls.OrderByDescending(x => x.Date).ToList<ImageTbl>();
            return View(ImageData);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult ImageCreate()   //Insert PartialView  
        {
            return PartialView(new GarhwalBhawan.Models.ImageTbl());
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult ImageCreate(GarhwalBhawan.Models.ImageTbl ImgObj, HttpPostedFileBase file) // Record Insert  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/") + FileString);
                        ImgObj.Image = FileString;
                    }

                    db.ImageTbls.Add(ImgObj);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { success = false, responseText = "Can't Save. Contact Support!" }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult ImageEdit(Int32 ImgID)  // Update PartialView  
        {
            ImageTbl Img = db.ImageTbls.Where(x => x.ID == ImgID).FirstOrDefault();
            ImageTbl ImgInfo = new ImageTbl();

            ImgInfo.ID = Img.ID;
            ImgInfo.Description = Img.Description;
            ImgInfo.Image = Img.Image;
            ImgInfo.Alt = Img.Alt;
            ImgInfo.Year = Img.Year;
            ImgInfo.ImgGroup = Img.ImgGroup;

            return PartialView(ImgInfo);
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult ImageEdit(GarhwalBhawan.Models.ImageTbl ImgID, HttpPostedFileBase file)  // Record Update 
        {
            if (ModelState.IsValid)
            {
                ImageTbl ImgDt = db.ImageTbls.Where(x => x.ID == ImgID.ID).FirstOrDefault();

                if (file != null)
                {
                    ImageTbl MyImgID = db.ImageTbls.Where(x => x.ID == ImgID.ID).FirstOrDefault();

                    string completFileName = HttpContext.Server.MapPath("~/Image/" + MyImgID.Image);
                    if (System.IO.File.Exists(completFileName))
                    {
                        System.IO.File.Delete(completFileName);

                    }

                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Image/") + fileName);
                    ImgID.Image = fileName;

                    ImgDt.ID = ImgID.ID;
                    ImgDt.Description = ImgID.Description;
                    ImgDt.Image = ImgID.Image;
                    ImgDt.Alt = ImgID.Alt;
                    ImgDt.Year = ImgID.Year;
                    ImgDt.ImgGroup = ImgID.ImgGroup;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    ImgDt.ID = ImgID.ID;
                    ImgDt.Description = ImgID.Description;
                    ImgDt.Alt = ImgID.Alt;
                    ImgDt.Year = ImgID.Year;
                    ImgDt.ImgGroup = ImgID.ImgGroup;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                }

            }

            else
            {
                return Json(new { success = false, responseText = "Cannot Save! Fill Details or Contact Support." }, JsonRequestBehavior.AllowGet);
            }

        }

        [LogInFilter]
        public JsonResult ImageDelete(Int32 ImgID)
        {
            ImageTbl Img = db.ImageTbls.Where(x => x.ID == ImgID).FirstOrDefault();

            if (Img.Image != "")
            {
                string completFileName = HttpContext.Server.MapPath("~/Image/" + Img.Image);
                if (System.IO.File.Exists(completFileName))
                {
                    System.IO.File.Delete(completFileName);

                }
            }
            db.ImageTbls.Remove(Img);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //ImageTbl Ends

        //TeamTbl Starts
        [LogInFilter]
        public ActionResult TeamView()
        {
            List<TeamTbl> TeamData = db.TeamTbls.OrderBy(x => x.Rank).ToList<TeamTbl>();
            return View(TeamData);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult TeamCreate()   //Insert PartialView  
        {
            return PartialView(new GarhwalBhawan.Models.TeamTbl());
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult TeamCreate(GarhwalBhawan.Models.TeamTbl TmObj, HttpPostedFileBase file) // Record Insert  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/Cabinet/") + FileString);
                        TmObj.Image = FileString;
                    }

                    db.TeamTbls.Add(TmObj);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { success = false, responseText = "Can't Save. Contact Support!" }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult TeamEdit(Int32 TmID)  // Update PartialView  
        {
            TeamTbl Tm = db.TeamTbls.Where(x => x.ID == TmID).FirstOrDefault();
            TeamTbl TmInfo = new TeamTbl();

            TmInfo.ID = Tm.ID;
            TmInfo.Name = Tm.Name;
            TmInfo.Description = Tm.Description;
            TmInfo.Image = Tm.Image;
            TmInfo.Alt = Tm.Alt;
            TmInfo.Rank = Tm.Rank;

            return PartialView(TmInfo);
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult TeamEdit(GarhwalBhawan.Models.TeamTbl TmID, HttpPostedFileBase file)  // Record Update 
        {
            if (ModelState.IsValid)
            {
                TeamTbl TmDt = db.TeamTbls.Where(x => x.ID == TmID.ID).FirstOrDefault();

                if (file != null)
                {
                    TeamTbl MyTmID = db.TeamTbls.Where(x => x.ID == TmID.ID).FirstOrDefault();

                    string completFileName = HttpContext.Server.MapPath("~/Image/Cabinet/" + MyTmID.Image);
                    if (System.IO.File.Exists(completFileName))
                    {
                        System.IO.File.Delete(completFileName);

                    }

                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Image/Cabinet/") + fileName);
                    TmID.Image = fileName;
                   
                    TmDt.Image = TmID.Image;
                   
                }

                    TmDt.ID = TmID.ID;
                    TmDt.Name = TmID.Name;
                    TmDt.Description = TmID.Description;
                    TmDt.Alt = TmID.Alt;
                    TmDt.Rank = TmID.Rank;
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                

            }

            else
            {
                return Json(new { success = false, responseText = "Cannot Save! Fill Details or Contact Support." }, JsonRequestBehavior.AllowGet);
            }

        }

        [LogInFilter]
        public JsonResult TeamDelete(Int32 TmID)
        {
            TeamTbl Tm = db.TeamTbls.Where(x => x.ID == TmID).FirstOrDefault();

            string completFileName = HttpContext.Server.MapPath("~/Image/Cabinet/" + Tm.Image);
            if (System.IO.File.Exists(completFileName))
            {
                System.IO.File.Delete(completFileName);

            }

            db.TeamTbls.Remove(Tm);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //TeamTbl Ends

        //Multiple Image Upload Starts
        [LogInFilter]
        [HttpGet]
        public ViewResult MultiImageUpload()
        {
            return View();
        }

        [LogInFilter]
        [HttpPost]
        public ActionResult MultiImageUpload(GarhwalBhawan.Models.ImageTbl ImgObj, IEnumerable<HttpPostedFileBase> files)
        {
            int _count = 1;
            foreach (var file in files)
            {
                // if (file.ContentLength > 0)
                {
                    if (file != null)
                    {
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString()+Convert.ToString(_count) + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/") + FileString);
                        ImgObj.Image = FileString;

                        ImgObj.Date = DateTime.Now;
                        ImgObj.Description = "Not Defined";
                        ImgObj.Alt = "Not Defined";

                        db.ImageTbls.Add(ImgObj);
                        db.SaveChanges();
                        _count++;
                    }
                }
            }
            return RedirectToAction("ImageView");

        }
        //Multiple Image Upload Ends

        //AdTbl Starts
        [LogInFilter]
        public ActionResult AdView()
        {
            List<AdTbl> AdData = db.AdTbls.OrderByDescending(x => x.Amount).ToList<AdTbl>();
            return View(AdData);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult AdCreate()   //Insert PartialView  
        {
            return PartialView(new GarhwalBhawan.Models.AdTbl());
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult AdCreate(GarhwalBhawan.Models.AdTbl AdObj, HttpPostedFileBase file) // Record Insert  
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (file != null)
                    {
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/AdImg/") + FileString);
                        AdObj.Image = FileString;
                    }
                    AdObj.Status = "A";
                    db.AdTbls.Add(AdObj);
                    db.SaveChanges();
                    return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);
                }

                else
                {
                    return Json(new { success = false, responseText = "Can't Save. Contact Support!" }, JsonRequestBehavior.AllowGet);
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [LogInFilter]
        public PartialViewResult AdEdit(Int32 AdID)  // Update PartialView  
        {
            AdTbl Ad = db.AdTbls.Where(x => x.ID == AdID).FirstOrDefault();
            AdTbl AdInfo = new AdTbl();

            AdInfo.ID = Ad.ID;
            AdInfo.Name = Ad.Name;
            AdInfo.StartDate = Ad.StartDate;
            AdInfo.EndDate = Ad.EndDate;
            AdInfo.Link = Ad.Link;
            AdInfo.Image = Ad.Image;
            AdInfo.Amount = Ad.Amount;

            return PartialView(AdInfo);
        }

        [HttpPost]
        [LogInFilter]
        public JsonResult AdEdit(GarhwalBhawan.Models.AdTbl AdID, HttpPostedFileBase file)  // Record Update 
        {
            if (ModelState.IsValid)
            {
                AdTbl AdDt = db.AdTbls.Where(x => x.ID == AdID.ID).FirstOrDefault();

                if (file != null)
                {
                    AdTbl MyAdID = db.AdTbls.Where(x => x.ID == AdID.ID).FirstOrDefault();

                    string completFileName = HttpContext.Server.MapPath("~/Image/AdImg/" + MyAdID.Image);
                    if (System.IO.File.Exists(completFileName))
                    {
                        System.IO.File.Delete(completFileName);

                    }

                    var fileName = Path.GetFileName(file.FileName);
                    file.SaveAs(HttpContext.Server.MapPath("~/Image/AdImg/") + fileName);
                    AdID.Image = fileName;

                    AdDt.Image = AdID.Image;
                }
                AdDt.ID = AdID.ID;
                AdDt.Name = AdID.Name;
                AdDt.StartDate = AdID.StartDate;
                AdDt.EndDate = AdID.EndDate;
                AdDt.Amount = AdID.Amount;
                AdDt.Link = AdID.Link;
                db.SaveChanges();
                return Json(new { success = true, responseText = "Saved Successfully!" }, JsonRequestBehavior.AllowGet);

            }

            else
            {
                return Json(new { success = false, responseText = "Cannot Save! Fill Details or Contact Support." }, JsonRequestBehavior.AllowGet);
            }

        }

        [LogInFilter]
        public JsonResult AdDelete(Int32 AdID)
        {
            AdTbl Ad = db.AdTbls.Where(x => x.ID == AdID).FirstOrDefault();

            string completFileName = HttpContext.Server.MapPath("~/Image/AdImg/" + Ad.Image);
            if (System.IO.File.Exists(completFileName))
            {
                System.IO.File.Delete(completFileName);

            }

            db.AdTbls.Remove(Ad);
            db.SaveChanges();
            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //AdTbl Ends

        //BannerTbl Starts
        [LogInFilter]
        public ActionResult BannerView()
        {
            return View(db.BannerTbls.ToList());
        }

        [LogInFilter]
        [HttpGet]
        public ActionResult BannerCreate()
        {
            return View();
        }

        [LogInFilter]
        [HttpPost]
        public ActionResult BannerCreate(BannerTbl Bnr, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string FileExtn = System.IO.Path.GetExtension(file.FileName);
                    string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                    file.SaveAs(HttpContext.Server.MapPath("~/Image/") + FileString);
                    Bnr.Image = FileString;
                }

                db.BannerTbls.Add(Bnr);
                db.SaveChanges();

                return RedirectToAction("BannerView");
            }

            return View();
        }

        [HttpGet]
        [LogInFilter]
        public ActionResult BannerEdit(int id = 0)
        {
            BannerTbl BannerTbl = db.BannerTbls.Find(id);
            if (BannerTbl == null)
            {
                return HttpNotFound();
            }


            return View(BannerTbl);
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult BannerEdit(BannerTbl BnrID, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    BannerTbl BnrDt = db.BannerTbls.Where(x => x.ID == BnrID.ID).FirstOrDefault();

                    if (file != null)
                    {
                        BannerTbl MyBnrID = db.BannerTbls.Where(x => x.ID == BnrID.ID).FirstOrDefault();

                        string completFileName = HttpContext.Server.MapPath("~/Image/" + MyBnrID.Image);
                        if (System.IO.File.Exists(completFileName))
                        {
                            System.IO.File.Delete(completFileName);

                        }
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/") + FileString);
                        BnrID.Image = FileString;

                        BnrDt.Heading = BnrID.Heading;
                        BnrDt.Description = BnrID.Description;
                        BnrDt.Image = BnrID.Image;
                        BnrDt.Alt = BnrID.Alt;
                        db.SaveChanges();
                        return RedirectToAction("BannerView");
                    }

                    else
                    {
                        BnrDt.Heading = BnrID.Heading;
                        BnrDt.Description = BnrID.Description;
                        BnrDt.Alt = BnrID.Alt;
                        db.SaveChanges();
                        return RedirectToAction("BannerView");
                    }

                }

                else
                {
                    return RedirectToAction("BannerEdit");
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult BannerDelete(BannerTbl BnrID)
        {
            BannerTbl BnrImgID = db.BannerTbls.Where(x => x.ID == BnrID.ID).FirstOrDefault();
            string ImgVal = Convert.ToString(BnrImgID.Image);

            string completFileName = HttpContext.Server.MapPath("~/Images/" + ImgVal);

            if (BnrImgID.Image != null || BnrImgID.Image != "")
            {
                if (System.IO.File.Exists(completFileName))
                {
                    System.IO.File.Delete(completFileName);
                }
            }

            BannerTbl BnrTbl = db.BannerTbls.Find(BnrID.ID);
            db.BannerTbls.Remove(BnrTbl);
            db.SaveChanges();

            return RedirectToAction("BannerView");
        }
        //BannerTbl Ends

        //DisasterTbl Starts
        [LogInFilter]
        public ActionResult DisasterView()
        {
            return View(db.DisasterTbls.ToList());
        }

        [LogInFilter]
        [HttpGet]
        public ActionResult DisasterCreate()
        {
            return View();
        }

        [LogInFilter]
        [HttpPost]
        public ActionResult DisasterCreate(DisasterTbl Dis)
        {
            if (ModelState.IsValid)
            {
                db.DisasterTbls.Add(Dis);
                db.SaveChanges();

                return RedirectToAction("DisasterView");
            }

            return View();
        }

        [HttpGet]
        [LogInFilter]
        public ActionResult DisasterEdit(int id = 0)
        {
            DisasterTbl DisasterTbl = db.DisasterTbls.Find(id);
            if (DisasterTbl == null)
            {
                return HttpNotFound();
            }


            return View(DisasterTbl);
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult DisasterEdit(DisasterTbl DisID)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    DisasterTbl DisDt = db.DisasterTbls.Where(x => x.ID == DisID.ID).FirstOrDefault();

                    DisDt.Heading = DisID.Heading;
                    DisDt.Body = DisID.Body;
                    db.SaveChanges();
                    return RedirectToAction("DisasterView");
                }

                else
                {
                    return RedirectToAction("DisasterEdit");
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult DisasterDelete(DisasterTbl DisID)
        {
            DisasterTbl DisTbl = db.DisasterTbls.Find(DisID.ID);
            db.DisasterTbls.Remove(DisTbl);
            db.SaveChanges();

            return RedirectToAction("DisasterView");
        }
        //DisasterTbl Ends

        //EkParichay Starts
        [LogInFilter]
        public ActionResult EkParichayView()
        {
            return View(db.EkParichays.ToList());
        }

        [LogInFilter]
        [HttpGet]
        public ActionResult EkParichayCreate()
        {
            return View();
        }

        [LogInFilter]
        [HttpPost]
        public ActionResult EkParichayCreate(EkParichay EkPar)
        {
            if (ModelState.IsValid)
            {
                db.EkParichays.Add(EkPar);
                db.SaveChanges();

                return RedirectToAction("EkParichayView");
            }

            return View();
        }

        [HttpGet]
        [LogInFilter]
        public ActionResult EkParichayEdit(int id = 0)
        {
            EkParichay EkParichay = db.EkParichays.Find(id);
            if (EkParichay == null)
            {
                return HttpNotFound();
            }


            return View(EkParichay);
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult EkParichayEdit(EkParichay EkPar)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    EkParichay EkParDt = db.EkParichays.Where(x => x.ID == EkPar.ID).FirstOrDefault();

                    EkParDt.Heading = EkPar.Heading;
                    EkParDt.Description = EkPar.Description;
                    db.SaveChanges();
                    return RedirectToAction("EkParichayView");
                }

                else
                {
                    return RedirectToAction("EkParichayEdit");
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult EkParichayDelete(EkParichay EkParID)
        {
            EkParichay EkParTbl = db.EkParichays.Find(EkParID.ID);
            db.EkParichays.Remove(EkParTbl);
            db.SaveChanges();

            return RedirectToAction("EkParichayView");
        }
        //EkParichay Ends

        //SamacharTbl Starts
        [LogInFilter]
        public ActionResult SamacharView()
        {
            return View(db.SamacharTbls.ToList());
        }

        [LogInFilter]
        [HttpGet]
        public ActionResult SamacharCreate()
        {
            return View();
        }

        [LogInFilter]
        [HttpPost]
        public ActionResult SamacharCreate(SamacharTbl Smchr, HttpPostedFileBase file)
        {
            if (ModelState.IsValid)
            {
                if (file != null)
                {
                    string FileExtn = System.IO.Path.GetExtension(file.FileName);
                    string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                    file.SaveAs(HttpContext.Server.MapPath("~/Image/") + FileString);
                    Smchr.Image = FileString;
                }

                else
                {
                    Smchr.Image = "";
                }

                db.SamacharTbls.Add(Smchr);
                db.SaveChanges();

                return RedirectToAction("SamacharView");
            }

            return View();
        }

        [HttpGet]
        [LogInFilter]
        public ActionResult SamacharEdit(int id = 0)
        {
            SamacharTbl SamacharTbl = db.SamacharTbls.Find(id);
            if (SamacharTbl == null)
            {
                return HttpNotFound();
            }


            return View(SamacharTbl);
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult SamacharEdit(SamacharTbl SmchrID, HttpPostedFileBase file)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    SamacharTbl SmchrDt = db.SamacharTbls.Where(x => x.ID == SmchrID.ID).FirstOrDefault();

                    if (file != null)
                    {
                        SamacharTbl MySmchrID = db.SamacharTbls.Where(x => x.ID == SmchrID.ID).FirstOrDefault();

                        string completFileName = HttpContext.Server.MapPath("~/Image/" + MySmchrID.Image);
                        if (System.IO.File.Exists(completFileName))
                        {
                            System.IO.File.Delete(completFileName);

                        }
                        string FileExtn = System.IO.Path.GetExtension(file.FileName);
                        string FileString = System.DateTime.Now.Ticks.ToString() + FileExtn;

                        file.SaveAs(HttpContext.Server.MapPath("~/Image/") + FileString);
                        SmchrID.Image = FileString;

                        SmchrDt.Heading = SmchrID.Heading;
                        SmchrDt.Description = SmchrID.Description;
                        SmchrDt.Image = SmchrID.Image;
                        SmchrDt.Alt = SmchrID.Alt;
                        db.SaveChanges();
                        return RedirectToAction("SamacharView");
                    }

                    else
                    {
                        SmchrDt.Heading = SmchrID.Heading;
                        SmchrDt.Description = SmchrID.Description;
                        SmchrDt.Alt = SmchrID.Alt;
                        db.SaveChanges();
                        return RedirectToAction("SamacharView");
                    }

                }

                else
                {
                    return RedirectToAction("SamacharEdit");
                }
            }

            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}",
                                                validationError.PropertyName,
                                                validationError.ErrorMessage);
                    }
                }
            }

            return View();
        }

        [HttpPost]
        [LogInFilter]
        public ActionResult SamacharDelete(SamacharTbl SmchrID)
        {
            SamacharTbl SmchrIDID = db.SamacharTbls.Where(x => x.ID == SmchrID.ID).FirstOrDefault();
            string ImgVal = Convert.ToString(SmchrID.Image);

            string completFileName = HttpContext.Server.MapPath("~/Images/" + ImgVal);

            if (SmchrIDID.Image != null || SmchrIDID.Image != "")
            {
                if (System.IO.File.Exists(completFileName))
                {
                    System.IO.File.Delete(completFileName);
                }
            }

            SamacharTbl SmchrTbl = db.SamacharTbls.Find(SmchrID.ID);
            db.SamacharTbls.Remove(SmchrTbl);
            db.SaveChanges();

            return RedirectToAction("SamacharView");
        }
        //SamacharTbl Ends

        //Search Image Starts
        [HttpGet]
        public ActionResult SearchImage()
        {
            List<ImageTbl> ImageData = db.ImageTbls.OrderByDescending(x => x.Date).ToList<ImageTbl>();
            return View(ImageData);
        }

        [HttpPost]
        public JsonResult getSearchImage(string searchTxt, string searchClmn)
        {
            //Thread.Sleep(2000);
            string Message="";

            if (searchClmn == "ID")
            {
                int id = Convert.ToInt32(searchTxt);
                var result = (from r in db.ImageTbls
                              where r.ID == id
                              select new { r.ID, r.Image, r.Description, r.ImgGroup, r.Alt, r.Year,r.Date }).ToList();
                return Json(result);
            }

            else if (searchClmn == "Description")
            {
                var result = (from r in db.ImageTbls
                              where r.Description.Contains(searchTxt)
                              select new { r.ID, r.Image, r.Description, r.ImgGroup, r.Alt, r.Year }).ToList();
                return Json(result);
            }

            else if (searchClmn == "Image")
            {
                var result = (from r in db.ImageTbls
                              where r.Image.Contains(searchTxt)
                              select new { r.ID, r.Image, r.Description, r.ImgGroup, r.Alt, r.Year }).ToList();
                return Json(result);
            }

            else if (searchClmn == "ImgGroup")
            {
                var result = (from r in db.ImageTbls
                              where r.ImgGroup.Contains(searchTxt)
                              select new { r.ID, r.Image, r.Description, r.ImgGroup, r.Alt, r.Year }).ToList();
                return Json(result);
            }

            else if (searchClmn == "Year")
            {
                var result = (from r in db.ImageTbls
                              where r.Year == searchTxt
                              select new { r.ID, r.Image, r.Description, r.ImgGroup, r.Alt, r.Year }).ToList();
                return Json(result);
            }
            else
            {
                var result = (from r in db.ImageTbls
                              select new { r.ID, r.Image, r.Description, r.ImgGroup, r.Alt, r.Year }).ToList();
                return Json(result);
            }

        }
        //Search Image Ends

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
