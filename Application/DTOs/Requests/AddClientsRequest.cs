using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Requests
{
    public class AddClientsRequest
    {
        [Range(1, 6, ErrorMessage = "Group size must be between 1 and 6")]
        public int GroupSize { get; set; }
    }
}
