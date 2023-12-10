using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("street")]
    public class Street
    {
        [Key]
        [Column("id_street")]
        public int IdStreet { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Превышена длина строки")]
        [Display(Name = "Название улицы")]
        [Column("name_street")]
        public string NameStreet { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Название района")]
        [Column("district_id")]
        [ForeignKey("district")]
        public int DistrictId { get; set; }

        public virtual District? District { get; set; }

        public List<ResidentialBuilding> ResidentialBuildings { get; set; } = new List<ResidentialBuilding>();
    }
}
