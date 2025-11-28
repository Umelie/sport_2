using System;
using SQLite; // Required for SQLite attributes

namespace oculus_sport.Models
{
    public class Booking
    {
        // [PrimaryKey] ensures the local booking record can be looked up quickly.
        [PrimaryKey] 
        public string Id { get; set; } = Guid.NewGuid().ToString().Substring(0, 8).ToUpper(); 
        
        // Use [Indexed] for efficient querying in SQLite (e.g., finding all of a user's bookings)
        [Indexed]
        public string UserId { get; set; }

        // Facility Details
        public string FacilityName { get; set; }
        public string FacilityImage { get; set; }
        public string Location { get; set; }

        // Time Details
        public DateTime Date { get; set; }
        public string TimeSlot { get; set; } 
        public string Status { get; set; } = "Pending";

        // Contact Details
        public string ContactName { get; set; }
        public string ContactStudentId { get; set; }
        public string ContactPhone { get; set; }

        // Payment
        public string TotalCost { get; set; } = "Rp 50.000"; 
    }
}