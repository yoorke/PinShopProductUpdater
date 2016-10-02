using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using System.Net;

namespace PinShopProductUpdater
{
    public static class Common
    {
        public static void log(string message, bool newLine, string logFilename)
        {
            if (newLine)
                Console.WriteLine(DateTime.Now.ToString() + " - " + message);
            else
                Console.Write(DateTime.Now.ToString() + " - " + message);

            using (StreamWriter writer = new StreamWriter("log/" + logFilename, true, Encoding.GetEncoding(65001)))
            {
                if (newLine)
                    writer.WriteLine(DateTime.Now.ToString() + " - " + message);
                else
                    writer.Write(DateTime.Now.ToString() + " - " + message);
            }
        }

        public static void sendMail(string message, string status, string header)
        {
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["emailSource"], ConfigurationManager.AppSettings["emailSourceName"]);
            mail.To.Add(new MailAddress(ConfigurationManager.AppSettings["emailDestination"]));
            mail.Subject = "Automatsko ažuriranje proizvoda";
            mail.IsBodyHtml = true;
            StringBuilder body = new StringBuilder();
            body.Append("<p style='display:block;padding:0.5em;text-align:center;background-color:" + getColor(status)[0] + ";color:" + getColor(status)[1] + "'>" + header + "</p>");
            body.Append("<br/><p>" + message + "</p>");
            mail.Body = body.ToString();

            SmtpClient smtp = getSmtp();
            smtp.Send(mail);

        }

        private static string[] getColor(string status)
        {
            switch (status)
            {
                case "success":
                    {
                        return new string[] { "#00ff00", "#000000" };
                    }
                case "working":
                    {
                        return new string[] { "#0000ff", "#ffffff" };
                    }
                case "danger":
                    {
                        return new string[] { "#ff0000", "#000000" };
                    }
                default:
                    {
                        return new string[] { "#ffffff", "#000000" };
                    }
            }
        }

        private static SmtpClient getSmtp()
        {
            SmtpClient smtp = new SmtpClient();
            NetworkCredential networkCredential = new NetworkCredential(ConfigurationManager.AppSettings["emailSource"], ConfigurationManager.AppSettings["emailSourcePassword"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = networkCredential;
            smtp.Host = ConfigurationManager.AppSettings["emailSourceSmtp"];
            smtp.Port = int.Parse(ConfigurationManager.AppSettings["emailSourceSmtpPort"]);
            smtp.EnableSsl = bool.Parse(ConfigurationManager.AppSettings["emailSourceSSl"]);

            return smtp;
        }
    }
}
