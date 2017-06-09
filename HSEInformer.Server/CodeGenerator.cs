using HSEInformer.Server.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HSEInformer.Server
{
    public class CodeGenerator : IConfirmationCodeGenerator
    {
        public string Generate()
        {
            Random rnd = new Random();
            string symbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            string result = string.Empty;
            for (int i = 0; i < 6; i++)
            {
                result += symbols[rnd.Next(0, symbols.Length)];
            }
            return result;
        }
    }
}
