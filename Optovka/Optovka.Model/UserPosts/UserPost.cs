namespace Optovka.Model
{
    public class UserPost
    {
        private int id;
        private string authorUserId;
        private string title;
        private string section;
        private string description;
        private int requiredQuantity;
        private int takenQuantity;
        public ApplicationUser AuthorUser { get; set; }
        private DateTime createdAt;
        public List<ApplicationUser> ParticipatingUsers;

        public UserPost(string authorUserId, string title, string section, string description, int requiredQuantity) 
        {
            this.authorUserId = authorUserId;
            this.title = title;
            this.section = section;
            this.description = description;
            this.requiredQuantity = requiredQuantity;
            createdAt = DateTime.UtcNow;
            ParticipatingUsers = new List<ApplicationUser>();
            takenQuantity = 0;
        }

        public UserPost(UserPostModel userPostModel, string authorUserId) 
        {
            this.authorUserId = authorUserId;
            title = userPostModel.Title;
            section = userPostModel.Section;
            description = userPostModel.Description;
            requiredQuantity = userPostModel.RequiredQuantity;
            createdAt = DateTime.UtcNow;
            ParticipatingUsers = new List<ApplicationUser>();
            takenQuantity = 0;
        }

        public DateTime СreatedAt => createdAt;

        public int Id => id;

        public string AuthorUserId
        {
            get => authorUserId;
            set => authorUserId = value;
        }

        public string Title
        {
            get => title;
            set => title = value;
        }

        public string Section
        {
            get => section;
            set => section = value;
        }

        public string Description
        {
            get => description;
            set => description = value;
        }

        public int RequiredQuantity
        {
            get => requiredQuantity;
            set => requiredQuantity = value;
        }

        public int TakenQuantity
        {
            get => takenQuantity;
            set => takenQuantity = value;
        }
    }
}