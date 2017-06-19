using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server.ViewModels
{
    public class RequestAnswerViewModel
    {
        public int GroupId { get; set; }

        public string UserName { get; set; }

        public bool Accepted { get; set; }
    }
}
