using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("district")]
    public class District
    {
        [Key]
        [Column("id_district")]
        public int IdDistrict { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Превышена длина строки")]
        [Display(Name = "Название района")]
        [Column("name_district")]
        public string NameDistrict { get; set; }

        //[Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Название города")]
        [Column("city_id")]
        [ForeignKey("city")]
        public int? CityId { get; set; }

        public virtual City? City { get; set; }

        public List<Street> Streets { get; set; } = new List<Street>();
    }
}
