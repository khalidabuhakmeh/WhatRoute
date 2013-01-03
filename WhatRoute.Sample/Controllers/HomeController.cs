using System.Web.Mvc;

namespace WhatRoute.Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Show(int id)
        {
            return View();
        }

        public ActionResult Another()
        {
            return View();
        }

        private new ActionResult View()
        {
            return base.View("index");
        }

        public ActionResult Gross(int a, int really, int gross, int path)
        {
            return View();
        }
    }
}
