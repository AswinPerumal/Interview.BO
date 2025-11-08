namespace BidOne.Gateway.API.Attributes
{
    using BidOne.Gateway.API.Filters;
    using Microsoft.AspNetCore.Mvc;

    [AttributeUsage(AttributeTargets.Method)]
    public class IdempotentAttribute : TypeFilterAttribute
    {
        public IdempotentAttribute() : base(typeof(IdempotencyFilter))
        {
        }
    }

}
