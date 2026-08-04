// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    /// <summary>
    /// Used in <see cref="InputChanger{T}">InputChanger</see>
    /// to modify an unmanaged value by passing it by reference.
    /// </summary>
    /// <typeparam name="T"> Any unmanaged type. </typeparam>
    public delegate void ValueChanger<T>(ref T value) where T : unmanaged;
}