
using IdentityMicroservice.Models;

namespace tests.MockData
{
    internal class AdminMockData
    {
        public static List<Admin> GetSampleAdminItems()
        {
            return new List<Admin>
            {
                new Admin
                {
                AId = new Guid("30188938-5308-4B19-8E97-57E7F36A6184"),
                Email = "admin@gmail.com",
                Password = "Admin@123",
                Name = "Natasha"

                }

            };

        }
    }
}
