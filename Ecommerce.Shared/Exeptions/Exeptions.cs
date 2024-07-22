
namespace Ecommerce.Shared.Exeptions
{
    public class CloudinaryException : Exception
    {
        public CloudinaryException(string message) : base(message) { }
    }

    public class DatabaseException : Exception
    {
        public DatabaseException(string message) : base(message) { }
    }

    public class ProductNotFoundException : Exception
    {
        public ProductNotFoundException(string message) : base(message) { }
    }
}
