

using IdentityMicroservice.Models;

namespace tests.MockData
{
    internal class UserMockData
    {

        public static List<User> GetSampleUserItems()
        {
            return new List<User>
            {
                new User
                {
                CId = new Guid("10188938-5308-4B19-8E97-57E7F36A6184"),
                Email = "poorna@gmail.com",
                Password = "Poorna@123",
                Name = "Poorna Chander"

                },

               new User
                {
                CId = new Guid("20188938-5308-4B19-8E97-57E7F36A6184"),
                Email = "sai@gmail.com",
                Password = "Sai@123",
                Name = "Sai Kumar"

                },

            };

        }
    }
}
