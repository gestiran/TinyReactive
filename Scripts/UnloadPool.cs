// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TinyReactive {
    /// <summary> A pool of resources requiring unloading, for unsubscribing from events and stopping asynchronous actions in case of scene unloading. </summary>
    public sealed class UnloadPool : IUnload {
        /// <summary> Pool lifetime status. Becomes True after Unload, resets to False after Clear. </summary>
        public bool isUnloaded { get; private set; }
        
        private readonly List<IUnload> _pool;
        
        public UnloadPool(int capacity = 4) => _pool = new List<IUnload>(capacity);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Add() { }
        
        /// <summary> Add an object to the Pool. </summary>
        /// <param name="unload"> Any unload object. </param>
        /// <typeparam name="T"> Any unload object type. </typeparam>
        /// <returns> Current object. </returns>
        public T Add<T>([NotNull] T unload) where T : IUnload {
            _pool.Add(unload);
            return unload;
        }
        
        /// <summary> Add objects to the pool. </summary>
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
            foreach (IUnload unload in unloads) {
                _pool.Remove(unload);
            }
        }
        
        /// <summary> Remove all objects from the pool and reset the unloaded status. </summary>
        public void Clear() {
            _pool.Clear();
            isUnloaded = false;
        }
        
        /// <summary> Call Unload on all objects. </summary>
        public void Unload() {
            foreach (IUnload unload in _pool) {
                unload.Unload();
            }
            
            isUnloaded = true;
        }
    }
    
    /// <summary> A pool of resources requiring unloading, for unsubscribing from events and stopping asynchronous actions in case of scene unloading. </summary>
    /// <typeparam name="T"> Any unload object type. </typeparam>
    public sealed class UnloadPool<T> : IUnload where T : IUnload {
        /// <summary> Pool lifetime status. Becomes True after Unload, resets to False after Clear. </summary>
        public bool isUnloaded { get; private set; }
        
        private readonly List<T> _pool;
        
        public UnloadPool(int capacity = 4) => _pool = new List<T>(capacity);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Add() { }
        
        /// <summary> Add an object to the Pool. </summary>
        /// <param name="unload"> Any unload object. </param>
        /// <returns> Current object. </returns>
        public T Add([NotNull] T unload) {
            _pool.Add(unload);
            return unload;
        }
        
        /// <summary> Add objects to the pool. </summary>
        /// <param name="unloads"> Any unload objects. </param>
        public void Add([NotNull] params T[] unloads) => _pool.AddRange(unloads);
        
        /// <summary> Remove the object from the pool. </summary>
        /// <param name="unload"> Any unload object. </param>
        /// <returns> Current object. </returns>
        public T Remove([NotNull] T unload) {
            _pool.Remove(unload);
            return unload;
        }
        
        /// <summary> Remove objects from the pool. </summary>
        /// <param name="unloads"> Any unload objects. </param>
        public void Remove([NotNull] params T[] unloads) {
            foreach (T unload in unloads) {
                _pool.Remove(unload);
            }
        }
        
        /// <summary> Remove all objects from the pool and reset the unloaded status. </summary>
        public void Clear() {
            _pool.Clear();
            isUnloaded = false;
        }
        
        /// <summary> Call Unload on all objects. </summary>
        public void Unload() {
            foreach (T unload in _pool) {
                unload.Unload();
            }
            
            isUnloaded = true;
        }
    }
}