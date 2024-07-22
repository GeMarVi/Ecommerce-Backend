namespace Ecommerce.Shared.DTOs
{
    public class ShippmentInfoResponseDto
    {
        public Guid ShipmentInfo_Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string street { get; set; }
        public int ExteriorNumber { get; set; }
        public int? InteriorNumber { get; set; }
        public int CodigoPostal { get; set; }
        public string? AditionalInformation { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Municipality { get; set; }
        public string Colony { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string user_Id { get; set; }
    }
}
