using DAL;
using EmailHandling;
using Models;
using System;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Wikimedia
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            CultureInfo culture = new CultureInfo("fr-FR");
            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            foreach (Login login in DB.Logins.ToList().Copy())
            {
                if (login.User == null)
                    DB.Logins.Delete(login.Id);
            }

            foreach (UnverifiedEmail uvEmail in DB.UnverifiedEmails.ToList().Copy())
            {
                if (uvEmail.User == null)
                    DB.UnverifiedEmails.Delete(uvEmail.Id);
            }

            foreach (RenewPasswordCommand renewPC in DB.RenewPasswordCommands.ToList().Copy())
            {
                if (renewPC.User == null)
                    DB.RenewPasswordCommands.Delete(renewPC.Id);
            }

            foreach (Models.Event currentEvent in DB.Events.ToList().Copy())
            {
                if (currentEvent.User == null)
                    DB.Events.Delete(currentEvent.Id);
            }

            foreach (Notification notification in DB.Notifications.ToList().Copy())
            {
                if (notification.User == null)
                    DB.Notifications.Delete(notification.Id);
            }

            foreach (Registration registration in DB.Registrations.ToList().Copy())
            {
                if (registration.Student == null || registration.Course == null)
                    DB.Registrations.Delete(registration.Id);
            }

            foreach (Allocation allocation in DB.Allocations.ToList().Copy())
            {
                if (allocation.Teacher == null || allocation.Course == null)
                    DB.Allocations.Delete(allocation.Id);
            }
        }

        protected void Session_Start()
        {
        }

        protected void Session_End()
        {
            User connectedUser = Session["ConnectedUser"] as User;

            if (connectedUser != null)
                connectedUser.Online = false;
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}