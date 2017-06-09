using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace HSEInformer.Server
{
    public class AuthOptions
    {
        public const string ISSUER = "HseInformerServer"; // издатель токена
        public const string AUDIENCE = "http://localhost:51569/"; // потребитель токена
        const string KEY = "HSEInformer_Key_230514";   // ключ для шифрации
        public const int LIFETIME = 10000; // время жизни токена - 10000 минут
        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}
