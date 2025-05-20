namespace Ecommerce.BackEnd.UseCases.Helpers
{
    public static class Helpers
    {
        internal static string GenerateVerificationCode()
        {
            return new Random().Next(100000, 999999).ToString();
        }
    }
}
