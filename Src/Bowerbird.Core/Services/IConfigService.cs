using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Services
{
    public interface IConfigService : IService
    {
        string GetEmailServerName();

        string GetMediaRootUri();

        string GetMediaRootPath();

        string GetDatabaseName();
    }
}