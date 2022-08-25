namespace Gymbokning.ViewModels
{
    public class GymClassesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public DateTime StartDate { get; set; }
        public TimeSpan Duration { get; set; }
        public bool Attending { get; set; }
    }
}
