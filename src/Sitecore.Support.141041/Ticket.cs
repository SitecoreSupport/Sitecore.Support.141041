using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Web.Authentication;
using System;
using System.Web;
using Sitecore.Pipelines.LoggedIn;
using Sitecore.Security.Accounts;

namespace Sitecore.Support.Pipelines.LoggedIn
{
    public class Ticket : LoggedInProcessor
    {
        public override void Process(LoggedInArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            string str = TicketManager.CreateTicket(args.Username, args.StartUrl, args.Persist);
            if (!string.IsNullOrEmpty(str))
            {
                HttpContext current = HttpContext.Current;
                if (current != null)
                {
                    HttpCookie cookie2 = new HttpCookie("sitecore_userticket", str);
                    cookie2.HttpOnly = true;
                    cookie2.Expires = args.Persist ? DateTime.UtcNow.Add(Settings.Authentication.ClientPersistentLoginDuration) : DateTime.MinValue;
                    HttpCookie cookie = cookie2;
                    current.Response.AppendCookie(cookie);
                    User user = Sitecore.Security.Accounts.User.FromName(args.Username, false);
                    HttpCookie langCookie = new HttpCookie("sc_lang", user.Profile.ClientLanguage);
                    langCookie.HttpOnly = true;
                    langCookie.Expires = args.Persist ? DateTime.UtcNow.Add(Settings.Authentication.ClientPersistentLoginDuration) : DateTime.MinValue;
                    current.Response.AppendCookie(langCookie);
                }
            }
        }
    }
}