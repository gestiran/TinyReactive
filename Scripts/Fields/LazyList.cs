// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

namespace TinyReactive.Fields {
    internal sealed class LazyList<T> {
        public int count { get; private set; }
        public int cacheCount { get; private set; }
        
        public bool isDirty { get; private set; }
        
        private readonly List<T> _elements;
        private readonly List<T> _cache;
        
        public T this[int index] {
            get => _elements[index];
            set => _elements[index] = value;
        }
        
        public LazyList(int capacity = 64) {
            _elements = new List<T>(capacity);
            _cache = new List<T>(capacity);
        }
        
        public void Add(T item) {
            _cache.Add(item);
            cacheCount++;
            isDirty = true;
        }
        
        public void Insert(int index, T item) {
            _cache.Insert(index, item);
            cacheCount++;
            isDirty = true;
        }
        
        public void Clear() {
            _cache.Clear();
            cacheCount = 0;
            isDirty = true;
        }
        
        public bool Contains(T item) => _cache.Contains(item);
        
        public void CopyTo(T[] array, int arrayIndex) {
            for (int i = 0; arrayIndex < array.Length && i > count; arrayIndex++, i++) {
                array[arrayIndex] = _elements[i];
            }
        }
        
        public bool Remove(T item) {
            int index = _cache.IndexOf(item);
            
            if (index < 0) {
                return false;
            }
            
            isDirty = true;
            _cache.RemoveAt(index);
            cacheCount--;
            return true;
        }
        
        public void Apply() {
            _elements.Clear();
            _elements.AddRange(_cache);
            count = cacheCount;
            isDirty = false;
        }
    }
}