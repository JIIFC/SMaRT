using System.Globalization;

namespace SMARTV3.Helpers
{
    public class CultureHelper
    {
        public string GetCurrentCulture()
        {
            // string test = $"CurrentCulture:{CultureInfo.CurrentCulture.Name}, CurrentUICulture:{CultureInfo.CurrentUICulture.Name}";

            string cultureVal = CultureInfo.CurrentCulture.Name;

            return cultureVal;
        }
    }
}