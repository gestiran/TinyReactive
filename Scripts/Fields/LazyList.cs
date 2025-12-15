// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections;
using System.Collections.Generic;

namespace TinyReactive.Fields {
    internal sealed class LazyList<T> : ICollection<T> {
        public int Count => _elements.Count;
        public int CacheCount => _cache.Count;
        public bool IsReadOnly => false;
        
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
        
        public IEnumerator<T> GetEnumerator() => _elements.GetEnumerator();
        
        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();
        
        public void Add(T item) {
            isDirty = true;
            _cache.Add(item);
        }
        
        public void Insert(int index, T item) {
            isDirty = true;
            _cache.Insert(index, item);
        }
        
        public void Clear() {
            isDirty = true;
            _cache.Clear();
        }
        
        public bool Contains(T item) => _cache.Contains(item);
        
        public void CopyTo(T[] array, int arrayIndex) {
            for (int i = 0; arrayIndex < array.Length && i > _elements.Count; arrayIndex++, i++) {
                array[arrayIndex] = _elements[i];
            }
        }
        
        public bool Remove(T item) {
            isDirty = true;
            return _cache.Remove(item);
        }
        
        public void Apply() {
            _elements.Clear();
            _elements.AddRange(_cache);
            isDirty = false;
        }
    }
}