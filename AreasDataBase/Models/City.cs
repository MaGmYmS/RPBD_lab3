using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("city")]
    public class City
    {
        [Key]
        [Column("id_city")]
        public int IdCity { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Превышена длина строки")]
        [Display(Name = "Название города")]
        [Column("name_city")]
        public string NameCity { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(100000, 999999, ErrorMessage = "Почтовый код должен быть в диапазоне от 100000 до 999999")]
        [Display(Name = "Почтовый код")]
        [Column("postal_code")]
        public int PostalCode { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Название области")]
        [Column("area_id")]
        [ForeignKey("area")]
        public int AreaId { get; set; }

        public virtual Area? Area { get; set; }

        public List<District> Districts { get; set; } = new List<District>();
    }
}
