using System;
using System.Collections.Generic;

namespace IEnumerableExtras
{
    /// <summary>
    /// <see cref="IEnumerable{T}"/> reduce extension.
    /// </summary>
    public static class EnumerableReduce
    {
        /// <summary>
        /// Reduce this instance of <see cref="IEnumerable{TValue}"/> to a single value of
        /// type <typeparamref name="TValue"/> using given predicate.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TValue Reduce< TValue >( this IEnumerable<TValue> collection,
                                               Func<TValue, TValue, TValue> predicate )
        {
            bool first = true;
            TValue result = default( TValue );

            foreach ( TValue value in collection )
            {
                if ( first )
                {
                    result = value;
                    first = false;
                }
                else
                {
                    result = predicate( result, value );
                }
            }

            return result;
        }

        /// <summary>
        /// Reduce this instance of <see cref="IEnumerable{TValue}"/> to a single value of
        /// type <typeparamref name="TResult"/> using given predicate and an initial value
        /// of type <typeparamref name="TResult"/>.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="collection"></param>
        /// <param name="init"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TResult Reduce< TValue, TResult >( this IEnumerable<TValue> collection, TResult init,
                                                         Func<TResult, TValue, TResult> predicate )
        {
            foreach ( TValue value in collection )
            {
                init = predicate( init, value );
            }

            return init;
        }
    }
}