using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.Interfaces
{
    public interface IEmailManager
    {
        Task<bool> SendConfirmationEmailAsync(string email, string confirmation_code);
    }
}
