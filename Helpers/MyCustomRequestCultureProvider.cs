using Microsoft.AspNetCore.Localization;

namespace SMARTV3.Helpers
{
    public class MyCustomRequestCultureProvider : RequestCultureProvider
    {
        public override async Task<ProviderCultureResult> DetermineProviderCultureResult(HttpContext httpContext)
        {
            await Task.Yield();
            return new ProviderCultureResult("fr");
        }
    }
}