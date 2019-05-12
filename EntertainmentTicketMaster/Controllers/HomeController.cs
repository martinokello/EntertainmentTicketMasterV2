using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using EmailServices;
using RepositoryServices.Services;
using TicketMasterDataAccess.ConcreteRepositories;
using TicketMasterDataAccess.UnitOfWork;
using UPAEventsPayPal;

namespace EntertainmentTicketMaster.Controllers
{
    public class HomeController : Controller
    {
        EmailService _emailService;
        public HomeController()
        {
            _emailService = new EmailService(ConfigurationManager.AppSettings["SmtpHostServer"]);
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home";
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            ViewBag.Title = "About";
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Title = "Contact";
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Cancel()
        {
            ViewBag.Title = "Cancel";
            ViewBag.Message = "The Transaction was canceled.";

            return View();
        }        
		public ActionResult Success()
        {
            ViewBag.Title = "Success";
            ViewBag.Message = "You've bought tickets to shows. Check your registered Email for details.";

            return View();
        }        
        public ActionResult PaypalNotify(FormCollection formCollection)
        {
            var paymentVerification = new InstantPaymentNotification(HttpContext.ApplicationInstance.Context.Request,
                ConfigurationManager.AppSettings["BusinessEmail"], formCollection, new BookingRepository(new UnitOfWork()));

            FileInfo fileInfo = new FileInfo(Server.MapPath("~/IPN_Notification/IPNMessage.txt"));
            var ipnWriter = fileInfo.CreateText();

            var isVerified = paymentVerification.ProcessIPNResults(HttpContext.ApplicationInstance.Context, ipnWriter);
            if (isVerified)
                return Content("IPN Verification Successfull");
            return Content("IPN Verification Failed");
        }
        [Authorize()]
        [HttpPost]
        public ActionResult SendEmail([System.Web.Http.FromBody]  HttpPostedFileBase attachment)
        {
            try { 
                //Send Email:
              var emailTo = new List<string>();
                emailTo.AddRange(Request.Form["emailTo"].Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries));
                _emailService.SendEmail(new EmailServices.EmailDomain.TicketMasterEmailMessage { EmailFrom = Request.Form["emailFrom"] , EmailTo= emailTo, Subject = Request.Form["emailSubject"], EmailMessage = Request.Form["emailBody"],  AttachmentStream = (attachment!=null? attachment.InputStream: null ), AttachedFileName = (attachment != null ? attachment.FileName : null) });
                return View("Index");
            }
            catch (Exception e)
            {
                return Json(new { Result = false, Message = e.Message, StackTracke = e.StackTrace });
            }
        }
    }
}