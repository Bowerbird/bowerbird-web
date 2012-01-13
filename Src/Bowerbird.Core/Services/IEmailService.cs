﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Bowerbird.Core.DomainModels;

namespace Bowerbird.Core.Services
{
    public interface IEmailService : IService
    {
        void SendMailMessage(MailMessage mailMessage);
    }
}