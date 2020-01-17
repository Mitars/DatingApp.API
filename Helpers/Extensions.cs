using System;

namespace DatingApp.API.Helpers
{
    /// <summary>
    /// The extensions helper class.
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// Generates the password hash and salt using the specified password.
        /// </summary>
        /// <param name="password">The password used to generate a hash and salt.</param>
        /// <returns>A tuple with the password hash and salt.</returns>
        public static (byte[] passwordHash, byte[] passwordSalt) GeneratePasswordHashSalt(this string password)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512()) {
                byte[] passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                byte[] passwordSalt = hmac.Key;
                return (passwordHash, passwordSalt);
            }
        }

        /// <summary>
        /// Gets the age using the specified date time.
        /// </summary>
        /// <param name="theDateTime">The date time which should be converted to the age.</param>
        /// <returns>The age.</returns>
        public static int Age(this DateTime theDateTime)
        {
            var age = DateTime.Today.Year - theDateTime.Year;
            if (theDateTime.AddYears(age) > DateTime.Today)
            {
                age--;
            }

            return age;
        } 
    }
}