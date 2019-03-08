using CatMash.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace CatMash.Domain.StoredProceduresParameters
{
    public class BaseStoredProcedureParameters : IBaseStoredProcedureParameters
    {
        public string Id { get; }
        public int? Rows { get; }
        public int? Offset { get; }
        public StoredProceduresEnum StoredProcedure { get; protected set; }

        protected BaseStoredProcedureParameters()
        {

        }

        protected BaseStoredProcedureParameters(int? pagesSize = null, int? startingLine = 1)
        {
            Rows = pagesSize;
            Offset = startingLine;
        }

        protected BaseStoredProcedureParameters(string id)
        {
            Id = id;
        }
    }
}
