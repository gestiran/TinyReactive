// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    /// <summary> Used in all reactive objects as a reference to the listener method. </summary>
    public delegate void ActionListener();
    
    /// <summary> Used in all reactive objects as a reference to the listener method. </summary>
    /// <typeparam name="T"> Any object type. </typeparam>
    public delegate void ActionListener<in T>(T value);
    
    /// <summary> Used in all reactive objects as a reference to the listener method. </summary>
    /// <typeparam name="T1"> Any object type. </typeparam>
    /// <typeparam name="T2"> Any object type. </typeparam>
    public delegate void ActionListener<in T1, in T2>(T1 value1, T2 value2);
    
    /// <summary> Used in all reactive objects as a reference to the listener method. </summary>
    /// <typeparam name="T1"> Any object type. </typeparam>
    /// <typeparam name="T2"> Any object type. </typeparam>
    /// <typeparam name="T3"> Any object type. </typeparam>
    public delegate void ActionListener<in T1, in T2, in T3>(T1 value1, T2 value2, T3 value3);
}