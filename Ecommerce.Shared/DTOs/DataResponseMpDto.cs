using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Ecommerce.Shared.DTOs
{
    public class DataResponseMpDto
    {
        public string? action { get; set; }
        public string? api_version { get; set; }
        public Data? data { get; set; }
        public DateTime? date_created { get; set; }
        public string? id { get; set; }
        public bool? live_mode { get; set; }
        public string? type { get; set; }
        public long? user_id { get; set; }
    }

    public class Data
    {
        public string? id { get; set; }
    }
}
