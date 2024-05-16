using System.Runtime.Serialization;

namespace Optovka.Model
{
    [DataContract]
    public class UserInfoDto
    {
        [DataMember(Name = "username")]
        public required string Username { get; set; }

        [DataMember(Name = "email")]
        public required string Email { get; set; }
        
        [DataMember(Name = "phoneNumber")]
        public required string PhoneNumber { get; set; }

        [DataMember(Name = "birthDate")]
        public required DateTime BirthDate { get; set; }
    }
}
