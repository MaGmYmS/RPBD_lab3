using AreasDataBase.Data;
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
        [RegularExpression(@"^[a-zA-Zа-яА-ЯЁ-]+(\s[a-zA-Zа-яА-ЯЁ-]+)+$", ErrorMessage = "Введите как минимум фамилию и имя")]
        [Display(Name = "ФИО")]
        [Column("full_name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1000000000, 9999999999, ErrorMessage = "Паспорт должен содержать 10 символов")]
        [UniquePassport(ErrorMessage = "Паспорт уже зарегистрирован")]
        [Display(Name = "Паспортные данные")]
        [Column("passport_data")]
        public long PassportData { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(10000000000, 99999999999, ErrorMessage = "Длина номера телефона должна быть 11 символов")]
        [UniquePhoneNumber(ErrorMessage = "Номер телефона уже зарегистрирован")]
        [Display(Name = "Номер телефона")]
        [Column("phone_number")]
        public long PhoneNumber { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Дата рождения")]
        [Column("date_of_birth")]
        [DataType(DataType.Date)]
        [ValidBirthDate(0, 100)] // минимальный возраст 0 (в дни), максимальный - 100 лет
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateOfBirth { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Пол")]
        [Column("gender")]
        public bool Gender { get; set; }

        //[Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Номер квартиры")]
        [Column("apartment_id")]
        [ForeignKey("apartment")]
        public int? ApartmentId { get; set; }

        public virtual Apartment? Apartment { get; set; }
    }


    public class UniquePassportAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (AreasDataBaseContext)validationContext.GetService(typeof(AreasDataBaseContext));
            var entity = _context.Citizen.FirstOrDefault(c => c.PassportData == (long)value);

            if (entity != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    public class UniquePhoneNumberAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var _context = (AreasDataBaseContext)validationContext.GetService(typeof(AreasDataBaseContext));
            var entity = _context.Citizen.FirstOrDefault(c => c.PhoneNumber == (long)value);

            if (entity != null)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }

    public class ValidBirthDateAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        private readonly int _maxAge;

        public ValidBirthDateAttribute(int minAge, int maxAge)
        {
            _minAge = minAge;
            _maxAge = maxAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateOfBirth)
            {
                if (dateOfBirth > DateTime.Now)
                {
                    return new ValidationResult("Дата рождения не может быть в будущем.");
                }

                int age = DateTime.Now.Year - dateOfBirth.Year;

                if (DateTime.Now < dateOfBirth.AddYears(age))
                {
                    age--;
                }

                if (age < _minAge || age > _maxAge)
                {
                    return new ValidationResult($"Возраст должен быть в диапазоне от {_minAge} до {_maxAge} лет.");
                }

                return ValidationResult.Success;
            }

            return new ValidationResult("Некорректный формат даты рождения.");
        }
    }
}
