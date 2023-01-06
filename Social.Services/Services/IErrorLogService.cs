using Social.Entity.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Social.Services.Services
{
    public interface IErrorLogService
    {
        Task InsertErrorLog(BWErrorLog log);
    }
}
