namespace Domain.Models
{
    public class Table
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int Size { get; set; }
        public bool IsOccupied { get; set; } = false;
        public List<Guid> ClientGroupIds { get; set; } = new List<Guid>();
    }
}
