using Microsoft.AspNetCore.Identity;
using System.Text;

namespace Optovka.Model
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime BirthDate {  get; set; }
        public long CardNumber { get; set; }
        public List<UserPost> PersonalUserPosts { get; set; }
        public List<UserPost> ParticipatedUserPosts { get; set; }

        public ApplicationUser() {
            PersonalUserPosts = new List<UserPost>();
            ParticipatedUserPosts = new List<UserPost>();
        }

        public (bool, StringBuilder) IsValid()
        {
            var sb = new StringBuilder("Errors: \n");
            var isValid = true;
            Validate(() => this.Email == null, "The email can not be null");

            if (this.Email != null)
            {
                var symbols = "!#$%^&*()-=_+,?/|\\`:;\'\"{}[]";
                Validate(() => StringExtensions.ContainsAny(this.Email, symbols), "The email can only contain letters, numbers and . ");

                var emailArr = this.Email.Split('@');
                Validate(() => emailArr.Length != 2, "Email should contains one @");

                if (emailArr.Length == 2)
                {
                    var emailSecPartArr = emailArr[1].Split('.');
                    Validate(() => emailSecPartArr.Length != 2, "Email should contains 1 . after @");
                }
                
            }
            
            Validate(() => this.CardNumber.ToString().Length != 16, "Card number is incorrect");
            Validate(() => CountYears() < 16, $"You are too young. Come back in {16 - CountYears()} years ");

            return (isValid, sb);

            void Validate(Func<bool> condition, string message)
            {
                if (condition())
                {
                    sb.AppendLine(message);
                    isValid = false;
                }
            }
            
        }

        public int CountYears()
        {
            var today = DateTime.Today;

            if (today.Month - BirthDate.Month == 0 && today.Day - BirthDate.Day < 0)
            {
                return today.Year - BirthDate.Year - 1;
            }
            else if (today.Month - BirthDate.Month < 0)
            {
                return today.Year - BirthDate.Year - 1;
            }
            else
            {
                return today.Year - BirthDate.Year;
            }
        }
    }
}