using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("citizen")]
    public class Citizen
    {
        [Key]
        [Column("id_citizen")]
        public int IdCitizen { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Превышена длина строки")]
        [Display(Name = "ФИО")]
        [Column("full_name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1000000000, 9999999999, ErrorMessage = "Паспортные данные должны быть в диапазоне от 1000000000 до 9999999999")]
        [Display(Name = "Паспортные данные")]
        [Column("passport_data")]
        public long PassportData { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(10000000000, 99999999999, ErrorMessage = "Длина номера телефона должна быть 11 символов")]
        [Display(Name = "Номер телефона")]
        [Column("phone_number")]
        public long PhoneNumber { get; set; }

        private DateTime _dateOfBirth;

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Дата рождения")]
        [Column("date_of_birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        //[Range(typeof(DateTime), "01/01/1900", "01/01/2100", ErrorMessage = "Дата рождения должна быть между 01.01.1900 и 01.01.2100")]
        public DateTime DateOfBirth 
        {
            get => _dateOfBirth;
            set => _dateOfBirth = DateTime.SpecifyKind(value, DateTimeKind.Utc);
        }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Пол")]
        [Column("gender")]
        public bool Gender { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Номер квартиры")]
        [Column("apartment_id")]
        [ForeignKey("apartment")]
        public int ApartmentId { get; set; }

        public virtual Apartment? Apartment { get; set; }
    }
}
