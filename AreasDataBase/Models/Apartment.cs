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
        [MaxLength(50, ErrorMessage = "Превышена длина строки")]
        [Column("apartment_number")]
        public string ApartmentNumber { get; set; }

        [Display(Name = "Количество комнат")]
        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1, int.MaxValue, ErrorMessage = "Количество комнат должно быть больше 0")]
        [Column("num_of_rooms")]
        public int NumberOfRooms { get; set; }

        [Display(Name = "Площадь квартиры")]
        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(0, double.MaxValue, ErrorMessage = "Площадь квартиры должна быть неотрицательной")]
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
