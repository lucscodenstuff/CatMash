using System.Collections.Generic;
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
        public double Rating { get; set; }
        public int ViewsNumber { get; set; }
        public double ProbabilityWeight { get; set; }
        public IEnumerable<FurTypesEnum> FurTypes { get; set; }
    }

    public class Payload
    {
        public Cat Winner { get; set; }
        public Cat Loser { get; set; }
    }
}
