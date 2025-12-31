using System.Reflection;
using CSS.Challenge.Domain.Configuration;
using CSS.Challenge.Domain.Interfaces;
using CSS.Challenge.Domain.Models;
using CSS.Challenge.Domain.Models.Enums;

namespace CSS.Challenge.Domain.Services
{
    public class RandomOrderGeneratorService : IRandomOrderGeneratorService
    {
        private static readonly Random _random = new();
        private readonly List<string> _dishes;

        public RandomOrderGeneratorService(ApplicationSettings appSettings)
        {
            _dishes = LoadDishesFromFile(appSettings.DishFileName).ToList();
        }

        public List<OrderModel> Generate(OrderProcessingOptions options)
        {
            if (_dishes.Count < options.OrderCount)
                throw new ArgumentException("Not enough dish names provided");

            var dishes = new List<OrderModel>();
            var usedIds = new HashSet<string>();
            
            List<OrderTypeEnum> orderTypes = [OrderTypeEnum.Hot, OrderTypeEnum.Cold, OrderTypeEnum.Room, OrderTypeEnum.Unknown];

            for (var i = 0; i < options.OrderCount; i++)
            {
                var dishId = GenerateId(5);
                while (usedIds.Contains(dishId))
                {
                    dishId = GenerateId(5);
                }

                dishes.Add(new OrderModel(
                    dishId, 
                    _dishes[i], 
                    orderTypes[_random.Next(0, orderTypes.Count)],
                    _random.Next(5, 40),
                    _random.Next(options.FreshnessMin, options.FreshnessMax)
                ));

                usedIds.Add(dishId);
            }

            return dishes;
        }

        public List<string> LoadDishesFromFile(string resourceName)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream(resourceName)
                               ?? throw new InvalidOperationException(
                                   $"Embedded resource not found: {resourceName}");

            using var reader = new StreamReader(stream);

            var names = new List<string>();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(line))
                    names.Add(line);
            }

            names = names
                .Select(l => l.Trim())
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .Distinct()
                .ToList();

            if (names.Count < 100)
                throw new InvalidOperationException(
                    $"Expected at least 100 unique dish names, found {names.Count}");

            return names;
        }

        private string GenerateId(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            var buffer = new char[length];

            for (int i = 0; i < length; i++)
                buffer[i] = chars[_random.Next(chars.Length)];

            return new string(buffer);
        }

    }
}
