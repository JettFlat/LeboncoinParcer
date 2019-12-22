using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace LeboncoinParcer
{
    public class Realty
    {
        [Key]
        public string Id { get; set; }
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
        public bool Isbroken { get; set; }
        public string Status
        {
            get
            {
                if (Isbroken == true)
                    return "Broken";
                return "Active";
            }
        }
        public string Ges { get; set; }
        public string EnergyClass { set; get; }
        public string Desciption { get; set; }
        public void Update(Realty realty)
        {
            Name = realty.Name;
            Date = realty.Date;
            Phone = realty.Phone;
            LocalisationTown = realty.LocalisationTown;
            District = realty.District;
            Index = realty.Index;
            Type = realty.Type;
            Rooms = realty.Rooms;
            Isbroken = realty.Isbroken;
            Surface = realty.Surface;
            Furniture = realty.Furniture;
            Ges = realty.Ges;
            EnergyClass = realty.EnergyClass;
            Desciption = realty.Desciption;
        }
    }
}
