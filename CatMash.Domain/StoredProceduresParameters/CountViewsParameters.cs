using CatMash.Domain.Enums;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class CountViewsParameters : BaseStoredProcedureParameters, ICountViewsParameters
    {
        public FurTypesEnum? FurType { get;  }
        public CountViewsParameters()
        {
            StoredProcedure = StoredProceduresEnum.CountViews;
        }

        public CountViewsParameters(FurTypesEnum? furType = null)
        {
            StoredProcedure = StoredProceduresEnum.CountViews;

            FurType = furType;
        }
    }
}
