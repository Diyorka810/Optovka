using System.Runtime.Serialization;

namespace Optovka
{
    [DataContract]
    public record ApiResponseDto
    {
        [DataMember(Name = "status")]
        public required string Status;

        [DataMember(Name = "message")]
        public required string Message;
    }
}