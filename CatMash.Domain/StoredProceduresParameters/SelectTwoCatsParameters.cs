using CatMash.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class SelectTwoCatsParameters : BaseStoredProcedureParameters, ISelectTwoCatsParameters
    {
        public int CatOneId { get; set; }
        public int CatTwoId { get; set; }
        public FurTypesEnum? FurType { get; set; }
        public SelectTwoCatsParameters()
        {
            StoredProcedure = StoredProceduresEnum.SelectTwoCats;
        }

        public SelectTwoCatsParameters(int catOneId, int catTwoId, FurTypesEnum? furType = null)
        {
            StoredProcedure = StoredProceduresEnum.SelectTwoCats;

            CatOneId = catOneId;
            FurType = furType;
            CatTwoId = catTwoId;
        }
    }
}
