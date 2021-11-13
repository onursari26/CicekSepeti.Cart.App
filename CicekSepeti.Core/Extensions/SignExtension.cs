using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace CicekSepeti.Core.Extensions
{
    public static class SignExtension
    {
        public static SecurityKey GetSymmetricSecurityKey(this string securityKey)
        {
            return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
        }
    }
}
