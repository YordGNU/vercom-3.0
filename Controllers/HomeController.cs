using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vercom.Interfaces;
using vercom.Models;

namespace vercom.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        VERCOMEntities db = new VERCOMEntities();

        [RBAC]
        public ActionResult Index()
        {
            var job = new StockJob();
            job.Execute(null);
            return View();
        }     
      
    }
}