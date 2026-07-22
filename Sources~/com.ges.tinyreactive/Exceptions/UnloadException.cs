// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Exceptions {
    /// <summary> Error unloading the object. </summary>
    public sealed class UnloadException : Exception {
        public UnloadException(string message, Exception innerException) : base(message, innerException) { }
    }
}