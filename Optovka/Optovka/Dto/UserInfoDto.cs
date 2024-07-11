using System.Runtime.Serialization;

namespace Optovka
{
    [DataContract]
    public class UserInfoDto
    {
        [DataMember(Name = "userName")]
        public required string UserName { get; set; }

        [DataMember(Name = "email")]
        public required string Email { get; set; }
        
        [DataMember(Name = "phoneNumber")]
        public required string PhoneNumber { get; set; }

        [DataMember(Name = "birthDate")]
        public required DateTime BirthDate { get; set; }
    }
}