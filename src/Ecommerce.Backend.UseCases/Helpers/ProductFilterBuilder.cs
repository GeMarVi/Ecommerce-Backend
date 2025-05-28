using Ecommerce.BackEnd.Data.Models;
using System.Linq.Expressions;

namespace Ecommerce.BackEnd.UseCases.Helpers
{
    public static class ProductFilterBuilder
    {
        /// <summary>
        /// Builds a combined filter expression for the Product entity based on a dictionary of key-value filters.
        /// </summary>
        /// <param name="filters">Dictionary of filters where the key is the field name and the value is the expected value.</param>
        /// <returns>A composed expression to filter products based on the provided filters.</returns>
        public static Expression<Func<Product, bool>> Build(Dictionary<string, string> filters)
        {
            var combinedExpression = AlwaysTrue<Product>();

            foreach (var (key, value) in filters)
            {
                var filterExpression = GetExpressionForFilter(key, value);
                combinedExpression = combinedExpression.AndAlso(filterExpression);
            }

            return combinedExpression;
        }

        /// <summary>
        /// Returns an expression that corresponds to a specific filter key and value.
        /// </summary>
        private static Expression<Func<Product, bool>> GetExpressionForFilter(string key, string value)
        {
            return key switch
            {
                "Gender" => p => p.Gender == value,
                "Brand" => p => p.Brand == value,
                "TypeDeport" => p => p.TypeDeport == value,

                "NewsProducts" when bool.TryParse(value, out var isNew) && isNew =>
                    p => p.ProductStatus == "Nuevo Lanzamiento",

                "Discount" when bool.TryParse(value, out var hasDiscount) && hasDiscount =>
                    p => p.DiscountRate > 0,

                _ => AlwaysTrue<Product>()
            };
        }

        /// <summary>
        /// Combines two expressions using a logical AND operation.
        /// </summary>
        /// <typeparam name="T">The type of the entity being filtered.</typeparam>
        /// <param name="expr1">The first expression (accumulated).</param>
        /// <param name="expr2">The second expression (new condition to add).</param>
        /// <returns>A new expression combining both with logical AND.</returns>
        public static Expression<Func<T, bool>> AndAlso<T>(
            this Expression<Func<T, bool>> expr1,
            Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));

            var body = Expression.AndAlso(
                Expression.Invoke(expr1, parameter),
                Expression.Invoke(expr2, parameter)
            );

            return Expression.Lambda<Func<T, bool>>(body, parameter);
        }

        /// <summary>
        /// Returns a neutral expression that always evaluates to true.
        /// Used as a starting point for combining filter expressions.
        /// </summary>
        private static Expression<Func<T, bool>> AlwaysTrue<T>() => _ => true;
    }
}
