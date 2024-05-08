using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreWebAPI.Models
{
    public class ExceptionLogger
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(100)")]
        public string? ExceptionType  { get; set; }

        [Column(TypeName = "int")]
        public int StatusCode  { get; set; }

        [Column(TypeName = "varchar(300)")]
        public string? Message  { get; set; }

        [Column(TypeName = "varchar(500)")]
        public string? ErrorPath  { get; set; }
        
        [Column(TypeName = "varchar(50)")]
        public string? Device  { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
    }
}
