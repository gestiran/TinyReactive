// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TinyReactive {
    /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/UnloadPool/*" />
    public sealed class UnloadPool : IUnload, IUnloadLink {
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/fields/isUnloaded/*" />
        public bool isUnloaded { get; private set; }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/fields/_pool/*" />
        private readonly List<IUnload> _pool;
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/constructor/main/*" />
        public UnloadPool(int capacity = 4) => _pool = new List<IUnload>(capacity);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Add() { }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/methods/Add_0/*" />
        public T Add<T>([NotNull] T unload) where T : IUnload {
            _pool.Add(unload);
            return unload;
        }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/methods/Add_1/*" />
        public void Add([NotNull] params IUnload[] unloads) => _pool.AddRange(unloads);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Remove() { }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/methods/Remove_0/*" />
        public T Remove<T>([NotNull] T unload) where T : IUnload {
            _pool.Remove(unload);
            return unload;
        }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/methods/Remove_1/*" />
        public void Remove([NotNull] params IUnload[] unloads) {
            for (int i = 0; i < unloads.Length; i++) {
                _pool.Remove(unloads[i]);
            }
        }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/methods/Clear/*" />
        public void Clear() {
            _pool.Clear();
            isUnloaded = false;
        }
        
        /// <include file="../Documentation~/Scripts/UnloadPool.xml" path="docs/methods/Unload/*" />
        public void Unload() {
            int count = _pool.Count;
            
            for (int i = 0; i < count; i++) {
                _pool[i].Unload();
            }
            
            isUnloaded = true;
        }
    }
}