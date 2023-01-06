using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Social.Services.DateTimeModelBinderProvider
{
    public class Helper
    {
        public static DateTime? ParseDateTime(
            string dateToParse,
            IFormatProvider provider,
            string[] formats = null,
            DateTimeStyles styles = DateTimeStyles.None)
        {
            var CUSTOM_DATE_FORMATS = new string[]
                {  
                    "dd-MM-yyyy",
                    "d-M-yyyy",
                    "MM-dd-yyyy",
                    "yyyy-MM-dd",
                       "dd-MM-yyyy HH:mm:ss",
                    "MM-dd-yyyy HH:mm:ss",
                    "yyyy-MM-dd HH:mm:ss",
            //"MM-dd-yyyy",
            // "yyyyMMddTHHmmssZ",
            //    "yyyyMMddTHHmmZ",
            //    "yyyyMMddTHHmmss",
            //    "yyyyMMddTHHmm",
            //    "yyyyMMddHHmmss",
            //    "yyyyMMddHHmm",
            //    "yyyyMMdd",
            //    "yyyy-MM-ddTHH-mm-ss",
            //    "yyyy-MM-dd-HH-mm-ss",
            //    "yyyy-MM-dd-HH-mm",
            //    "yyyy-MM-dd",
            //    "MM-dd-yyyy",
            //"yyyy-mm-dd",
            //"dd/MM/yyyy"
                };

            if (formats == null || !formats.Any())
            {
                formats = CUSTOM_DATE_FORMATS;
            }
            
            DateTime validDate;
    
            foreach (var format in formats)
            {
                if (format.EndsWith("Z"))
                {
                    if (DateTime.TryParseExact(dateToParse, format,
                             provider,
                             DateTimeStyles.AssumeUniversal,
                             out validDate))
                    {
                        return validDate;
                    }
                }

                if (DateTime.TryParseExact(dateToParse, format,
                         provider, styles, out validDate))
                {
                    return validDate;
                }
                //dateToParse;
            }
            
            return null;
        }
    }
}
