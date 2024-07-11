using System.Runtime.Serialization;

namespace Optovka {
    [DataContract]
    public class RegisterDto
    {
        [DataMember(Name = "userName")]
        public required string UserName { get; set; }

        [DataMember(Name = "password")]
        public required string Password { get; set; }

        [DataMember(Name = "email")]
        public required string Email { get; set; }

        [DataMember(Name = "phoneNumber")]
        public required string PhoneNumber { get; set; }

        [DataMember(Name = "birthDate")]
        public DateTime BirthDate { get; set; }

        [DataMember(Name = "cardNumber")]
        public long CardNumber { get; set; }
    }

}