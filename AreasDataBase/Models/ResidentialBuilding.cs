﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AreasDataBase.Models
{
    [Table("residential_building")]
    public class ResidentialBuilding
    {
        [Key]
        [Column("id_residential_building")]
        public int IdResidentialBuilding { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Номер дома")]
        [Column("house_number")]
        public string HouseNumber { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Год постройки")]
        [Range(1800, 2100, ErrorMessage = "Год постройки должен быть не ранее 1800 года и не позднее 2100")]
        [Column("year_of_construction")]
        public int YearOfConstruction { get; set; }

        [Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Количество этажей")]
        [Range(1, 300, ErrorMessage = "Количество этажей должно быть в диапазоне от 1 до 300")]
        [Column("numbers_of_floors")]
        public int NumbersOfFloors { get; set; }

        //[Required(ErrorMessage = "Значение не может быть пустым")]
        [Display(Name = "Название улицы")]
        [Column("street_id")]
        [ForeignKey("street")]
        public int? StreetId { get; set; }

        public virtual Street? Street { get; set; }

        public List<Apartment> Apartments { get; set; } = new List<Apartment>();
    }
}
