
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedTrip.Data.Models
{
    using static DataConstants;
    public class UserTrip
    {
        
        [Required]
        [MaxLength(IdMaxLength)]
        public string UserId { get; init; } = Guid.NewGuid().ToString();

        public User User { get; set; }

        
        [Required]
        [MaxLength(IdMaxLength)]
        public string TripId { get; init; } = Guid.NewGuid().ToString();

        public Trip Trip { get; set; }
    }
}
