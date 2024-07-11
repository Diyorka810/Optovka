using Optovka.Model;
using System.Runtime.Serialization;

namespace Optovka
{
    [DataContract]
    public class UserPostDto
    {
        [DataMember(Name= "title")]
        public required string Title { get; set; }
        
        [DataMember(Name = "section")]
        public required string Section { get; set; }
        
        [DataMember(Name = "description")]
        public required string Description { get; set; }
        
        [DataMember(Name = "requiredQuantity")]
        public int RequiredQuantity { get; set; }

        public UserPostModel ToUserPostModel()
        {
            return new UserPostModel(Title, Section, Description, RequiredQuantity);
        }
    }
}