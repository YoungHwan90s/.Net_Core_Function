using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NetCoreWebAPI.Models
{
    public class SuperHeros
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string? SuperHero { get; set; }

        [Column(TypeName = "varchar(30)")]
        public string? Name { get; set; }

        [Column(TypeName = "int")]
        public int PowerIndex { get; set; }
    }
}