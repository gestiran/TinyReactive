// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive {
    /// <summary> Designates the object as a pool for <see cref="TinyReactive.IUnload">unloading</see> objects. </summary>
    public interface IUnloadLink {
        /// <summary> Add an object to the Pool. </summary>
        /// <param name="unload"> Any unload object. </param>
        /// <typeparam name="T"> Any unload object type. </typeparam>
        /// <returns> Current object. </returns>
        public T Add<T>(T unload) where T : IUnload;
    }
}