using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("area")]
    public class Area
    {
        [Key]
        [Column("id_area")]
        public int IdArea { get; set; }


        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Range(1, 999, ErrorMessage = "Код области должен быть в диапазоне от 1 до 999")]
        [Display(Name = "Код субъекта")]
        [Column("subject_code")]
        //[Remote(action: "IsSubjectCodeUnique", controller: "Area", ErrorMessage = "Код региона уже существует.")]
        public int SubjectCode { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [MaxLength(255, ErrorMessage = "Превышена длина строки")]
        [Display(Name = "Название области")]
        [Column("name_area")]
        public string NameArea { get; set; }    

        public List<City> Cities { get; set; } = new List<City>();
    }
}
