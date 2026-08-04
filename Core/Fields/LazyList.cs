// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections;
using System.Collections.Generic;
// ReSharper disable InconsistentNaming

namespace TinyReactive.Fields {
    /// <summary> Internal list that defers synchronization until Apply is called, optimizing batch operations. </summary>
    /// <typeparam name="T"> The type of the stored elements. </typeparam>
    internal sealed class LazyList<T> : IList<T> {
        /// <summary> Gets the number of synchronized elements. </summary>
        public int Count { get; private set; }
        
        /// <summary> Gets the number of elements including pending changes. </summary>
        public int CountCache { get; private set; }
        
        /// <summary> Gets a value indicating whether the cache has un-applied changes. </summary>
        public bool isDirty { get; private set; }
        
        /// <summary> Always returns False. </summary>
        public bool IsReadOnly => false;
        
        /// <summary> Internal list storing the synchronized elements. </summary>
        private readonly List<T> _elements;
        
        /// <summary> Internal list storing the pending changes. </summary>
        private readonly List<T> _cache;
        
        /// <summary> Gets or sets the element at the specified index in the synchronized list. </summary>
        /// <param name="index"> The zero-based index of the element to get or set. </param>
        public T this[int index] {
            get => _elements[index];
            set => _elements[index] = value;
        }
        
        /// <summary> Creates a new instance and initializes the internal lists. </summary>
        /// <param name="capacity"> Initial capacity of the internal lists. </param>
        public LazyList(int capacity = 64) {
            _elements = new List<T>(capacity);
            _cache = new List<T>(capacity);
        }
        
        /// <summary> Returns an enumerator that iterates through the synchronized elements. </summary>
        /// <returns> An enumerator for the synchronized elements. </returns>
        public IEnumerator<T> GetEnumerator() {
            for (int elementId = 0; elementId < _elements.Count; elementId++) {
                yield return _elements[elementId];
            }
        }
        
        /// <summary> Returns an enumerator that iterates through the synchronized elements. </summary>
        /// <returns> An enumerator for the synchronized elements. </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary> Adds an element to the pending cache and marks the list as dirty. </summary>
        /// <param name="item"> The element to add. </param>
        public void Add(T item) {
            _cache.Add(item);
            CountCache++;
            isDirty = true;
        }
        
        /// <summary> Determines the index of a specific element in the pending cache. </summary>
        /// <param name="item"> The object to locate in the cache. </param>
        /// <returns> The index of the element if found; otherwise, -1. </returns>
        public int IndexOf(T item) => _cache.IndexOf(item);
        
        /// <summary> Inserts an element into the pending cache at the specified index and marks the list as dirty. </summary>
        /// <param name="index"> The zero-based index at which the element should be inserted. </param>
        /// <param name="item"> The element to insert. </param>
        public void Insert(int index, T item) {
            if (index < 0 || index >= CountCache) {
                return;
            }
            
            _cache.Insert(index, item);
            CountCache++;
            isDirty = true;
        }
        
        /// <summary> Clears the pending cache and marks the list as dirty. </summary>
        public void Clear() {
            _cache.Clear();
            CountCache = 0;
            isDirty = true;
        }
        
        /// <summary> Determines whether the pending cache contains a specific element. </summary>
        /// <param name="item"> The object to locate in the cache. </param>
        /// <returns> True if the element is found in the cache; otherwise, false. </returns>
        public bool Contains(T item) => _cache.Contains(item);
        
        /// <summary> Copies the synchronized elements to an array, starting at a particular array index. </summary>
        /// <param name="array"> The one-dimensional array that is the destination of the elements. </param>
        /// <param name="arrayIndex"> The zero-based index in the array at which copying begins. </param>
        public void CopyTo(T[] array, int arrayIndex) {
            for (int i = 0; arrayIndex < array.Length && i > Count; arrayIndex++, i++) {
                array[arrayIndex] = _elements[i];
            }
        }
        
        /// <summary> Removes the element at the specified index from the pending cache and marks the list as dirty. </summary>
        /// <param name="index"> The zero-based index of the element to remove. </param>
        public void RemoveAt(int index) {
            if (index < 0 || index >= CountCache) {
                return;
            }
            
            isDirty = true;
            _cache.RemoveAt(index);
            CountCache--;
        }
        
        /// <summary> Removes the first occurrence of a specific element from the pending cache and marks the list as dirty. </summary>
        /// <param name="item"> The element to remove. </param>
        /// <returns> True if the element was successfully removed; otherwise, false. </returns>
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
        
        /// <summary> Synchronizes the pending cache to the main list and clears the dirty flag. </summary>
        public void Apply() {
            _elements.Clear();
            _elements.AddRange(_cache);
            Count = CountCache;
            isDirty = false;
        }
    }
}