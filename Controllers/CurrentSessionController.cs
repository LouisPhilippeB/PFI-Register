using Models;
using System;
using System.Web.Mvc;
using static Controllers.AccessControl;

namespace Controllers
{
    [UserAccess(Access.View)]
    public class CurrentSessionController : Controller
    {
        public ActionResult Edit()
        {
            ViewBag.Year = NextSession.Year;
            ViewBag.Session = NextSession.ValidSessions.Contains(1) ? "Automne" : "Hiver";
            return View();
        }

        [HttpPost]
        [UserAccess(Access.Write)]
        public ActionResult Edit(int year, string session)
        {
            if (year >= 2020 && year <= 2100)
            {
                if (session == "Automne")
                {
                    NextSession.CurrentDate = new DateTime(year, 2, 1);
                }
                else
                {
                    NextSession.CurrentDate = new DateTime(year, 9, 1);
                }
            }

            return Redirect("/Students/List");
        }
    }
}