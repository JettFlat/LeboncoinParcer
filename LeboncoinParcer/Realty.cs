using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace LeboncoinParcer
{
    public class Realty
    {
        [Key]
        public int Id { get; set; }
        public string Url { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Phone { get; set; }
        public string LocalisationTown { get; set; }
        public string District { get; set; }
        public string Index { get; set; }
        public string Type { get; set; }
        public int Rooms { get; set; }
        public string Surface { get; set; }
        public string Furniture { get; set; }
        public string Ges { get; set; }
        public string EnergyClass { set; get; }
        public string Desciption { get; set; }
    }
}
