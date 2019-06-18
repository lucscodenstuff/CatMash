using CatMash.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class GetTotalWeightParameters : BaseStoredProcedureParameters, IGetTotalWeightParameters
    {
        public GetTotalWeightParameters()
        {
            StoredProcedure = StoredProceduresEnum.GetTotalWeight;
        }
    }
}
