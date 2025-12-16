// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using TinyReactive.Fields;

namespace TinyReactive.Extensions {
    public static class ObservedListExtension {
        public static void Sort<T>(this ObservedList<T> list) where T : IComparable<T> => list.list.Sort();
        
        public static T[] ToArray<T>(this ObservedList<T> list) {
            T[] result = new T[list.count];
            list.list.CopyTo(result, 0);
            return result;
        }
    }
}