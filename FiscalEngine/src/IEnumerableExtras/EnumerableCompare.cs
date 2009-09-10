using System;
using System.Collections.Generic;

namespace IEnumerableExtras
{
    /// <summary>
    /// Compare <see cref="IEnumerable{T}"/> extension.
    /// </summary>
    public static class EnumerableCompare
    {
        /// <summary>
        /// Compare this instance of <see cref="IEnumerable{TValue}"/> with another instance 
        /// if same type using equivalence function and produce a list of items present 
        /// in both collections and a list of items missing from second collection.
        /// </summary>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="missingFromOther"></param>
        /// <param name="equivFunc"></param>
        /// <returns></returns>
        public static IEnumerable<Pair<TValue>> Compare< TValue >( this IEnumerable<TValue> a,
                                                                   IEnumerable<TValue> b,
                                                                   out IEnumerable<TValue> missingFromOther,
                                                                   Func<TValue, TValue, bool> equivFunc )
        {
            List<Pair<TValue>> common = new List<Pair<TValue>>();
            List<TValue> missing = new List<TValue>();


            foreach ( TValue fromA in a )
            {
                bool found = false;

                foreach ( TValue fromB in b )
                {
                    if ( equivFunc( fromA, fromB ) )
                    {
                        common.Add( new Pair<TValue>( fromA, fromB ) );
                        found = true;
                        break;
                    }
                }

                if ( !found )
                {
                    missing.Add( fromA );
                }
            }

            missingFromOther = missing;
            return common;
        }
    }
}