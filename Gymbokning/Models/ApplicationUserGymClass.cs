namespace Gymbokning.Models
{
    public class ApplicationUserGymClass
    {
        // Foreign Keys
        public string ApplicationUserId { get; set; }
        public int GymClassId { get; set; }

        // Nav props
        public ApplicationUser ApplicationUser { get; set; }
        public GymClass GymClass { get; set; }
    }
}
