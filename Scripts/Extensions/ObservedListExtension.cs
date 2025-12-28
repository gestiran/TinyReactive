// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TinyReactive.Fields;

namespace TinyReactive.Extensions {
    /// <summary> List of extensions for ObservedList. </summary>
    public static class ObservedListExtension {
        /// <summary> Sorts the elements in the entire list using the default comparer. </summary>
        /// <param name="list"> List with IComparable interface implementation. </param>
        /// <typeparam name="T"> Any type with IComparable interface implementation. </typeparam>
        public static void Sort<T>(this ObservedList<T> list) where T : IComparable<T> => list.list.Sort();
        
        /// <summary> Convert ObservedList to array. </summary>
        /// <param name="list"> Any ObservedList. </param>
        /// <typeparam name="T"> Any type. </typeparam>
        /// <returns> New array. </returns>
        [Pure]
        public static T[] ToArray<T>(this ObservedList<T> list) {
            T[] result = new T[list.count];
            list.list.CopyTo(result, 0);
            return result;
        }
        
        /// <summary> Convert ObservedList to System.List. </summary>
        /// <param name="list"> Any ObservedList. </param>
        /// <typeparam name="T"> Any type. </typeparam>
        /// <returns> New list. </returns>
        [Pure]
        public static List<T> ToList<T>(this ObservedList<T> list) {
            List<T> result = new List<T>(list.count);
            result.AddRange(list.list);
            return result;
        }
    }
}