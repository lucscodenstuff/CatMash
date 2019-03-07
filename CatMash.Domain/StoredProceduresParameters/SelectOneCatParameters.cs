using System;
using System.Collections.Generic;
using System.Text;
using CatMash.Domain.Enums;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class SelectOneCatParameters : BaseStoredProcedureParameters, ISelectOneCatParameters
    {
        public SelectOneCatParameters(int id) : base(id)
        {
            StoredProcedure = StoredProceduresEnum.SelectOneCat;
        }
    }
}
