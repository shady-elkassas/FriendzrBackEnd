using Microsoft.AspNetCore.Http;
using Social.Entity.Models;
using Social.Entity.ModelView;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IGlobalMethodsService
    {
        IHttpContextAccessor _HttpContextAccessor { get; }
        public int getUserIdFromCookie();
        //public string uploadFile(string PlaceOnServer, IFormFile file);
        Task<string> uploadFileAsync(string PlaceOnServer, IFormFile file);
        Task<string> adminuploadFileAsync(string PlaceOnServer, IFormFile file);
        Task<string> uploadFileFromUrl(string PlaceOnServer, string fileUrl);
        Task uploadFile(string PlaceOnServer, string Json);
        public void addCookie(string key, string value, DateTime? expireTime);
        void DeleteFiles(string PlaceOnServer, string fileName);
        void DeleteFiles(string PlaceOnServer, List<string> fileName);
         List<string> getmonthList();
       
        string ConvertNumerals(string input);
        string GetBaseDomain( );
         

        public object GetJsonObjectData(string url, ref object recordToFill);
        public string GetJsonProperityData(string url, string properity);
   

    }
}
