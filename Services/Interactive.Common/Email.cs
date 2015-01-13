using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Interactive.Common
{
    public class Email
    {
        #region Send email

        public void SendMail(List<string> toAddress, string subject, string body)
        {
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["MailHost"]);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["FromAddress"]);
            foreach (string s in toAddress)
            {
                message.To.Add(s);
            }

            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;

            message.Body += body;
            message.BodyEncoding = Encoding.UTF8;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            client.EnableSsl = true;
            client.UseDefaultCredentials = false;

            NetworkCredential myCredentials = new NetworkCredential(ConfigurationManager.AppSettings["FromAddress"], ConfigurationManager.AppSettings["fromAddressPwd"]);
            client.Credentials = myCredentials;

            client.Send(message);
        }

        public void SendMail(List<string> toAddress, string subject, string body, Dictionary<string, string> LinkedResourcesDic)
        {
            SmtpClient client = new SmtpClient(ConfigurationManager.AppSettings["MailHost"]);

            MailMessage message = new MailMessage();
            message.From = new MailAddress(ConfigurationManager.AppSettings["FromAddress"]);
            foreach (string s in toAddress)
            {
                message.To.Add(s);
            }

            message.Subject = subject;
            message.SubjectEncoding = Encoding.UTF8;

            if (LinkedResourcesDic != null && LinkedResourcesDic.Count > 0)
            {
                //create Alrternative HTML view
                AlternateView htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");
                foreach (KeyValuePair<string, string> kvp in LinkedResourcesDic)
                {
                    //Add Image
                    LinkedResource imageLR = new LinkedResource(System.Web.HttpContext.Current.Server.MapPath("/img/" + kvp.Value));
                    imageLR.ContentId = kvp.Key;
                    //Add the Image to the Alternate view
                    htmlView.LinkedResources.Add(imageLR);
                }
                //Add view to the Email Message
                message.AlternateViews.Add(htmlView);
            }

            message.Body += body;
            message.BodyEncoding = Encoding.UTF8;

            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            client.EnableSsl = true;
            client.UseDefaultCredentials = false;

            NetworkCredential myCredentials = new NetworkCredential(ConfigurationManager.AppSettings["FromAddress"], ConfigurationManager.AppSettings["fromAddressPwd"]);
            client.Credentials = myCredentials;

            client.SendAsync(message, "UserToken");
        }

        #endregion

        #region Email body template from interactive

        public static string EmailBodyTemplate(string link, string content)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("<!DOCTYPE html>");
            strBuilder.Append("<html lang=\"en\">");
            strBuilder.Append("<head>");
            strBuilder.Append("<meta charset=\"UTF-8\">");
            strBuilder.Append("<title>Document</title>");
            strBuilder.Append("<style type=\"text/css\">");
            strBuilder.Append("body{padding: 0; margin:0; font-size: 14px; }");
            strBuilder.Append("#emailContent{ width: 600px; margin: 0 auto; border: 1px solid #005daa;}");
            strBuilder.Append("#emailContent .header{ background:#005daa; padding: 10px;}");
            strBuilder.Append("#emailContent .content{ padding: 10px;}");
            strBuilder.Append("#emailContent .content p a{ color: #005daa; font-weight: bold;}");
            strBuilder.Append("#emailContent .footer{ background:#005daa; padding: 5px;}");
            strBuilder.Append("#emailContent .footer img{ float: left;}");
            strBuilder.Append("#emailContent .footer .terms{float: right; margin-top: 5px;}");
            strBuilder.Append("#emailContent .footer .terms a{color: #fff;padding-right: 10px;}");
            strBuilder.Append(".clear{ clear: both;}");
            strBuilder.Append("</style>");
            strBuilder.Append("</head>");
            strBuilder.Append("<body>");
            strBuilder.Append("<div id=\"emailContent\">");
            strBuilder.Append("<div class=\"header\">");
            strBuilder.Append("<img src=\"cid:logoImage\" alt=\"\">");
            strBuilder.Append("</div>");
            strBuilder.Append("<div class=\"content\">");
            if (!string.IsNullOrEmpty(content))
            {
                strBuilder.Append(content);
            }
            if (!string.IsNullOrEmpty(link))
            {
                strBuilder.Append("<h2>" + link + "</h2>");
            }
            strBuilder.Append("</div>");
            strBuilder.Append("<div class=\"footer\">");
            strBuilder.Append("<img src=\"cid:logoFooterImage\" alt=\"\">");
            strBuilder.Append("<div class=\"terms\">");
            strBuilder.Append("<a href=\"#\">Terms and conditions</a>");
            strBuilder.Append("<a href=\"#\">contact us</a>");
            strBuilder.Append("</div>");
            strBuilder.Append("<div class=\"clear\"></div>");
            strBuilder.Append("</div>");
            strBuilder.Append("</div>");
            strBuilder.Append("</body>");
            strBuilder.Append("</html>");
            return strBuilder.ToString();
        }

        #endregion

        #region Test email address

        public static string[] TestEmailAddress()
        {
            string[] mailAddress = { "patrick.zhong@fabricgroup.com.au", "cass.fan@fabricgroup.com.au", "eric.sun@fabricgroup.com.au", "water.li@fabricgroup.com.au", "ken.cliche@fabricgroup.com.au", "lachlan.greed@fabricgroup.com.au" };
            //string[] mailAddress = { "eric.sun@fabricgroup.com.au" };
            //string[] mailAddress = { "patrick.zhong@fabricgroup.com.au", "cass.fan@fabricgroup.com.au", "eric.sun@fabricgroup.com.au" };
            return mailAddress;
        }

        #endregion
    }
}
