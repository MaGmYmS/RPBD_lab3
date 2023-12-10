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
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Длина паспортных данных должна быть 10 символов")]
        [Display(Name = "Паспортные данные")]
        [Column("passport_data")]
        public string PassportData { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "Длина номера телефона должна быть 11 символов")]
        [Display(Name = "Номер телефона")]
        [Column("phone_number")]
        public string PhoneNumber { get; set; }

        private DateTime _dateOfBirth;

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Дата рождения")]
        [Column("date_of_birth")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        [DataType(DataType.Date)]
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
