// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

namespace TinyReactive.Fields {
    public delegate IEnumerable<T> CompositionListener<out T>();
    
    public delegate IEnumerable<TResult> CompositionListener<in T, out TResult>(T value);
    
    public delegate IEnumerable<TResult> CompositionListener<in T1, in T2, out TResult>(T1 value1, T2 value2);
    
    public delegate IEnumerable<TResult> CompositionListener<in T1, in T2, in T3, out TResult>(T1 value1, T2 value2, T3 value3);
}