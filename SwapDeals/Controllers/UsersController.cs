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
    public class UsersController : Controller
    {
        private SwapDealsDBEntities db = new SwapDealsDBEntities();
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
            if (Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            return View(db.Users.ToList());
        }

        // GET: Users/Details/5
       


        [HttpGet]
        public ActionResult SignUp()
        {
            return View();
        }
        [HttpPost]
        public ActionResult SignUp(User user)
        {
            if (ModelState.IsValid)
            {
                var checkUser = db.Users.Where(u => u.UserEmail.Equals(user.UserEmail)).FirstOrDefault();
                if (checkUser != null)
                {
                    ViewBag.errorMsg = "An account already exists with this email";
                }
                else
                {
                    user.Rating = 8;
                    try
                    {
                        db.Users.Add(user);
                        db.SaveChanges();
                        ViewBag.errorMsg = "Signup successful";
                        // return Content("Sign up successful");
                    }
                    catch (Exception e)
                    {
                        ViewBag.errorMsg = "e";

                    }
                }

            }
            return View();
        }
        public ActionResult Details()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Session["user_id"] == null)
                return RedirectToAction("Index", "Home");
            int id = Convert.ToInt32(Session["user_id"]);
            var user = db.Users.Where(u => u.UserID == id).FirstOrDefault();
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }
        [HttpGet]
        public ActionResult Update(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Convert.ToInt32(Session["user_id"]) != id)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Update(User user)
        {
           // if (ModelState.IsValid)
            //{
                try
                {
                    var cu = db.Users.Find(Convert.ToInt32(Session["user_id"]));
                    user.UserEmail = "" + cu.UserEmail;
                    user.Rating = cu.Rating;
                //db.Entry(user).State = EntityState.Modified;
                db.Database.ExecuteSqlCommand("Update Users set UserName = '" + user.UserName + "' , UserPassword = '" +
                   user.UserPassword + "', UserPhone = '"+user.UserPhone+"', UserAdress = '"+user.UserAdress+"' where UserID = "+cu.UserID);
                    db.SaveChanges();
                }catch(Exception e)
                {
                    return Content(e.ToString());
                }
                
           // }
            return RedirectToAction("Details","Users");
        }
        [HttpGet]
        public ActionResult Login()
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Session["user_id"] !=null)
            return RedirectToAction("Index", "Home");
            return View();
        }
        [HttpPost]
        public ActionResult Login(TempUser tempUser)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Where(u => u.UserEmail.Equals(tempUser.UserEmail) && u.UserPassword.Equals(tempUser.UserPassword)).FirstOrDefault();
                if (user != null)
                {
                    //ratings
                    var d1 = db.Deals.SqlQuery("Select * from Deals where UserID1 = " + user.UserID).ToList<Deal>();
                    var d2 = db.Deals.SqlQuery("Select * from Deals where UserID2 = " + user.UserID).ToList<Deal>();
                    int s = 0, c = 0;
                    float r;
                    bool check = false;
                    foreach (var x in d1)
                    {
                        c++;
                        s += x.User1Rating;
                        check = true;
                    }
                    foreach (var y in d2)
                    {
                        c++;
                        s += y.User2Rating;
                        check = true;
                    }
                    r = (float)s / c;
                    if (!check)
                    {
                        r = (float)user.Rating;
                        // System.Diagnostics.Debug.WriteLine(user.Rating);

                        System.Diagnostics.Debug.WriteLine(r);
                    }
                    if (r >= 3.5)
                    {
                        db.Database.ExecuteSqlCommand("Update Users set Rating = " + r + " where UserID = " + user.UserID);
                        db.SaveChanges();
                        ViewBag.msg = "Log in successful";
                        Session["user_id"] = user.UserID;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ViewBag.msg = "Your account has been disabled due to poor ratings. Contact admin";

                }
                else
                    ViewBag.msg = "Wrong email or password";
            }

            return View();
        }


        public ActionResult Logout()
        {

            Session.Abandon();
            //  Session.Clear();
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");

            return RedirectToAction("Login");
        }
        public ActionResult Delete(int? id)
        {
            HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            HttpContext.Response.Cache.SetValidUntilExpires(false);
            HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            HttpContext.Response.Cache.SetNoStore();
            HttpContext.Response.ExpiresAbsolute = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
            HttpContext.Response.Expires = 0;
            HttpContext.Response.Cache.AppendCacheExtension("no-store, no-cache, must-revalidate, proxy-revalidate, post-check=0, pre-check=0");
            if (Session["admin"] == null)
                return RedirectToAction("Index", "Home");
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = db.Users.Find(id);
            db.Users.Remove(user);
            db.SaveChanges();
            return RedirectToAction("Index");
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
