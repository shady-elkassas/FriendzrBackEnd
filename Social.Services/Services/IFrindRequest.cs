using Social.Entity.Enums;
using Social.Entity.Models;
using Social.Entity.ModelView;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IFrindRequest
    {
        Task addrequest(Requestes code);
        Task updaterequest(Requestes code);
        Task deleterequest(Requestes code);
        Task requestresponse(Requestes code);
        int Getallkey(int userid, int requserid, List<Requestes> reqs);
        int GetallkeyForFeed(int userid, int requserid);
        Requestes GetReque(int userid, int requserid);
        int Getkey(int userid, int requserid);
        bool Getboolkey(int userid, int requserid);

        IQueryable<Requestes> GetallRequestes(int userid,RequestesType requestesType, string search=null);
        List<Requestes> GetallFrendes(int userid,string search);
        IQueryable<Requestes> GetallBlock(int userid,string search=null);
    }
}
