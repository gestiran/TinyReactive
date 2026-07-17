// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    /// <summary> Wrapper for returning a specific type of value. </summary>
    /// <typeparam name="T"> Return type. </typeparam>
    public interface IValue<out T> {
        /// <summary> Target type. </summary>
        public T value { get; }
    }
}