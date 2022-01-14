using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TradingBotWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Algos()
        {
            ViewBag.Message = "Algo me bb";

            return View();
        }

        public ActionResult Commands()
        {
            ViewBag.Message = "Your wish is my command";

            return View();
        }
    }
}