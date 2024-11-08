﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Helpers
{
    public class ExternalEventUtility
    {
        private readonly IConfiguration _configuration;

        private string apiKey { get; set; }
        private string baseUri { get; set; }
        private string apiMasterKey { get; set; }
        private string baseMasterUri { get; set; }


        public ExternalEventUtility(IConfiguration configuration)
        {
            _configuration = configuration;
            apiKey = _configuration.GetValue<string>("SkiddleApi:ApiKey");
            apiMasterKey = _configuration.GetValue<string>("TicketMaster:ApiKey");
            baseUri = _configuration.GetValue<string>("SkiddleApi:BaseUrl");
            baseMasterUri = _configuration.GetValue<string>("TicketMaster:BaseUrl");
        }

        
        // for job call
        public async Task<ExternalEventDataResponse> GetExternalEvents(int totalCount, string? minDate, string? maxDate)
        {           
            var events = await GetAllEvents(totalCount,minDate,maxDate);
            events = events.FindAll(e => e.cancelled == 0);
            events.ForEach(e => e.categorieId = e.EventCode.GetCategoryId());
            events.ForEach(e => e.SubCategoriesIds = e.EventCode.GetSubCategoriesIds());
            events = events.FindAll(e => e.categorieId != 0);

            return new ExternalEventDataResponse {ExternalEventData=events, TotalCount = events.Count,IsJob=true}; 
        }

        /// <summary>
        /// Ticket Master
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        public async Task<ExternalEventTaskMasterResponse> GetExternalTiceketMasterEvents(int totalCount, string? minDate, string? maxDate)
        {
            var events = await GetAllEventsTicketMaster(totalCount, minDate, maxDate);
           // events = events.FindAll(x => x.results.FindAll(r => r.dates.status.code.ToLower() != "canceled"));

            foreach (var eve in events)
            {

                events = events.FindAll(x=> x.dates.status.code.ToLower()!="canceled");
                
            }


            //events.ForEach(e => e.categorieId = e.EventCode.GetCategoryId());
            //events.ForEach(e => e.SubCategoriesIds = e.EventCode.GetSubCategoriesIds());
            //events = events.FindAll(e => e.categorieId != 0);

            return new ExternalEventTaskMasterResponse { ExternalEventDataForTciketMaster = events, TotalCount = events.Count, IsJob = true };
        }

        public async Task<ExternalEventDataResponse> GetEvents()
        {
            var limit = 100;           
            var baseAddress = new Uri(baseUri);
            var externalEvents = new List<ExternalEventData>();
            var responseCount = 0;
            var offset = 0;

            try
            {
                //do
                //{
                    var httpClient = new HttpClient { BaseAddress = baseAddress };
             
                var response = await httpClient.GetAsync($"?api_key={apiKey}&limit={limit}&offset={offset}");

                    var responseHeaders = response.Headers.ToString();
                    var result = await response.Content.ReadAsStringAsync();
                    var status = (int)response.StatusCode;
                    var headers = responseHeaders;
                    var responseData = JsonConvert.DeserializeObject<ExternalEventDataModel>(result);
                    responseCount = responseData.totalcount;
                    externalEvents.AddRange(responseData.results);
                    offset += limit;

                //} while (externalEvents.Count != responseCount);
                externalEvents = externalEvents.FindAll(e => e.cancelled == 0);
                externalEvents.ForEach(e => e.categorieId = e.EventCode.GetCategoryId());
                externalEvents.ForEach(e => e.SubCategoriesIds = e.EventCode.GetSubCategoriesIds());
                externalEvents = externalEvents.FindAll(e => e.categorieId != 0);
                return new ExternalEventDataResponse {  ExternalEventData= externalEvents, TotalCount= responseCount,IsJob=false};


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }



        /// <summary>
        /// TicketMaster GET EVEnt
        /// </summary>
        /// <returns></returns>
        public async Task<ExternalEventTaskMasterResponse> GetTicketmasterEvents()
        {
            var limit = 100;
            var baseAddress = new Uri(baseUri);
            var externalEvents = new List<Event>();
            var responseCount = 0;
            var offset = 0;

            try
            {
                //do
                //{
                var httpClient = new HttpClient { BaseAddress = baseAddress };

                var response = await httpClient.GetAsync($"?api_key={apiMasterKey}&limit={limit}&offset={offset}");

                var responseHeaders = response.Headers.ToString();
                var result = await response.Content.ReadAsStringAsync();
                var status = (int)response.StatusCode;
                var headers = responseHeaders;
                var responseData = JsonConvert.DeserializeObject<ExternalEventTicketMasterDataModel>(result);
                responseCount = responseData.totalcount;
                externalEvents.AddRange(responseData.results);
                offset += limit;

                //} while (externalEvents.Count != responseCount);
                //externalEvents = externalEvents.FindAll(e => e.cancelled == 0);
                //externalEvents.ForEach(e => e.categorieId = e.EventCode.GetCategoryId());
                //externalEvents.ForEach(e => e.SubCategoriesIds = e.EventCode.GetSubCategoriesIds());
                //externalEvents = externalEvents.FindAll(e => e.categorieId != 0);
                return new ExternalEventTaskMasterResponse { ExternalEventDataForTciketMaster= externalEvents, TotalCount = responseCount, IsJob = false };


            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        private async Task<List<ExternalEventData>> GetAllEvents(int totalCount, string? minDate, string? maxDate)
        {
            var limit = 100;
            var offset = 0;
            var baseAddress = new Uri(baseUri);
            var externalEvents = new List<ExternalEventData>();
            var responseCount = 0;
            if (totalCount > 0 && totalCount <= 10000)
            {
                responseCount = totalCount;
            }
            else if (totalCount > 10000)
                throw new Exception("Exceeded the specified number");
            try
            {                
                do
                {
                    //GetAsync($"?api_key={apiKey}&limit={limit}&offset={offset}&ticketsavailable=1&country=GB&minDate=2022-12-18&maxDate=2023-01-18");
                    var httpClient = new HttpClient { BaseAddress = baseAddress };
                    //https://www.skiddle.com/api/v1/events/search/?api_key=c9860a34e7d70d84048f8d020b92ed84&minDate=2022-12-18&maxDate=2023-01-18&ticketsavailable=1&country=GB
                    var response = await httpClient.GetAsync($"?api_key={apiKey}&limit={limit}&offset={offset}&ticketsavailable=1&country=GB&minDate={minDate}&maxDate={maxDate}");
                   // var response = await httpClient.GetAsync($"?api_key={apiKey}&limit={limit}&offset={offset}&ticketsavailable=1&country=GB&minDate=2022-12-18&maxDate=2023-01-18");
                    var responseHeaders = response.Headers.ToString();
                    var result = await response.Content.ReadAsStringAsync();
                    var status = (int)response.StatusCode;
                    var headers = responseHeaders;
                    var responseData = JsonConvert.DeserializeObject<ExternalEventDataModel>(result);
                    if(responseCount==0)
                    {
                        responseCount = responseData.totalcount;
                    }                    
                    externalEvents.AddRange(responseData.results);
                    offset += limit;

                } while (externalEvents.Count != responseCount);               
                 
                 return externalEvents;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


        /// <summary>
        /// Tikcet Master
        /// </summary>
        /// <param name="totalCount"></param>
        /// <param name="minDate"></param>
        /// <param name="maxDate"></param>
        /// <returns></returns>
        private async Task<List<Event>> GetAllEventsTicketMaster(int totalCount, string? minDate, string? maxDate)
        {
            var limit = 100;
            var offset = 0;
            var baseAddress = new Uri(baseMasterUri);
            var externalEvents = new List<Event>();
            var responseCount = 0;
            if (totalCount > 0 && totalCount <= 10000)
            {
                responseCount = totalCount;
            }
            else if (totalCount > 10000)
                throw new Exception("Exceeded the specified number");
            try
            {
                do
                {
                    var httpClient = new HttpClient { BaseAddress = baseAddress };
                    var response = await httpClient.GetAsync($"?apikey={apiMasterKey}&minDate={minDate}&maxDate={maxDate}");
                    var responseHeaders = response.Headers.ToString();
                    var result = await response.Content.ReadAsStringAsync();
                    var status = (int)response.StatusCode;
                    var headers = responseHeaders;
                    //deserilize main response 
                    var responseData = JsonConvert.DeserializeObject<Event>(result);
                    if (responseCount == 0)
                    {
                        ///Total count can not get total count 
                        responseCount = responseData._embedded.events.Count();
                    }
                    externalEvents.AddRange(responseData._embedded.events);
                    offset += limit;

                } while (externalEvents.Count != responseCount);

                return externalEvents;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }


    }

   
    public class ExternalEventDataResponse
    {
        public int TotalCount { get; set; }
        public List<ExternalEventData> ExternalEventData { get;set;}
        //public List<Event> ExternalEventDataForTciketMaster{ get;set;}
        public bool IsJob { get; set; }

    }
    public class ExternalEventTaskMasterResponse
    {
        public int TotalCount { get; set; }
        public List<Event> ExternalEventDataForTciketMaster { get; set; }
        public bool IsJob { get; set; }
    }
}
