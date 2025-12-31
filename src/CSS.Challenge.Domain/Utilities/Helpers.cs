namespace CSS.Challenge.Domain.Utilities
{
    public static class Helpers
    {
        public static int GeneratePickupTime(int min, int max)
        {
            var rnd = new Random();
            return rnd.Next(min, max);
        }
    }
}