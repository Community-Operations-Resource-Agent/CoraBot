using Microsoft.Azure.Cosmos.Spatial;
using Shared.ApiInterface;
using Shared.Models;
using System;
using System.Threading.Tasks;

namespace BotTests
{
    public static class TestHelpers
    {
        public const int DefaultQuantity = 5;

        public const bool DefaultIsUnopened = true;

        public const string DefaultInstructions = "Instructions";

        public static Point LocationCoordinatesSeattle = new Point(-122.4821495, 47.6131746);

        public static Point LocationCoordinatesNewYork = new Point(-74.2598737, 40.6976701);

        public static async Task<User> CreateUser(IApiInterface api)
        {
            var user = new User() { PhoneNumber = Guid.NewGuid().ToString() };
            await api.Create(user);
            return user;
        }
    }
}
