// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    public delegate void ValueChanger<T>(ref T value) where T : unmanaged;
}