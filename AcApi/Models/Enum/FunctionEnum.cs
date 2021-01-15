using System;

namespace AcApi.Models.Enum
{
    [Flags]
    public enum FunctionEnum 
    {
        None = 0,
        AccessInfo = 1,
        Snapshot = 2
    }
}
