using System.Web.Mvc;

namespace Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return Redirect("/Students/List");
        }
    }
}