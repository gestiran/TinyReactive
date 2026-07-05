// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace TinyReactive {
    public sealed class UnloadPool : IUnload, IUnloadLink {
        public bool isUnloaded { get; private set; }
        
        private readonly List<IUnload> _pool;
        
        public UnloadPool(int capacity = 4) => _pool = new List<IUnload>(capacity);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Add() { }
        
        public T Add<T>([NotNull] T unload) where T : IUnload {
            _pool.Add(unload);
            return unload;
        }
        
        public void Add([NotNull] params IUnload[] unloads) => _pool.AddRange(unloads);
        
        [Obsolete("Can`t use without parameters.", true)]
        public void Remove() { }
        
        public T Remove<T>([NotNull] T unload) where T : IUnload {
            _pool.Remove(unload);
            return unload;
        }
        
        public void Remove([NotNull] params IUnload[] unloads) {
            for (int i = 0; i < unloads.Length; i++) {
                _pool.Remove(unloads[i]);
            }
        }
        
        public void Clear() {
            _pool.Clear();
            isUnloaded = false;
        }
        
        public void Unload() {
            int count = _pool.Count;
            
            for (int i = 0; i < count; i++) {
                _pool[i].Unload();
            }
            
            isUnloaded = true;
        }
    }
}