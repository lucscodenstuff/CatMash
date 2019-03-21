using CatMash.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class UpdateOneCatParameters : BaseStoredProcedureParameters, IUpdateOneCatParameters
    {
        
        public int Views { get; set; }
        public double Weight { get; set; }
        public double? Rating { get; set; }
        public UpdateOneCatParameters()
        {
            StoredProcedure = StoredProceduresEnum.UpdateOneCat;
        }

        public UpdateOneCatParameters(int id, int views, double weight, double? rating = null)
        {
            StoredProcedure = StoredProceduresEnum.UpdateOneCat;

            Id = id;
            Views = views;
            Weight = weight;
            Rating = rating;
        }
    }
}
