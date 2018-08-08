using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;

namespace WebApp.Models
{
    public static  class Email
    {
        public static string SendEmail(string type, string emailid, string val)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient();
                mail.From = new MailAddress("noreply@test.com");
                mail.To.Add(emailid);
                switch (type)
                {
                    case "VERIFY":
                        mail.Subject = "Verify Email";
                        mail.Body = emailBodyforVerification(val);
                        break;
                    case "FORGOT":
                        mail.Subject = "Password Reset";
                        mail.Body = emailBodyforForgotPassword(val);
                        break;
                }

                mail.IsBodyHtml = true;
                SmtpServer.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                SmtpServer.PickupDirectoryLocation = @"C:\Praveen\Project\";
                SmtpServer.Send(mail);
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();                
            }
        }

        private static string emailBodyforVerification(string id)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Hi,<br/>");
            sb.Append("<br/>");
            sb.Append("kindly verify your account by activating it through below link.<br/>");
            sb.Append("<a href=http://localhost:58521/Login/Verify?ID=" + id + ">Verify Email</a>");
            sb.Append("<br/>");
            sb.Append("<br/>");
            sb.Append("Best Regards,");
            sb.Append("<br/>");
            sb.Append("ABC TEAM");
            return sb.ToString();
        }

        private static string emailBodyforForgotPassword(string pass)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Hi,<br/>");
            sb.Append("<br/>");
            sb.Append("your password is : <br/>");
            sb.Append(pass);
            sb.Append("<br/>");
            sb.Append("<br/>");
            sb.Append("Best Regards,");
            sb.Append("<br/>");
            sb.Append("ABC TEAM");
            return sb.ToString();
        }
    }
}