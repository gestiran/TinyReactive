// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TinyReactive.Fields;

namespace TinyReactive.Extensions {
    /// <summary> List of extensions for <see cref="TinyReactive.Fields.ObservedList{T}">ObservedList</see>. </summary>
    public static class ObservedListExtension {
        /// <summary> Sorts the elements in the entire list using the default comparer. </summary>
        /// <param name="list"> ObservedList with IComparable interface implementation. </param>
        /// <typeparam name="T"> Any type with IComparable interface implementation. </typeparam>
        public static void Sort<T>(this ObservedList<T> list) where T : IComparable<T> => list.list.Sort();
        
        /// <summary> Convert <see cref="TinyReactive.Fields.ObservedList{T}">ObservedList</see> to array. </summary>
        /// <param name="list"> Any ObservedList object. </param>
        /// <typeparam name="T"> ObservedList object type. </typeparam>
        /// <returns> New array. </returns>
        [Pure]
        public static T[] ToArray<T>(this ObservedList<T> list) {
            T[] result = new T[list.Count];
            list.list.CopyTo(result, 0);
            return result;
        }
        
        /// <summary> Convert <see cref="TinyReactive.Fields.ObservedList{T}">ObservedList</see> to <see cref="System.Collections.Generic.List{T}">List</see>. </summary>
        /// <param name="list"> Any ObservedList object. </param>
        /// <typeparam name="T"> ObservedList object type. </typeparam>
        /// <returns> New list. </returns>
        [Pure]
        public static List<T> ToList<T>(this ObservedList<T> list) {
            List<T> result = new List<T>(list.Count);
            result.AddRange(list.list);
            return result;
        }
    }
}