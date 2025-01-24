using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class LookupClientsRequest
    {
        [Required]
        public Guid GroupId { get; set; }
    }
}
