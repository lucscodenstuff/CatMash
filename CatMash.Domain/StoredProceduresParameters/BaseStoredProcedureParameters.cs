using CatMash.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class BaseStoredProcedureParameters : IBaseStoredProcedureParameters
    {
        public int? Id { get; set; }
        public int? Rows { get; set; }
        public int? Offset { get; set; }
        public StoredProceduresEnum StoredProcedure { get; protected set; }

        protected BaseStoredProcedureParameters()
        {

        }

        protected BaseStoredProcedureParameters(int? pagesSize = null, int? startingLine = 1)
        {
            Rows = pagesSize;
            Offset = startingLine;
        }

        protected BaseStoredProcedureParameters(int id)
        {
            Id = id;
        }
    }
}
