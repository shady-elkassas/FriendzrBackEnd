using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
   public interface IMessageServes
    {
        (int TotalCount, IQueryable<Messagedata> messagedatas) GetUserMessageData(string CurrentUserID, string OtherUserID, PaginationFilter validFilter);
        Task<bool> CheckUserMessages(int currentuserid, int antherid);
        Task<string> addUserMessages(UserMessages UserMessages,bool firstone=true);
       // Task addEventChat(EventChat EventChat);
        Task<MessageVIEWDTO> addusermessage(MessageDTO MessageDTO, UserDetails userDeatils, string usermessid);
         Task<string>  addMessagedata(Messagedata Messagedata);
        bool CheckUserMessagescolor(int currentuserid, string messagedataid);
        UserMessages getUserMessages(int currentuserid, int antherid,bool message=false);
        List<Messagedata> getallMessagedata(string messagedataid);
        List<Messagedata> geteventMessagedata(string UserMessagesid);
        bool getUserMessages(string UserMessagesid, string userid);

        //string geteventMessages(string eventid);
       // EventChat getEventChat(string eventid);
        FireBaseDatamodel getFireBaseData(int userid, FireBaseData model,DateTime? date=null,TimeSpan?time=null,bool IsCreatedByAdmin = false);
        Task addFireBaseDatamodel(FireBaseDatamodel model);
        Task addFireBaseDatamodel(List<FireBaseDatamodel> model);
        List<FireBaseDatamodel> getFireBaseDatamodel(int userid);
        int getFireBasecount(int userid);


       Task<  List<UserDetailsvm>> getalllUserinconect(int currentuserid, string curen, string search=null);
       List<UserDetailsvm> getalllUserinconect_2(int currentuserid, string curen);
        Task<MessageVIEWDTO> addeventmessage(EventMessageDTO MessageDTO, UserDetails userDeatils);
        UserMessages GetUserMessages(string currentuserid, string antherid);
            Task updateUserMessages(UserMessages UserMessages);
        Task deleteeventmessage(string userid, string eventid);
        int messagelogincount(string userid);
    }
}
