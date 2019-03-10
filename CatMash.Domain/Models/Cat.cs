using System;
using System.Collections.Generic;
using System.Text;
using CatMash.Domain.Enums;

namespace CatMash.Domain.Models
{
    public class Cat
    {
        public int Id { get; set; }
        public string CatUrl { get; set; }
        public bool IsAStar { get; set; }
        public bool IsTopOne { get; set; }
        public bool IsAlone { get; set; }
        public decimal Rating { get; set; }
        public IEnumerable<FurTypesEnum> FurTypes { get; set; }
    }
}
