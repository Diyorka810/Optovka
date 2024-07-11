namespace Optovka.Model
{
    public class UserPostModel
    {
        public string Title { get; set; }
        public string Section { get; set; }
        public string Description { get; set; }
        public int RequiredQuantity { get; set; }

        public UserPostModel(string title, string section, string description, int requiredQuantity)
        {
            Title = title;
            Section = section;
            Description = description;
            RequiredQuantity = requiredQuantity;
        }
    }
}