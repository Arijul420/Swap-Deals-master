using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SwapDeals.Models;

namespace SwapDeals.Controllers
{
    public class DealsController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();

        // GET: Deals
        public ActionResult Index()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Session["admin"] == null && Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            var deals = db.Deals.Include(d => d.Booking).Include(d => d.User).Include(d => d.User1);
 
            if (Session["admin"] != null)
            {

            }
            else
            {
                int uid = Convert.ToInt32(Session["user_id"]);
                deals = db.Deals.Include(d => d.Booking).Include(d => d.User).Include(d => d.User1).Where(d => d.UserID1.Equals(uid) || d.UserID2.Equals(uid));
            }
            return View(deals.ToList());
        }

        // GET: Deals/Details/5
        public ActionResult Details(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
           // if (Session["user_id"] == null && Session["admin"] == null)
               // return RedirectToAction("Home", "Index");
            
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deal deal = db.Deals.Find(id);
            if (Convert.ToInt32(Session["user_id"]) !=deal.UserID1 && Convert.ToInt32(Session["user_id"]) != deal.UserID2
                && Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            var b = db.Bookings.Find(deal.BookingID);
            var ad = db.Advertisements.Find(b.AdID);
            ViewData["ad"] = ad;
            if (deal == null)
            {
                return HttpNotFound();
            }
            return View(deal);
        }

        public ActionResult Revenue()
        {
            if (Session["admin"] == null)
                return RedirectToAction("Index","Home");
            var d = db.Deals.SqlQuery("Select *from Deals").ToList<Deal>();
            Decimal tot = 0;
            foreach(Deal x in d)
            {
                tot += x.Revenue;
            }
            System.Diagnostics.Debug.WriteLine(tot);
            ViewData["r"] = tot;
            return View();
        }

        [HttpGet]
        public ActionResult FinishPostedDeal(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (id != null)
            {
                Session["AdID"] = id;
                Advertisement a = db.Advertisements.SqlQuery("Select * from Advertisements  where AdID = " + id).FirstOrDefault();
                if (a != null)
                {
                    if (a.UserID != Convert.ToInt32(Session["user_id"]))
                        return Content("Access denied");
                }
                Booking b = db.Bookings.SqlQuery("Select * from Booking  where AdID = "+id).FirstOrDefault();
                if (b != null)
                {
                    Deal d = db.Deals.SqlQuery("Select * from Deals  where BookingID = " + b.BookingID).FirstOrDefault();
                    
                    if (d != null)
                        return Content("Already completed");
                }
                else
                {
                    return Content("Your ad have not booked yet.");
                }
                
            }
            return View();
        }

       
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishPostedDeal(Deal deal)
        {
            try
            {
                Booking b = db.Bookings.SqlQuery("Select * from Booking where AdID = " + Convert.ToInt32(Session["AdID"])).FirstOrDefault();
                if (b == null)
                    return Content("Your ad have not booked yet.");
                deal.BookingID = b.BookingID;
                deal.UserID1 = Convert.ToInt32(Session["user_id"]);
                deal.UserID2 = b.UserID;
                deal.User1Rating = -1;
                Advertisement a = db.Advertisements.SqlQuery("Select * from Advertisements where AdID = " + Convert.ToInt32(Session["AdID"])).FirstOrDefault();
                deal.Revenue = (int)a.Payment;
                db.Deals.Add(deal);
                db.SaveChanges();
                return RedirectToAction("Index");
            }catch(Exception e)
            {
                return Content(e.ToString());
            }

        }
        [HttpGet]
        public ActionResult FinishBookedDeal(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (id != null)
            {
                Session["bookingID"] = id;
                Booking b = db.Bookings.SqlQuery("Select *from Booking where BookingID = " + id).FirstOrDefault();
                if(b!=null)
                {
                   // System.Diagnostics.Debug.WriteLine(b.UserID + " " + Convert.ToInt32(Session["user_id"]));
                    if (b.UserID != Convert.ToInt32(Session["user_id"]))
                        return Content("Access Denied");
                } 
                else
                {
                    return RedirectToAction("Index","Home");
                }
                Deal d = db.Deals.SqlQuery("Select *from Deals where BookingID = " + id).FirstOrDefault();
                
                if (d != null)
                {
                    
                    if (d.User1Rating != -1)
                        return Content("Already finished");
                    else
                        return View();
                }
                else
                {
                    return Content("Wait for Ad poster to finalize");
                }

            }

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult FinishBookedDeal(Deal deal)
        {
            try
            {
                
                Deal d = db.Deals.SqlQuery("Select * from Deals where BookingID = " + Convert.ToInt32(Session["bookingID"])).FirstOrDefault();
                
                if (d != null)
                {
                    db.Database.ExecuteSqlCommand("Update Deals set User1Rating = " + deal.User1Rating + " where DealID = " + d.DealID);
                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand("Update Booking set BookingStatus = 0 where BookingID = " + Convert.ToInt32(Session["bookingID"]));
                    db.SaveChanges();
                    Booking a = db.Bookings.SqlQuery("Select * from Booking where BookingID = " + Convert.ToInt32(Session["bookingID"])).FirstOrDefault();

                    db.Database.ExecuteSqlCommand("Update Advertisements set PriorityStatus = -3 where AdID = "+a.AdID);
                    return RedirectToAction("Index");
                }
                else
                {
                    return Content("Wait for Ad poster to finalize");
                }
                
            }
            catch (Exception e)
            {
                return Content(e.ToString());
            }

        }

     
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
