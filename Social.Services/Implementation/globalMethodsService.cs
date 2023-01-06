
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Social.Sercices;
using Social.Services.Helpers;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Social.Services.Implementation
{
    public class globalMethodsService : IGlobalMethodsService
    {
        private readonly IConfiguration configuration;
        private readonly string[] acceptedetentions;
        public IHttpContextAccessor _HttpContextAccessor { get; }
        public IWebHostEnvironment _HostingEnvironment { get; }


        public globalMethodsService(IHttpContextAccessor HttpContextAccessor, IConfiguration configuration, IWebHostEnvironment HostingEnvironment)
        {

            _HttpContextAccessor = HttpContextAccessor;
            this.configuration = configuration;
            _HostingEnvironment = HostingEnvironment;
            acceptedetentions = new string[] { ".jfif", ".pptx", ".webp", ".jpg", ".svg", ".jpeg", ".png", ".PNG", ".gif", ".docx", ".doc", ".pdf", ".PDF", ".txt", ".text", ".Word", ".ppt", ".jpg", ".jpeg", ".png", ".PNG", ".gif", ".docx", ".doc", ".pdf", ".PDF", ".txt", ".text", ".Word", ".ppt", ".pptx", ".xlsx", ".odt", ".xls ", ".xps",".ico" };


        }

        public int getUserIdFromCookie()
        {
            var user_cookie = _HttpContextAccessor.HttpContext.Request.Cookies["User_Id"];
            var userid = Convert.ToInt32(StringCipher.TryDecryptString(user_cookie));
            return userid;
        }
        public void DeleteFiles(string PlaceOnServer, string fileName)
        {
            try
            {
                string DeletetedfilePathpath = _HostingEnvironment.WebRootPath + PlaceOnServer + fileName;
                if (File.Exists(DeletetedfilePathpath)) File.Delete(DeletetedfilePathpath);
            }
            catch { }
        }
        public void DeleteFiles(string PlaceOnServer, List<string> fileName)
        {
            foreach (var item in fileName)
            {
                string DeletetedfilePathpath = _HostingEnvironment.WebRootPath + PlaceOnServer + item;
                if (File.Exists(DeletetedfilePathpath)) File.Delete(DeletetedfilePathpath);
            }
        }
        public async Task<string> uploadFileFromUrl(string PlaceOnServer, string fileUrl)
        {

            if (fileUrl == null || fileUrl == "") return null;
            string filename = System.IO.Path.GetFileName(fileUrl);


            var Path = fileUrl;
            using (HttpClient client = new HttpClient())
            {
                //client.DefaultRequestHeaders.Add("authorization", access_token); //if any
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(fileUrl);

                if (response.IsSuccessStatusCode)
                {

                    HttpContent content = response.Content;


                    var contentStream = await content.ReadAsStreamAsync(); // get the actual content stream



                    string ImagePath = _HostingEnvironment.WebRootPath + PlaceOnServer + filename;

                    using (var FileStream = new FileStream(ImagePath, FileMode.Create))
                    {

                        await contentStream.CopyToAsync(FileStream);
                    }
                }
                else
                {
                    filename = null;
                }
            }
            return filename;


        }

        public string uploadFile(string PlaceOnServer, IFormFile file)
        {
            if (file == null) return null;
            //Check If Image AleradyExist Befor In Server 
            else if (File.Exists(_HostingEnvironment.WebRootPath + PlaceOnServer + file.FileName))
            {
                return file.FileName;
            }
            else
            {
                var checkimageexist =  isExistimage(file, PlaceOnServer).Result;

                if (checkimageexist.isexist == true && string.IsNullOrEmpty(checkimageexist.imagename) == false)
                {
                    return checkimageexist.imagename;
                }
                else
                {
                    var PlaceOnServerUrl = _HostingEnvironment.WebRootPath + PlaceOnServer;
                    if (!Directory.Exists(PlaceOnServerUrl))
                    {
                        Directory.CreateDirectory(PlaceOnServerUrl);
                    }
                    string extention = Path.GetExtension(file.FileName);

                    string UniqImageName = Guid.NewGuid().ToString() + "_" + Regex.Replace(file.FileName, @"[^0-9a-zA-Z_]+", "") + extention;
                    if (UniqImageName.Length > 100)
                    {
                        UniqImageName = Guid.NewGuid().ToString() + "_NewName" + extention;
                    }
                    Task.Run(() =>
                    {

                        string ImagePath = _HostingEnvironment.WebRootPath + PlaceOnServer + UniqImageName;

                        using (var FileStream = new FileStream(ImagePath, FileMode.Create))
                        {
                            file.CopyTo(FileStream);
                            //FileStream.FlushAsync();
                        }
                    });
                    return UniqImageName;
                }
            }
        }
        public async Task uploadFile(string PlaceOnServer, string Json)
        {



            var PlaceOnServerUrl = _HostingEnvironment.WebRootPath + PlaceOnServer;
            if (!Directory.Exists(PlaceOnServerUrl))
            {
                Directory.CreateDirectory(PlaceOnServerUrl);
            }

            string UniqImageName = Guid.NewGuid().ToString() + "_" + ".json";


            string filePath = _HostingEnvironment.WebRootPath + PlaceOnServer + UniqImageName;

            if (!File.Exists(filePath))
            {

                using (var f = File.Create(filePath))
                {
                    f.Close();
                    f.Dispose();
                }
            }


            await System.IO.File.WriteAllTextAsync(filePath, Json);



        }
        public async Task<string> uploadFileAsync(string PlaceOnServer, IFormFile file)
        {
            if (file == null) return null;
            //Check If Image AleradyExist Befor In Server 
            else if (File.Exists(_HostingEnvironment.WebRootPath + PlaceOnServer + file.FileName))
            {
                return file.FileName;
            }
            else
            {
                //var checkimageexist = await isExistimage(file, PlaceOnServer);

                //if (checkimageexist.isexist==true&&string.IsNullOrEmpty(checkimageexist.imagename)==false)
                //{
                //    return checkimageexist.imagename;
                //}
                //else
                {
                    var PlaceOnServerUrl = _HostingEnvironment.WebRootPath + PlaceOnServer;
                    if (!Directory.Exists(PlaceOnServerUrl))
                    {
                        Directory.CreateDirectory(PlaceOnServerUrl);
                    }
                    string extention = Path.GetExtension(file.FileName);
                    if (acceptedetentions.Contains(extention.ToLower()) == false&& acceptedetentions.Contains(extention.ToUpper()) == false&& acceptedetentions.Contains(extention) == false)
                    {
                        throw new Exception("Not Accepted File Extention");
                    }
                    string UniqImageName = Guid.NewGuid().ToString() + "_" + Regex.Replace(file.FileName, @"[^0-9a-zA-Z_]+", "") + extention;
                    if (UniqImageName.Length > 100)
                    {
                        UniqImageName = Guid.NewGuid().ToString() + "_NewName" + extention;
                    }
                    string ImagePath = _HostingEnvironment.WebRootPath + PlaceOnServer + UniqImageName;
                    using (var FileStream = new FileStream(ImagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(FileStream);
                    }
                    return UniqImageName;
                }
            }
        }
        public async Task<string> adminuploadFileAsync(string PlaceOnServer, IFormFile file)
        {
            if (file == null) return null;
            //Check If Image AleradyExist Befor In Server 
            else if (File.Exists(_HostingEnvironment.WebRootPath + PlaceOnServer + file.FileName))
            {
                return file.FileName;
            }
            else
            {
                //var checkimageexist = await isExistimage(file, PlaceOnServer);

                //if (checkimageexist.isexist==true&&string.IsNullOrEmpty(checkimageexist.imagename)==false)
                //{
                //    return checkimageexist.imagename;
                //}
                //else
                {
                    var PlaceOnServerUrl = _HostingEnvironment.WebRootPath + PlaceOnServer;
                    if (!Directory.Exists(PlaceOnServerUrl))
                    {
                        Directory.CreateDirectory(PlaceOnServerUrl);
                    }
                    string extention = Path.GetExtension(file.FileName);
                   
                    string UniqImageName = Guid.NewGuid().ToString() + "_" + Regex.Replace(Path.GetFileNameWithoutExtension(file.FileName), @"[^0-9a-zA-Z_]+", "") + extention;
                    if (UniqImageName.Length > 100)
                    {
                        UniqImageName = Guid.NewGuid().ToString() + "_NewName" + extention;
                    }
                    string ImagePath = _HostingEnvironment.WebRootPath + PlaceOnServer + UniqImageName;
                    using (var FileStream = new FileStream(ImagePath, FileMode.Create))
                    {
                        await file.CopyToAsync(FileStream);
                    }
                    return UniqImageName;
                }
            }
        }
        public void addCookie(string key, string value, DateTime? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = expireTime;
            else
                option.Expires = DateTime.Now.AddMinutes(20);

            _HttpContextAccessor.HttpContext.Response.Cookies.Append(key, value, option);
        }



        public string ConvertNumerals(string input)
        {
            System.Text.UTF8Encoding utf8Encoder = new UTF8Encoding();
            System.Text.Decoder utf8Decoder = utf8Encoder.GetDecoder();
            System.Text.StringBuilder convertedChars = new System.Text.StringBuilder();
            char[] convertedChar = new char[1];
            byte[] bytes = new byte[] { 217, 160 };
            char[] inputCharArray = input.ToCharArray();
            foreach (char c in inputCharArray)
            {
                if (char.IsDigit(c))
                {
                    bytes[1] = Convert.ToByte(160 + char.GetNumericValue(c));
                    utf8Decoder.GetChars(bytes, 0, 2, convertedChar, 0);
                    convertedChars.Append(convertedChar[0]);
                }
                else
                {
                    convertedChars.Append(c);
                }
            }
            return convertedChars.ToString();

        }
        public List<string> getmonthList()
        {
            List<string> monthList = new List<string>();
            for (int i = 12; i >= 1; i--)
            {

                string month = new DateTime(2015, i, 1).ToString("MMM");


                monthList.Add(month);
            }
            return monthList;
        }

        public object GetJsonObjectData(string url, ref object recordToFill)
        {
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{url}");
            var JSON = System.IO.File.ReadAllText(folderDetails);
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(JSON);
            if (jsonObject != null)
            {
                foreach (PropertyInfo property in recordToFill.GetType().GetProperties())
                {
                    var prop = recordToFill.GetType().GetProperty(property.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (prop != null && prop.CanWrite)
                    {
                        prop.SetValue(recordToFill, jsonObject[property.Name], null);
                    }
                }
            }
            return recordToFill;
        }
        public string GetJsonProperityData(string url, string properity)
        {
            var folderDetails = Path.Combine(Directory.GetCurrentDirectory(), $"wwwroot\\{url}");
            var JSON = System.IO.File.ReadAllText(folderDetails);
            dynamic jsonObject = Newtonsoft.Json.JsonConvert.DeserializeObject(JSON);
            if (jsonObject != null)
            {
                return jsonObject[properity].ToString();
            }
            return "";
        }

        public string GetBaseDomain()
        {
            var request = _HttpContextAccessor.HttpContext?.Request;
            if (request == null)
            {
                return configuration["BaseUrl"];
            }
            var url = $"{request?.Scheme}://{request?.Host}{request?.PathBase}";
            return url;
        }
        bool IsSameImage(Bitmap img1, Bitmap img2)
        {
            bool result=true;
            int samepixcount = 0, diffrentpixcount = 0;

            for (int i = 0; i < img2.Width; i++)
                    {
                        for (int j = 0; j < img2.Height; j++)
                        {
                            var img2_ref = img2.GetPixel(i, j).ToString();
                           var img1_ref = img1.GetPixel(i, j).ToString();
                            if (img2_ref != img1_ref)
                            {
                                diffrentpixcount++;
                                result = false;
                                break;
                            }
                            samepixcount++;
                        }
                    }
            return result;
        }
        async Task<(bool isexist,string imagename)> isExistimage(IFormFile img, string PlaceOnServer)
        {
            var result = false;
            string imagename = null;
            Bitmap img1;
            using (var memoryStream = new MemoryStream())
            {
                await img.CopyToAsync(memoryStream);
                using (var im = Image.FromStream(memoryStream))
                {
                    img1 = new Bitmap(im);
                }
            }
            var files = Directory.GetFiles(_HostingEnvironment.WebRootPath + PlaceOnServer);
            foreach (var item in files)
            {
                try
                {
                    var oldimage = new Bitmap(item);
                    result= IsSameImage(img1, oldimage);
                    if (result == true)
                    {
                        imagename = Path.GetFileName(item);
                        break;
                    }
                }
                catch (Exception ex)
                {

                }
            }
            return (result,imagename);
        }
    }
}
