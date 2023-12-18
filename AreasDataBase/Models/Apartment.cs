using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("apartment")]
    public class Apartment
    {
        [Key]
        [Column("id_apartment")]
        public int IdApartment { get; set; }

        [Display(Name = "Номер квартиры")]
        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1, 1000, ErrorMessage = "Номер квартиры должен быть больше 0 и меньше 1000")]
        [Column("apartment_number")]
        public int ApartmentNumber { get; set; }

        [Display(Name = "Количество комнат")]
        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1, 100, ErrorMessage = "Количество комнат должно быть больше 0 и меньше 100")]
        [Column("num_of_rooms")]
        public int NumberOfRooms { get; set; }

        [Display(Name = "Площадь квартиры")]
        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1, 2000, ErrorMessage = "Площадь квартиры должна быть неотрицательной и меньше 2000")]
        [Column("area")]
        public double Area { get; set; }


        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Номер дома")]
        [Column("residential_building_id")]
        [ForeignKey("residential_building")]
        public int ResidentialBuildingId { get; set; }

        public ResidentialBuilding? ResidentialBuilding { get; set; }

        public List<Citizen> citizens { get; set; } = new List<Citizen>();
    }
}
