using System;
using System.Linq;

namespace Library
{
  public static class CartCalculations
    {

        /// <summary>
        /// Calculate tax based on 6% sales tax
        /// </summary>
        /// <param name="totalValue">The total value of the items in the cart</param>
        /// <returns>The tax value to add to the cart total</returns>
        public static decimal CalculateTax(decimal totalValue) {

            return totalValue * 0.06m;

        }

        /// <summary>
        /// Calculate the total number of items and price of the cart
        /// </summary>
        /// <param name="items"></param>
        /// <returns>Tuple with the total number of items and the total price of the items in the cart</returns>
        public static (int count, decimal totalPrice) TotalItems(IEnumerable<CartLineItem> items) {

            var count = items.Sum(i => i.Quantity);
            var totalPrice = items.Sum(i => i.TotalPrice);

            return (count, totalPrice);

        }


    }
}
