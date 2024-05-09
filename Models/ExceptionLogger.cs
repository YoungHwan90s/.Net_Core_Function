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

        [Column(TypeName = "varchar(255)")]
        public string? Message  { get; set; }

        [Column(TypeName = "varchar(1000)")]
        public string? ErrorPath  { get; set; }
        
        [Column(TypeName = "text")]
        public string? Device  { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
    }
}
