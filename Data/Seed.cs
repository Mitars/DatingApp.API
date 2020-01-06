using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp.API.Data
{
    /// <summary>
    /// The seed class.
    /// Used to seed data into the database.
    /// </summary>
    public class Seed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Seed"/> class.
        /// </summary>
        /// /// <param name="context">The data context.</param>
        public static void SeedUsers(DataContext context)
        {
            if (!context.Users.Any())
            {
                var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
                var users = JsonConvert.DeserializeObject<List<User>>(userData);
                foreach (var user in users)
                {
                    user.Username = user.Username.ToLower();
                    (user.PasswordHash, user.PasswordSalt) = "password".GeneratePasswordHashSalt();                   
                    context.Users.Add(user);
                }

                context.SaveChanges();
            }
        }
    }
}