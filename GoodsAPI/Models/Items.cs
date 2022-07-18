using System.ComponentModel.DataAnnotations;

namespace GoodsAPI.Models
{
    public class Items
    {
        [Key]
        [Required]
        [StringLength(128)]
        public string? ItemsCode { get; set; }
        [Required]
        [StringLength(254)]
        public string? ItemName { get; set; }
        [Required]
        public bool Active { get; set; }

        public Items()
        {
             Active = true;
        }
    }
}
