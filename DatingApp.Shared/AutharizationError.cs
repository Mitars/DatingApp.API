namespace DatingApp.Shared
{
    public class DatabaseError : Error
    {
        public DatabaseError(string message) : base(message) { }
    }
}