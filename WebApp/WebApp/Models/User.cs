using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace WebApp.Models
{
    public class RegisterUser
    {
        [Required(ErrorMessage = "Required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Required.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required.")]
        [System.Web.Mvc.Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public string VerifyID { get; set; }

        [Required(ErrorMessage = "Required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        public System.DateTime CreatedDate { get; set; }

    }

    public class LoginUser
    {
        [Required(ErrorMessage = "Required.")]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Required.")]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
        public string Status { get; set; }
    }

    public class User
    {
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string VerifyID { get; set; }
        public string Active { get; set; }
        public string CreatedDate { get; set; }
    }

    public class DB_Entities : IDisposable
    {
        public static List<User> lstUser = new List<User>();
        public static string filename = @"C:\Praveen\Project\USER_DETAILS.xml";
        public DB_Entities()
        {
            if (lstUser.Count == 0)
                LoadDBUser();             
        }

        public void Dispose()
        {
            //add disposable content
        }

        public static List<User> getAllUser()
        {
            DB_Entities db = new DB_Entities();
            if (lstUser != null && lstUser.Count == 0)
                db.LoadDBUser();
            return lstUser;
        }

        public LoginUser GetUserProfile(string username, string password)
        { 
            if (lstUser != null && lstUser.Count == 0)
                LoadDBUser();
            LoginUser us = new LoginUser();
            string pass = string.Empty;
            if (lstUser.Count > 0)
            {
                var user = lstUser.FirstOrDefault(s => s.UserId == username);

                if(user != null)
                    pass = Encryption.Decrypt(user.Password);

                if (user == null)
                    us.Status = "User Not Found.";
                else if (user != null && user.UserId == username && pass != password)
                    us.Status = "Invalid Password entered.";
                else if (user != null && user.UserId == username && pass == password && user.Active == "Y")
                    us.UserId = user.UserId;
                else if (user != null && user.UserId == username && pass == password && user.Active == "N")
                    us.Status = "Please Verify email id from mail sent."; 
            }
            else
                us.Status = "User not found.";

            return us;
        } 

        public bool RegisterUserProfile(RegisterUser user)
        {
            try
            {
                foreach (User _user in lstUser)
                {
                    if (user.UserId == _user.UserId || user.Email == _user.Email)
                        return false;
                } 
                AddDBUser(user);
                return true;
            }
            catch (Exception ex)
            {
                //throw ex;
                return false;
            }
        }
        
        public void AddDBUser(RegisterUser us)
        {
            User details = new User();
            details.UserId = us.UserId;
            details.Email = us.Email;
            details.Password = Encryption.Encrypt(us.Password);
            details.VerifyID = us.VerifyID;
            details.Active = "N";
            details.CreatedDate = DateTime.Now.ToString();
            lstUser.Add(details);
            writexml();
        }

        public void LoadDBUser()
        {
            readxml();
        } 

        public void UpdateDBUser(string id)
        {
            foreach (User user in lstUser)
            {
                if (user.VerifyID == id)
                    user.Active = "Y";
            }
            writexml();
        }

        public void writexml()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
            using (TextWriter writer = new StreamWriter(filename))
            {
                serializer.Serialize(writer, lstUser);
            } 
        }

        public void readxml()
        {
            if (File.Exists(filename))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(List<User>));
                using (StreamReader reader = new StreamReader(filename))
                {
                    using (var xmlReader = XmlReader.Create(reader))
                    {
                        lstUser = (List<User>)serializer.Deserialize(xmlReader);
                    }
                }
            }
        }
    }
}