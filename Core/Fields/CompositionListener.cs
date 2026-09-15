// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

namespace TinyReactive.Fields {
    /// <summary> Used in input compositions as a reference to a listener method that returns a sequence of values. </summary>
    /// <typeparam name="T"> The type of the returned values. </typeparam>
    public delegate IEnumerable<T> CompositionListener<out T>();
    
    /// <summary> Used in input compositions as a reference to a listener method with a single value. </summary>
    /// <typeparam name="T"> The type of the value. </typeparam>
    /// <typeparam name="TResult"> The type of the returned values. </typeparam>
    public delegate IEnumerable<TResult> CompositionListener<in T, out TResult>(T value);
    
    /// <summary> Used in input compositions as a reference to a listener method with two values. </summary>
    /// <typeparam name="T1"> The type of the first value. </typeparam>
    /// <typeparam name="T2"> The type of the second value. </typeparam>
    /// <typeparam name="TResult"> The type of the returned values. </typeparam>
    public delegate IEnumerable<TResult> CompositionListener<in T1, in T2, out TResult>(T1 value1, T2 value2);
    
    /// <summary> Used in input compositions as a reference to a listener method with three values. </summary>
    /// <typeparam name="T1"> The type of the first value. </typeparam>
    /// <typeparam name="T2"> The type of the second value. </typeparam>
    /// <typeparam name="T3"> The type of the third value. </typeparam>
    /// <typeparam name="TResult"> The type of the returned values. </typeparam>
    public delegate IEnumerable<TResult> CompositionListener<in T1, in T2, in T3, out TResult>(T1 value1, T2 value2, T3 value3);
}