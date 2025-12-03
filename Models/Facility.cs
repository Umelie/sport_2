//namespace oculus_sport.Models;

//public class Facility
//{
//    public string Name { get; set; } = string.Empty;
//    public string Location { get; set; } = string.Empty;
//    public string ImageUrl { get; set; } = "badminton_court.png"; 
//    public string Price { get; set; } = string.Empty;
//    public double Rating { get; set; }
//    public bool IsAvailable { get; set; }
//}

using System.Text.Json.Serialization;

namespace oculus_sport.Models
{
    public class Facility
    {
        [JsonPropertyName("facilityName")]
        public string Name { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; }

        [JsonPropertyName("price")]
        public double Price { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        //[JsonPropertyName("isAvailable")]
        //public bool IsAvailable { get; set; }

        //public string IdToken { get; set; }
    }
}
