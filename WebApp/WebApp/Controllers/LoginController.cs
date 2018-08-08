using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;
using WebApp.Models;

namespace WebApp.Controllers
{
    public class LoginController : Controller
    {
        public ActionResult Index()
        { 
            return View();
        }

        [HttpPost]
        public ActionResult Index(LoginUser objUser)
        {
            if (ModelState.IsValid)
            {
                using (DB_Entities db = new DB_Entities())
                {
                    var obj = db.GetUserProfile(objUser.UserId, objUser.Password);
                    if (obj != null && String.IsNullOrEmpty(obj.Status))
                    {
                        Session["UserID"] = obj.UserId;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        TempData["Message"] = obj.Status;
                        TempData.Keep();
                    }
                }
            }
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegisterUser objReg)
        {
            if (ModelState.IsValid)
            {
                using (DB_Entities db = new DB_Entities())
                {
                    objReg.VerifyID = Guid.NewGuid().ToString();
                    bool obj = db.RegisterUserProfile(objReg);
                    if (obj && string.IsNullOrEmpty(Email.SendEmail("VERIFY",objReg.Email,objReg.VerifyID)))
                    { 
                        TempData["Message"] = "Please check email " + objReg.Email + " for Verification.";
                        TempData.Keep();
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    { 
                        TempData["Message"] = "This user already Exist in DB.";
                        TempData.Keep();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult Verify()
        {
            if (Request.QueryString["ID"] != null)
            {
                DB_Entities db = new DB_Entities();
                db.UpdateDBUser(Request.QueryString["ID"]);
                TempData["Message"] = "Email verified succesfully.";
                TempData.Keep();
                return RedirectToAction("Index", "Home");
            }
            else
            {
                TempData["Message"] = "Please check your email and verify your email id.";
                TempData.Keep();
                return RedirectToAction("Index", "Login");
            } 
        }

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            string objUserId = Request.QueryString["userID"];
            foreach (User us in DB_Entities.getAllUser())
            {
                if (us.UserId == objUserId)
                {
                    Email.SendEmail("FORGOT", us.Email, Encryption.Decrypt(us.Password));
                    TempData["Message"] = "Email sent to registered email id.";
                    TempData.Keep();
                    return RedirectToAction("Index", "Login");
                }                    
            }            
            return View();
        }      

    }
}
