using System.IO;

namespace DatingApp.DataAccess.Dtos
{
    public class PhotoToUpload
    {
        public string FileName { get; set; }
        public Stream Stream  { get; set; }
    }
}