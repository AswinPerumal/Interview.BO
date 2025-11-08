using System.ComponentModel;

namespace BidOne.Gateway.Domain.Enums
{
    public enum ExceptionType
    {
        [Description("Validation failed")]
        Validation,
        [Description("Entity not found")]
        NotFound,
        [Description("Duplicate entity exists")]
        Conflict
    }
}
