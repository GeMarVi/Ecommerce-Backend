﻿namespace Ecommerce.Shared.Helpers
{
    public static class RandomGenerator
    {
        public static string GenetateRandomString(int size)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz!@#$%^&*()-_=+[{]}|;:',<.>/?";

            return new string(Enumerable.Repeat(chars, size).Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
