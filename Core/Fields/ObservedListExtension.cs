// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace TinyReactive.Fields {
    /// <summary> Extension methods for <see cref="ObservedList{T}">ObservedList</see>. </summary>
    public static class ObservedListExtension {
        /// <summary> Invokes the sort method on the included list. </summary>
        /// <param name="list"> Target list. </param>
        /// <typeparam name="T"> Comparable type. </typeparam>
        public static void Sort<T>(this ObservedList<T> list) where T : IComparable<T> {
            list.list.Sort();
        }
        
        /// <summary> Invokes the sort method on the included list. </summary>
        /// <param name="list"> Target list. </param>
        /// <param name="comparison"></param>
        /// <typeparam name="T"> Any object type. </typeparam>
        public static void Sort<T>(this ObservedList<T> list, [NotNull] Comparison<T> comparison) {
            list.list.Sort(comparison);
        }
        
        /// <summary> Converts values into an array. </summary>
        /// <param name="list"> Target list. </param>
        /// <typeparam name="T"> Any object type. </typeparam>
        /// <returns> The resulting array. </returns>
        [Pure]
        public static T[] ToArray<T>(this ObservedList<T> list) {
            T[] result = new T[list.Count];
            list.list.CopyTo(result, 0);
            return result;
        }
        
        /// <summary> Converts values into a list. </summary>
        /// <param name="list"> Target list. </param>
        /// <typeparam name="T"> Any object type. </typeparam>
        /// <returns> The resulting list. </returns>
        [Pure]
        public static List<T> ToList<T>(this ObservedList<T> list) {
            List<T> result = new List<T>(list.Count);
            result.AddRange(list.list);
            return result;
        }
    }
}