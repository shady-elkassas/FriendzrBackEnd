using Microsoft.AspNetCore.Mvc;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Helpers;
using Social.Services.ModelView;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using static Social.Services.Implementation.EventServ;

namespace Social.Services.Services
{
    public interface IEventServ
    {
        Task InsertEventColor(EventColor code);
        Task<EventData> InsertEvent(EventData code);
        Task updateEvent(EventData code);
        EventDataadminMV GetEventDataDetails(int Id);

        EventChatAttend GetEventChatAttendbyid(string id);
        // Task InsertEventChatAttend(EventChatAttend code);
        Task<EventChatAttend> InsertEventChatAttend(EventChatAttend code);
        bool CHECKChatAttend(EventChatAttend code);
        Task editeEventChatAttend(EventChatAttend code);

        //Task updateeventattend(EventChatAttend code);
        List<listoftags> GetINterestdata(int id);
        Task updatecategory(category code);
        Task Insertcategory(category code);
        Task deletecategory(string id);
        List<interlistdata> GetEventattendstat(string id);
        List<listdata> GetEventattendgender(string id);
        EventData GetEventbyid(string id);
        EventData GeteventbyPrimaryId(string id);
        category Getcategorybyid(string id);
        int GetEventattend(string id);
        int GetEventattend(string id, List<EventChatAttend> eventattend);
        (int count, bool type) GetEventattend(List<EventChatAttend> EventChatAttend, string id, int userid);
        //List< INterests> GetINterestdata(string id);
        EventChatAttend GetuserEvent(string id, string userid);
        Task deleteEvent(string id);
        EventChatAttend GetEventChatAttend(string id, string userid, bool message = false);
        Task InsertInterests(Interests code);
        Task updateInterests(Interests code);
        Task deleteInterests(string id);
        Interests getInterests(string id);
        WhatBestDescripsMe getWhatBestDescripsMe(string id);
        preferto getpreferto(string id);
        List<Interests> getInterests();
        bool priveteventlink(string id, String userid);
        bool priveteventlink(string id);
        List<EventColor> getEventColor();
        List<category> getcategory();
        List<EventData> getmyevent(string id);
        List<EventData> getallexternalevent();
        List<EventData> getallevent();
        List<EventData> getallevent(int id);
        List<EventData> getalluserevent(int id);
        List<EventData> getevent(string id);
        IEnumerable<EventChatAttend> getattendevent(string id);
        IEnumerable<EventChatAttend> getattendevent(string id, string userid);
        List<EventChatAttend> getEventChatAttend(string id, String userid);
        IEnumerable<EventChatAttend> allattendevent();
        List<EventChatAttend> allEventChatAttend();
        List<EventChatAttend> AllEventChatAttendByEventId(string eventId);
        List<EventChatAttend> getallEventChatAttend(string id, IEnumerable<EventChatAttend> data);
        IEnumerable<EventChatAttend> allattendevent(int id, string Search = null);

        public List<EventChatAttend> getallChatattendevent(string id, IEnumerable<EventChatAttend> data);
        List<EventChatAttend> getalluserevent(int id, List<EventChatAttend> data);


        List<EventVM> getallattendevent(IEnumerable<EventChatAttend> allateend, int PrimaryId, List<EventData> pagedModel);

        List<EventChatAttend> getallattendevent(string id);
        List<EventData> geteventbylocation(string lang, string lat);
        locationDataMV getAlleventlocation(int pageNumber, int pageSize, int id, double myLat, double myLon, double dis, string Gender, UserDetails user, AppConfigrationVM AppConfigrationVM , string categories);
        Task<(List<EventVM>, int totalRowCount)> getAlleventlocation_2(int id, double myLat, double myLon, double dis, string Gender, UserDetails user, AppConfigrationVM AppConfigrationVM, string categories, int pageNumber, int pageSize);

        Task<(List<EventVM>, int totalRowCount)> getAlleventlocationWithDateFilter(int id, double myLat, double myLon,
            double dis, string Gender, UserDetails user, AppConfigrationVM AppConfigrationVM, string categories,
            int pageNumber, int pageSize, string dateCriteria, DateTime? startDate, DateTime? endDate);
        peoplocationDataMV getUserDetailsbylocation(string lang, string lat);


        List<EventlocationDataMV> getAlleventlocation();
        IEnumerable<ValidationResult> _ValidationResult(EventDataadminMV VM);
        Task<CommonResponse<EventDataadminMV>> ToggleActiveConfigration(string ID, bool IsActive);
        Task<CommonResponse<EventDataadminMV>> ToggleActiveConfigrationByPrimaryKey(string ID, bool IsActive);


        EventDataadminMV GetData(string ID);
        IEnumerable<EventDataadminMV> GetData();
        IEnumerable<EventDataadminMV> GetData(Expression<Func<EventData, bool>> predicate);
        Task<CommonResponse<List<int>>> Create(EventDataadminMV VM);
        Task<bool> Createrang(EventData Obj);
        Task<bool> Createrang(List<EventData> Obj);
        Task<CommonResponse<EventDataadminMV>> Edit(EventDataadminMV VM);
        Task<CommonResponse<EventDataadminMV>> Remove(string ID);
        Task<CommonResponse<EventDataadminMV>> RemoveEventById(string id);

        Task<(RecommendedEventViewModel,string)> RecommendedEvent(UserDetails userDeatil, string eventId,  bool? previous);
        
        #region  public
        locationDataMV GetAllEventLocations(int pageNumber, int pageSize, double myLat, double myLon, AppConfigrationVM AppConfigrationVM , string categories);
        Task<(List<EventVM> eventData, int totalRowCount)> GetAllEventLocations_2(double myLat, double myLon, AppConfigrationVM AppConfigrationVM , string categories, int pageNumber, int pageSize);
        Task<List<EventData>> InsertEvents(List<EventData> eventsData);
        Task<bool> InsertFavoriteEvent(int userId, string eventId);
        Task<bool> DeleteFavoriteEvent(int userId, string eventId);

        Task<(List<GetFavoriteEventsDto> events, int totalCount, int PagesCount)> GetFavoriteEvents(int userId,
            int pageSize, int pageNumber);        bool CheckFavoriteEvent(int userId, string eventId);
        Task<List<EventChatAttend>> InsertEventChatAttends(List<EventChatAttend> eventChatAttends);
        void UpdateExistEvent (EventData eventExist, AddExternalEventDataModel newEventData, int eventType);
        int ExtractEventCategory(string evntCategory);
        Task UpdateEventsAddressFromGoogle(List<EventData> eventsData);
        Task UpdateEventAddressFromGoogle(EventData eventData);
        IQueryable<EventDataadminMV> GetWhiteLableEvents(PaginationFilter PaginationFilter,int PrimaryId,int? Search_EventTypeListID, int? Search_EventCategoryID, out int filteredCount);
        #endregion

        IEnumerable<EventChatAttend> GetValidChatAttends(int eventId);       
        
        IQueryable<EventDataadminMV> GetData(PaginationFilter PaginationFilter, int? Search_EventTypeListID, int? Search_EventCategoryID, out int filteredCount);
        locationDataMV getAlleventUserlocations(int pageNumber, int pageSize, UserDetails user, AppConfigrationVM AppConfigrationVM, string categories);

        locationDataMV GetAllEventsUserLocationsWithDateFilter(int pageNumber, int pageSize, UserDetails user,
            AppConfigrationVM AppConfigrationVM, string categories, string dateCriteria, DateTime? startDate,
            DateTime? endDate);

        locationDataMV GetEventsLocationsWithDateFilter(int pageNumber, int pageSize, UserDetails user,
            AppConfigrationVM AppConfigrationVM, string categories, string dateCriteria, DateTime? startDate,
            DateTime? endDate);

        List<EventDataByLocationMV> GetAllEventsByLocationsWithDateFilter(string eventLang, string eventLat, UserDetails user,
            AppConfigrationVM AppConfigrationVM, string categories, string dateCriteria, DateTime? startDate,
            DateTime? endDate);

        //Task<List<EventDataTicketMaster>> InsertTicketMasterEvents(List<EventDataTicketMaster> eventsData);
    }
}
