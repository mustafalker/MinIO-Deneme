using System.ComponentModel.DataAnnotations;

namespace MinIO.PutObject.Entity
{
    public class File
    {
        [Key]
        public string Key { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
    }
}
