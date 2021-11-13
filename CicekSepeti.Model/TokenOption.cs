using System.Collections.Generic;

namespace CicekSepeti.Model
{
    public class TokenOption
    {
        public List<string> Audience { get; set; }
        public string Issuer { get; set; }
        public int Expiration { get; set; }
    }
}
