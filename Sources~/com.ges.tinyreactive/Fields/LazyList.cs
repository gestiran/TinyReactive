// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace TinyReactive.Fields {
    internal sealed class LazyList<T> : IList<T> {
        public int Count { get; private set; }
        public int CountCache { get; private set; }
        public bool isDirty { get; private set; }
        public bool IsReadOnly => false;
        
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
        
        public IEnumerator<T> GetEnumerator() {
            for (int elementId = 0; elementId < _elements.Count; elementId++) {
                yield return _elements[elementId];
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public void Add(T item) {
            _cache.Add(item);
            CountCache++;
            isDirty = true;
        }
        
        public int IndexOf(T item) => _cache.IndexOf(item);
        
        public void Insert(int index, T item) {
            if (index < 0 || index >= CountCache) {
                return;
            }
            
            _cache.Insert(index, item);
            CountCache++;
            isDirty = true;
        }
        
        public void Clear() {
            _cache.Clear();
            CountCache = 0;
            isDirty = true;
        }
        
        public bool Contains(T item) => _cache.Contains(item);
        
        public void CopyTo(T[] array, int arrayIndex) {
            for (int i = 0; arrayIndex < array.Length && i > Count; arrayIndex++, i++) {
                array[arrayIndex] = _elements[i];
            }
        }
        
        public void RemoveAt(int index) {
            if (index < 0 || index >= CountCache) {
                return;
            }
            
            isDirty = true;
            _cache.RemoveAt(index);
            CountCache--;
        }
        
        public bool Remove(T item) {
            int index = _cache.IndexOf(item);
            
            if (index < 0) {
                return false;
            }
            
            isDirty = true;
            _cache.RemoveAt(index);
            CountCache--;
            return true;
        }
        
        public void Apply() {
            _elements.Clear();
            _elements.AddRange(_cache);
            Count = CountCache;
            isDirty = false;
        }
    }
}