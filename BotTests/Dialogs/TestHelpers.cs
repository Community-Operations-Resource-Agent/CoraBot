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

        public static async Task<User> CreateUser(IApiInterface api)
        {
            var user = new User() { PhoneNumber = Guid.NewGuid().ToString() };
            await api.Create(user);
            return user;
        }
    }
}
