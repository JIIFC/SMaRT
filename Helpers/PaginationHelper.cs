using Microsoft.AspNetCore.Mvc.Rendering;

using static Constants;

namespace SMARTV3.Helpers
{
    public static class PaginationHelper
    {
        public static List<SelectListItem> GetItemsPerPageList(string pageSize)
        {
            List<SelectListItem> PagesToShow = new();

            foreach (var option in PaginationOptions)
            {
                SelectListItem tempItem = new() { Text = option, Value = option };
                if (option == pageSize)
                {
                    tempItem.Selected = true;
                }
                PagesToShow.Add(tempItem);
            }
            return PagesToShow;
        }
    }
}