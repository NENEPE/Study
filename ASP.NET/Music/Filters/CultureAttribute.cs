using Microsoft.AspNetCore.Mvc.Filters;
using MusicPortal.Services;
using System.Globalization;

namespace MusicPortal.Filters
{

    public class CultureAttribute : Attribute, IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext) {}

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string? cultureName = null;

            var cultureCookie = filterContext.HttpContext.Request.Cookies["lang"];
            if (cultureCookie != null)
                cultureName = cultureCookie;
            else
            {
                cultureName = "en";
            }

            List<string> cultures = filterContext.HttpContext.RequestServices.GetRequiredService<ILangRead>()
                                    .languageList().Select(t => t.ShortName).ToList()!;
            if (!cultures.Contains(cultureName))
            {
                cultureName = "en";
            }

            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
        }
    }
}