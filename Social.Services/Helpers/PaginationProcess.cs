using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Social.Services.Helpers
{
    public class PaginationProcess
    {
        public PaginationParamaters GetPaginationParamaters(HttpRequest Request)
        {
            var draw = Request.Form["draw"].FirstOrDefault();
            var sortColumn = Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][data]"].FirstOrDefault();
            var sortColumnDirection = Request.Form["order[0][dir]"].FirstOrDefault();
            var searchValue = Request.Form["search[value]"].FirstOrDefault();
            int pageSize = Convert.ToInt32(Request.Form["length"].FirstOrDefault() ?? "0");
            int skip = Convert.ToInt32(Request.Form["start"].FirstOrDefault() ?? "0");

            return new PaginationParamaters {Draw=draw, PageNumber = skip, PageSize = pageSize, SortColumn = sortColumn, SortColumnDirection = sortColumnDirection,SearchValue=searchValue };
        }
    }
    public  class PaginationParamaters
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public string SortColumn { get; set; }
        public string SortColumnDirection { get; set; }
        public string Draw { get; set; }
        public string SearchValue { get ; set; }    
    }
}
