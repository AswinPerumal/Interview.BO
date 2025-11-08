using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BidOne.Gateway.Domain.Constants
{
    public static class StringConstants
    {
        public const string Validation_ProductNotFound = "Product not found";
        public const string Validation_ProductExists = "Product with same name already exists";

        public const string Idempotency_DuplicateRequest = "Previous request is still being processed";
        public const string Idempotency_MissingKey = "Missing Idempotency-Key header";
    }
}
