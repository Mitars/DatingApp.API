using System.Collections.Generic;

namespace DatingApp.Business.Dtos
{
    public class UserWithRoles
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}