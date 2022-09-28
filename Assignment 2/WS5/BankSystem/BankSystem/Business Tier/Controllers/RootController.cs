using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Business_Tier.Controllers
{
    public class RootController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Kesh Bank";
            return View();
        }
        // This is the homepage / function that gets run first.

        public ActionResult Users()
        {
            ViewBag.Message = "Users Page";
            return View();
        }

        public ActionResult Accounts()
        {
            ViewBag.Message = "Accounts Page";
            return View();
        }

        public ActionResult Transactions()
        {
            ViewBag.Message = "Transactions Page";
            return View();
        }
    }
}