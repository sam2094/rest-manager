namespace Domain.Models
{
    public class Table
    {
        public int Id { get; set; } 
        public int Size { get; set; } 
        public bool IsOccupied { get; set; } = false; 
        public List<int> ClientGroupIds { get; set; } = new List<int>();
    }
}
