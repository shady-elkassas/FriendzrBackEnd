using System;
using System.Collections.Generic;
using System.Text;

namespace CRM.Services .Wrappers
{
    public class PagedResponse<T>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int TotalRecords { get; set; }
        public T Data { get; set; }

        public PagedResponse(T data)
        {
            this.PageNumber = 0;
            this.PageSize = 0;
            this.Data = data;
            this.TotalRecords = 0;
            this.TotalPages = 0;
        }

        public PagedResponse(T data, int pageNumber, int pageSize, int totalRecords)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Data = data;
            this.TotalRecords = totalRecords;

            var totalPages = this.PageSize==0?0: totalRecords / this.PageSize;
            if (PageSize != 0)
            {
                if (totalRecords < this.PageSize)
                    totalPages = 1;

                else if (totalRecords % this.PageSize > 0)
                    totalPages = (this.PageSize == 0 ? 0 : (totalRecords / this.PageSize)) + 1;
            }
            this.TotalPages = totalPages;
        }
    }

}
