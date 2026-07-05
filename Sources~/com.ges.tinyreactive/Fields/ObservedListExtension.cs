// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;

namespace TinyReactive.Fields {
    public static class ObservedListExtension {
        public static void Sort<T>(this ObservedList<T> list) where T : IComparable<T> {
            list.list.Sort();
        }
        
        public static void Sort<T>(this ObservedList<T> list, [NotNull] Comparison<T> comparison) {
            list.list.Sort(comparison);
        }
        
        [Pure]
        public static T[] ToArray<T>(this ObservedList<T> list) {
            T[] result = new T[list.Count];
            list.list.CopyTo(result, 0);
            return result;
        }
        
        [Pure]
        public static List<T> ToList<T>(this ObservedList<T> list) {
            List<T> result = new List<T>(list.Count);
            result.AddRange(list.list);
            return result;
        }
    }
}