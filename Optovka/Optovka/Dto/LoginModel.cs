using System.Runtime.Serialization;

namespace Optovka.Model
{ 
    [DataContract]
    public class LoginModel
    {
        [DataMember(Name = "username")]
        public required string Username { get; set; }

        [DataMember(Name = "password")]
        public required string Password { get; set; }
    }
}
