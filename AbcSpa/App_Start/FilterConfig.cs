using System.Web.Mvc;

namespace AbcSpa
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            // Added by Ramon
            // This applies the AuthorizeAttribute to all controller actions in the application.
            // Use [AllowAnonymous] in the Actions that dont require the [Authorize] protection.
            filters.Add(new AuthorizeAttribute());  
            //------------------------------------
            filters.Add(new HandleErrorAttribute());
        }
    }
}