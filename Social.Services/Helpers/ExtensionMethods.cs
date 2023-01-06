using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Social.Services.Helpers
{
    public static class ExtensionMethods
    {
       
        public static List<bool> GetHash(this Bitmap bmpMin)
        {
            List<bool> lResult = new List<bool>();
            //create new image with 16x16 pixel
            //Bitmap bmpMin = new Bitmap(bmpSource, new Size(16, 16));
            for (int j = 0; j < bmpMin.Height; j++)
            {
                for (int i = 0; i < bmpMin.Width; i++)
                {
                    //reduce colors to true / false                
                    lResult.Add(bmpMin.GetPixel(i, j).GetBrightness() < 0.5f);
                }
            }
            return lResult;
        }
        public static Dictionary<string, string> GetModelStateErrors(this ModelStateDictionary modelState)
        {
            var ErrorsList = new Dictionary<string, string>();

            for (int i = 0; i < modelState.Keys.ToList().Count(); i++)
            {
                if (modelState.Values.ToArray()[i].Errors.Any())
                {
                    var ErrorMessage = modelState.Values.ToArray()[i].Errors.FirstOrDefault()?.ErrorMessage;
                    ErrorsList.Add(modelState.Keys.ToArray()[i], ErrorMessage);
                }
            }
            return ErrorsList;
        }
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException("Null request");
            }

            if (request.Headers != null)
            {
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }

            return false;
        }
        public static LoggedinUser GetUser(this HttpContext context)
        {
            return (LoggedinUser)context.Items["User"];
        }

        public static double ToRadians(this double val)
        {
            return (Math.PI / 180) * val;
        }
        public static double ToDegrees(this double val)
        {
            return (180 / Math.PI) * val;
        }
        public static string ConvertDateTimeToString(this DateTime dateTime)
        {
           return dateTime.ToString("dd-MM-yyyy");
        }

    }
}
