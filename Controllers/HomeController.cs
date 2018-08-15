using SendGrid;
using SendGrid.Helpers.Mail;
using shanesvacuums.Helpers;
using shanesvacuums.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace shanesvacuums.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "What We Do";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Contact Shane's";
            MailModel mailModel = new MailModel();
            mailModel.EmailSent = false;
            return View(mailModel);
        }

        public ActionResult Products()
        {
            ViewBag.Message = "Our Products";

            return View();
        }

        [HttpPost]
        public ActionResult Contact(MailModel e)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    //prepare email
                    var message = new StringBuilder();
                    message.Append("A new quote request has been received");
                    message.Append("Name: " + e.Name + "\n");
                    message.Append("Email: " + e.Email + "\n");
                    message.Append("Telephone: " + e.Telephone + "\n\n");
                    message.Append("Message: " + e.Message);

                    var HTMLContent = new StringBuilder();
                    HTMLContent.Append("<strong>A new quote request has been received</strong>");
                    HTMLContent.Append("<p>Name: " + e.Name + "</p>\n");
                    HTMLContent.Append("<p>Email: " + e.Email + "</p>\n");
                    HTMLContent.Append("<p>Telephone: " + e.Telephone + "</p>\n\n");
                    HTMLContent.Append("<p>Message: " + e.Message + "</p>\n\n");

                    var response = SendMailTask(message.ToString(), HTMLContent.ToString());
                    Response result = response.Result;

                    ViewBag.SuccessMessage = "Your message has been sent! Thank you!";
                }
            }
            catch (Exception ex)
            {
                //Response.Write("Exception occurred: " + ex.Message);
                ViewBag.ErrorMessage = "Exception occurred: " + ex.Message;
            }
            MailModel mailModel = new MailModel();
            mailModel.EmailSent = true;
            return View(mailModel);
        }

        public static async Task<Response> SendMailTask(string emailContentMessage, string emailHTMLContent)
        {
            // maybe need APPSETTING_SENDGRID_APIKEY
            //var apiKey = System.Environment.GetEnvironmentVariable("SENDGRID_APIKEY");
            var apiKey = WebConfigurationManager.AppSettings["SENDGRID_APIKEY"];
            var client = new SendGridClient(apiKey);

            string emailToAddresses = WebConfigurationManager.AppSettings["emailToAddresses"];
            string emailFromAddress = WebConfigurationManager.AppSettings["emailFromAddress"];
            string emailBCCAddress = WebConfigurationManager.AppSettings["emailBCCAddress"];
            string emailSubject = WebConfigurationManager.AppSettings["emailSubject"];
            string emailBCC = WebConfigurationManager.AppSettings["emailBCCAddress"];
            int isTest = Convert.ToInt32(WebConfigurationManager.AppSettings["isTest"]);

            var msg = new SendGridMessage()
            {
                From = new EmailAddress(emailFromAddress, "Shane's Built In Vacuums Web Form"),
                Subject = emailSubject,
                PlainTextContent = emailContentMessage,
                HtmlContent = emailHTMLContent
            };

            if (isTest > 0)
            {

            }
            else
            {
                var splitAddresses = emailToAddresses.Split(';');
                foreach (string address in splitAddresses)
                {
                    msg.AddTo(new EmailAddress(address));
                }
            }

            msg.AddBcc(new EmailAddress(emailBCC));

            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.Body.ReadAsStringAsync().Result); // The message will be here
            Console.WriteLine(response.Headers.ToString());
            return response;
        }


    }
}