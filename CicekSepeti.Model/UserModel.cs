using Newtonsoft.Json;

namespace CicekSepeti.Model
{
    public class UserModel
    {
        public string Username { get; set; }
        public string Email { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string Password { get; set; }
    }
}
