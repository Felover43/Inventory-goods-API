using System.ComponentModel.DataAnnotations;

namespace GoodsAPI.Models
{
    public class BPType
    {
        [Key]
        [Required]
        [StringLength(1)]
        public char? TypeCode { get; set; }
        [Required]
        [StringLength(20)]
        public string? TypeName { get; set; }

        public ICollection<BusinessPartners>? BP { get; set; }
    }
}
