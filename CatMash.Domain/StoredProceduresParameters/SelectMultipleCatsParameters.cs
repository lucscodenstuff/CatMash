using System;
using System.Collections.Generic;
using System.Text;
using CatMash.Domain.Enums;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class SelectMultipleCatsParameters : BaseStoredProcedureParameters, ISelectMultipleCatsParameters
    {
        public bool? IsAStar { get; set; }
        public FurTypesEnum? FurType { get; set; }
        public bool? IsAlone { get; set; }
        public SelectMultipleCatsParameters()
        {
            StoredProcedure = StoredProceduresEnum.SelectMultipleCats;
        }

        public SelectMultipleCatsParameters(bool? isAStar = null, FurTypesEnum? furType = null, bool? isAlone = null)
        {
            StoredProcedure = StoredProceduresEnum.SelectMultipleCats;

            IsAStar = isAStar;
            FurType = furType;
            IsAlone = isAlone;
        }
    }
}
