using Social.Entity.DBContext;
using Social.Entity.Models;
using Social.Entity.ModelView;
using Social.Services.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Diagnostics;
using Social.Entity.Enums;

namespace Social.Services.Implementation
{
    public class FrindRequest : IFrindRequest
    {
        public AuthDBContext _authContext;

        public FrindRequest(AuthDBContext authContext)
        {
            this._authContext = authContext;
        }
        public async Task addrequest(Requestes code)
        {
            code.EntityId = Guid.NewGuid().ToString();
            await this._authContext.Requestes.AddAsync(code);
            await this._authContext.SaveChangesAsync();

        }

        public Requestes GetReque(int userid, int requserid)
        {
            var regest = _authContext.Requestes.Where(m => (m.UserId == userid && m.UserRequestId == requserid) || (m.UserId == requserid && m.UserRequestId == userid)).FirstOrDefault();
            return regest;
        }
        public int Getallkey(int userid, int requserid, List<Requestes> reqs)
        {
            //var regest = _authContext.Requestes.Where(m => (m.UserId == requserid || m.UserRequestId == userid)).FirstOrDefault();
            var regest = reqs.FirstOrDefault(m => (m.UserId == userid && m.UserRequestId == requserid) || (m.UserId == requserid && m.UserRequestId == userid));

            return regest switch
            {
                var req when (regest is null) => 0,
                var req when (regest.UserId == userid && regest.status == 0) => 1,
                var req when (regest.UserRequestId == userid && regest.status == 0) => 2,
                var req when (regest.status == 1) => 3,
                var req when (regest.UserblockId == userid && regest.status == 2) => 4,
                var req when (regest.UserblockId == requserid && regest.status == 2) => 5,
                _ => 0
            };
        }

        public int GetallkeyForFeed(int userid, int requserid)
        {
            //var regest = _authContext.Requestes.Where(m => (m.UserId == requserid || m.UserRequestId == userid)).FirstOrDefault();
            var regest = _authContext.Requestes.FirstOrDefault(m => (m.UserId == userid && m.UserRequestId == requserid) || (m.UserId == requserid && m.UserRequestId == userid));

            return regest switch
            {
                var req when (regest is null) => 0,
                var req when (regest.UserId == userid && regest.status == 0) => 1,
                var req when (regest.UserRequestId == userid && regest.status == 0) => 2,
                var req when (regest.status == 1) => 3,
                var req when (regest.UserblockId == userid && regest.status == 2) => 4,
                var req when (regest.UserblockId == requserid && regest.status == 2) => 5,
                _ => 0
            };
        }
        public int Getkey(int userid, int requserid)
        {
            int key = 0;
            var regest = _authContext.Requestes.Where(m => (m.UserId == requserid || m.UserRequestId == userid)).FirstOrDefault();
            if (regest != null)
            {
                if (regest.UserId == userid && regest.status == 0)
                {
                    key = 1;
                }
                else if (regest.UserRequestId == userid && regest.status == 0)
                {
                    key = 2;
                }
                else if (regest.status == 1)
                {
                    key = 3;
                }
                else if (regest.UserblockId == userid && regest.status == 2)
                {
                    key = 4;
                }
                else if (regest.UserblockId == requserid && regest.status == 2)
                {
                    key = 5;
                }

            }
            return key;
        }
        public bool Getboolkey(int userid, int requserid)
        {
            bool key = false;
            var regest = _authContext.Requestes.Where(m => (m.UserId == requserid && m.UserRequestId == userid)).FirstOrDefault();
            if (regest != null)
            {
                if (regest.UserId == userid && regest.status == 0)
                {
                    key = true;
                }
                else if (regest.UserRequestId == userid && regest.status == 0)
                {
                    key = true;
                }
                else if (regest.status == 1)
                {
                    key = true;
                }
                else if (regest.UserblockId == userid && regest.status == 2)
                {
                    key = false;
                }
                else if (regest.UserblockId == requserid && regest.status == 2)
                {
                    key = false;
                }

            }
            return key;
        }
        public IQueryable<Requestes> GetallRequestes(int userid, RequestesType requestesType, string search = null)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;
            if (requestesType == RequestesType.RecivedOnly)
            {

            var regest = _authContext.Requestes.Include(q => q.User).ThenInclude(q => q.listoftags).Where(m => m.UserRequestId == userid && m.status != 2 && (search == null || search == "" || DbF.Like(m.User.User.DisplayedUserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")/*|| DbF.Like(m.User.User.UserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")*/));

            return regest.OrderByDescending(m=>m.regestdata);
            }
            else if (requestesType == RequestesType.SentOnly)
            {
                var regest = _authContext.Requestes.Include(q => q.User).ThenInclude(q => q.listoftags).Where(m =>  m.UserId == userid && m.status != 2 && (search == null || search == "" || DbF.Like(m.User.User.DisplayedUserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")/*|| DbF.Like(m.User.User.UserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")*/));
                return regest.OrderByDescending(m => m.regestdata);
            }
            else
            {
                var regest = _authContext.Requestes.Include(q => q.User).ThenInclude(q => q.listoftags).Where(m => (m.UserId == userid|| m.UserRequestId == userid) && m.status!=2 && (search == null || search == "" || DbF.Like(m.User.User.DisplayedUserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")/*|| DbF.Like(m.User.User.UserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")*/));

                return regest.OrderByDescending(m => m.regestdata);
            }
        }
      
        public List<Requestes> GetallFrendes(int userid, string search)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

            var regest = _authContext.Requestes.Include(m => m.UserRequest)
                .Include(m => m.UserRequest.User)
                .Include(m => m.User)
                .Include(m => m.User.User).Where(m => 
                
                ((m.UserRequestId == userid && (search == null || search == "" || DbF.Like(m.User.User.DisplayedUserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%"))) 
                ||
                (m.UserId == userid && (search == null || search == "" || DbF.Like(m.UserRequest.User.DisplayedUserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%")))) && m.status == 1).ToList();

            return regest;
        }
        public IQueryable<Requestes> GetallBlock(int userid, string search = null)
        {
            var DbF = Microsoft.EntityFrameworkCore.EF.Functions;

            var regest = _authContext.Requestes.Where(m => (m.UserblockId == userid )
            && m.status == 2&&
            (search == null || search == "" || DbF.Like(m.User.User.DisplayedUserName.ToLower().Trim(), $"%{search.ToLower().Trim()}%") 
            ));


            return regest;
        }
        public Task requestresponse(Requestes code)
        {
            throw new NotImplementedException();
        }

        public async Task updaterequest(Requestes code)
        {
             this._authContext.Requestes.Update(code);
            await this._authContext.SaveChangesAsync();
        }
        public async Task deleterequest(Requestes code)
        {
            this._authContext.Requestes.Remove(code);
            await this._authContext.SaveChangesAsync();
        }
    }
}
