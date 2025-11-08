using BidOne.Gateway.Domain.Enums;
using BidOne.Gateway.Domain.Extensions;

namespace BidOne.Gateway.Domain.Exceptions
{
    public class BidOneException : Exception
    {
        public readonly ExceptionType exceptionType;
        public BidOneException(ExceptionType exceptionType, string message) : base(message)
        {
            this.exceptionType = exceptionType;
            if (string.IsNullOrWhiteSpace(message))
            {
                message = exceptionType.GetDescription();
            }
        }
    }


}
