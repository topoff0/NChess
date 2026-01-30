namespace Chess.Models
{
    public class Tournament
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public int Prize { get; set; }
        public int? WinnerId { get; set; }
        public required List<int> ParticipantsId { get; set; }
    }
}