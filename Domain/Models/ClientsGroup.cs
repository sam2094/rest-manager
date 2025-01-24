namespace Domain.Models
{
    public class ClientsGroup
    {
        public Guid Id { get; set; } 
        public int Size { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is ClientsGroup other)
            {
                return Id == other.Id;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
