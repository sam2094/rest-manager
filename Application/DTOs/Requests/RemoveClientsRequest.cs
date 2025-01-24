using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class RemoveClientsRequest
    {
        [Required]
        public Guid GroupId { get; set; }
    }
}
