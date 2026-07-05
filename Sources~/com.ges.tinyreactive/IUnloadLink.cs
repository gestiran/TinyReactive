// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive {
    public interface IUnloadLink {
        public T Add<T>(T unload) where T : IUnload;
    }
}