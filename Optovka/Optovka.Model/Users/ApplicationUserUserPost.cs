namespace Optovka.Model
{
    public class ApplicationUserUserPost
    {
        public int ParticipatedUserPostId { get; set; }
        public string ParticipatingUserId { get; set; }
        public UserPost ParticipatedUserPost { get; set; }
        public ApplicationUser ParticipatingUser { get; set; }
        public int TakenQuantity { get; set; }
    }
}