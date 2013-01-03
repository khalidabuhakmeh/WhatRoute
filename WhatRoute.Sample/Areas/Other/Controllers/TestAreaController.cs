using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WhatRoute.Sample.Areas.Other.Controllers
{
    public class TestAreaController : Controller
    {
        //
        // GET: /Other/TestArea/

        public ActionResult Index()
        {
            return View("index");
        }

    }
}
