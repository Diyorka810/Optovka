using System.Runtime.Serialization;

namespace Optovka
{ 
    [DataContract]
    public class LoginDto
    {
        [DataMember(Name = "username")]
        public required string Username { get; set; }

        [DataMember(Name = "password")]
        public required string Password { get; set; }
    }
}