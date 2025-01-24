namespace Application.DTOs.Responses
{
    public class TableDto
    {
        public Guid Id { get; set; }
        public int Size { get; set; }
        public bool IsOccupied { get; set; }
        public List<Guid> ClientGroupIds { get; set; } = new List<Guid>();
    }
}
