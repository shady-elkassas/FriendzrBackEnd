using Social.Entity.DBContext;
using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Social.Services.Services;
namespace Social.Services.Implementation
{
    public class ErrorLogService : IErrorLogService
    {
        private readonly AuthDBContext _bwContext;
        public ErrorLogService(AuthDBContext bwContext)
        {
            this._bwContext = bwContext;
        }
        public async Task InsertErrorLog(BWErrorLog log)
        {
            this._bwContext.BWErrorLog.Add(log);
            await this._bwContext.SaveChangesAsync();
        }
    }
}
