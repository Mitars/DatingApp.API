using System.Collections.Generic;
using System.Linq;
using DatingApp.Models;
using Microsoft.AspNetCore.Identity;
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
        /// <param name="userManager">The user manager.</param>
        /// <param name="roleManager">The role manager.</param>
        public static void SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (userManager.Users.Any())
            {
                return;
            }

            var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
            var users = JsonConvert.DeserializeObject<List<User>>(userData);

            var roles = new List<Role>
            {
                new Role { Name = "Member" },
                new Role { Name = "Admin" },
                new Role { Name = "Moderator" },
                new Role { Name = "VIP" },
            };

            foreach (var role in roles)
            {
                roleManager.CreateAsync(role).Wait();
            }

            foreach (var u in users)
            {
                u.Photos.SingleOrDefault().IsApproved = true;
                userManager.CreateAsync(u, "password").Wait();
                userManager.AddToRoleAsync(u, "Member").Wait();
            }

            // Create admin user
            var adminUser = new User
            {
                UserName = "Admin"
            };

            var result = userManager.CreateAsync(adminUser, "password").Result;

            if (result.Succeeded)
            {
                var admin = userManager.FindByNameAsync("Admin").Result;
                userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }).Wait();
            }
        }
    }
}