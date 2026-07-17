// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TinyReactive {
    /// <summary> Pool of resources requiring unloading, for unsubscribing from events and stopping asynchronous actions in case of scene unloading. </summary>
    public sealed class UnloadPool : IUnload, IUnloadLink {
        /// <summary>
        /// Pool lifetime status. Becomes True after <see cref="TinyReactive.IUnload.Unload">Unload</see>,
        /// resets to False after <see cref="Clear">Clear</see>.
        /// </summary>
        public bool isUnloaded { get; private set; }
        
        /// <summary> List of abstract references for unloading. </summary>
        private readonly List<IUnload> _pool;
        
        /// <summary> Create empty unload pool. </summary>
        /// <param name="capacity"> References an unload <see cref="System.Collections.Generic.List{T}">List</see> capacity. </param>
        public UnloadPool(int capacity = 4) => _pool = new List<IUnload>(capacity);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Add() { }
        
        public T Add<T>([NotNull] T unload) where T : IUnload {
            _pool.Add(unload);
            return unload;
        }
        
        /// <summary> Add an object to the Pool. </summary>
        /// <param name="unloads"> Any unload objects. </param>
        public void Add([NotNull] params IUnload[] unloads) => _pool.AddRange(unloads);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Remove() { }
        
        /// <summary> Remove the object from the pool. </summary>
        /// <param name="unload"> Any unload object. </param>
        /// <typeparam name="T"> Any unload object type. </typeparam>
        /// <returns> Current object. </returns>
        public T Remove<T>([NotNull] T unload) where T : IUnload {
            _pool.Remove(unload);
            return unload;
        }
        
        /// <summary> Remove objects from the pool. </summary>
        /// <param name="unloads"> Any unload objects. </param>
        public void Remove([NotNull] params IUnload[] unloads) {
            for (int i = 0; i < unloads.Length; i++) {
                _pool.Remove(unloads[i]);
            }
        }
        
        /// <summary> Remove all objects from the pool and reset the unloaded status. </summary>
        public void Clear() {
            _pool.Clear();
            isUnloaded = false;
        }
        
        /// <summary> Call <see cref="TinyReactive.IUnload.Unload">Unload</see> on all objects. </summary>
        public void Unload() {
            int count = _pool.Count;
            
            for (int i = 0; i < count; i++) {
                _pool[i].Unload();
            }
            
            isUnloaded = true;
        }
    }
}