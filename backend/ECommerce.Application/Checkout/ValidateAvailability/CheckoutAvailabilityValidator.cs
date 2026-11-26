using ECommerce.Application.Checkout.StartCheckout;
using System;
using System.Collections.Generic;
using ECommerce.Application.Common.Results;

namespace ECommerce.Application.Checkout.ValidateAvailability
{
    public sealed class CheckoutAvailabilityValidator
    {
        public IReadOnlyList<Error> Validate(
        CheckoutCartSnapshot? cart)
        {
            var errors = new List<Error>();

            // BR-131: Checkout requires a valid, non-empty cart.
            if (cart is null || cart.Items.Count == 0)
            {
                errors.Add(new Error(
                    "Checkout.EmptyCart",
                    "The shopping cart is empty."));

                return errors;
            }

            foreach (var item in cart.Items)
            {
                // BR-128: Product must be active.
                if (!item.IsProductActive)
                {
                    errors.Add(new Error(
                        "Checkout.InactiveProduct",
                        $"Product for variant {item.ProductVariantId} is inactive."));

                    continue;
                }

                // BR-129: Variant must be active.
                if (!item.IsVariantActive)
                {
                    errors.Add(new Error(
                        "Checkout.InactiveVariant",
                        $"Variant {item.ProductVariantId} is inactive."));

                    continue;
                }

                // Validate cart quantity.
                if (item.Quantity <= 0)
                {
                    errors.Add(new Error(
                        "Checkout.InvalidQuantity",
                        $"Variant {item.ProductVariantId} has an invalid quantity."));

                    continue;
                }

                // BR-130: Current available stock must be sufficient.
                if (item.AvailableQuantity < item.Quantity)
                {
                    errors.Add(new Error(
                        "Checkout.InsufficientStock",
                        $"Variant {item.ProductVariantId} does not have enough available stock."));
                }
            }

            return errors;
        }
    }
}
