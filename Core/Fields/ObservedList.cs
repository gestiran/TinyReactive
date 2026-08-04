// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

#if EXTERNAL_DEPENDENCIES
using System.Text.Json.Serialization;
using TinyReactive.JsonConverters;
#endif

namespace TinyReactive.Fields {
    /// <summary> Observable list, allows subscribing to add, remove, and clear operations. </summary>
    /// <typeparam name="T"> The type of the stored elements. </typeparam>
#if EXTERNAL_DEPENDENCIES
    [JsonConverter(typeof(ObservedListJsonConverter))]
#endif
    public class ObservedList<T> : IList<T>, IEnumerator<T>, IEquatable<ObservedList<T>>, IUnload {
        /// <summary> Gets the number of elements contained in the list. </summary>
        public int Count => list.Count;
        
        /// <summary> Gets the element in the collection at the current position of the enumerator. </summary>
        public T Current => list[_currentId];
        
        /// <summary> Gets the element in the collection at the current position of the enumerator. </summary>
        object IEnumerator.Current => list[_currentId];
        
        /// <summary> Always returns False. </summary>
        public bool IsReadOnly => false;
        
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on add operation. </summary>
        internal readonly LazyList<ActionListener> onAdd;
        
        /// <summary> List of listeners that receive the added value on add operation. </summary>
        internal readonly LazyList<ActionListener<T>> onAddWithValue;
        
        /// <summary> List of parameterless listeners invoked on remove operation. </summary>
        internal readonly LazyList<ActionListener> onRemove;
        
        /// <summary> List of listeners that receive the removed value on remove operation. </summary>
        internal readonly LazyList<ActionListener<T>> onRemoveWithValue;
        
        /// <summary> List of parameterless listeners invoked on clear operation. </summary>
        internal readonly LazyList<ActionListener> onClear;
        
        /// <summary> Internal list storing the elements. </summary>
        internal List<T> list;
        
        /// <summary> Current index for the enumerator. </summary>
        private int _currentId;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public ObservedList(int capacity = Observed.CAPACITY) : this(new List<T>(), capacity) { }
        
        /// <summary> Creates a new instance from an array and initializes the listener lists. </summary>
        /// <param name="value"> The initial elements. </param>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public ObservedList(T[] value, int capacity = Observed.CAPACITY) : this(value.ToList(), capacity) { }
        
        /// <summary> Creates a new instance from a list and initializes the listener lists. </summary>
        /// <param name="value"> The initial elements. </param>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public ObservedList(List<T> value, int capacity = Observed.CAPACITY) {
            list = value;
            id = Observed.GetID();
            onAdd = new LazyList<ActionListener>(capacity);
            onAddWithValue = new LazyList<ActionListener<T>>(capacity);
            onRemove = new LazyList<ActionListener>(capacity);
            onRemoveWithValue = new LazyList<ActionListener<T>>(capacity);
            onClear = new LazyList<ActionListener>(capacity);
            _currentId = -1;
        }
        
        /// <summary> Gets or sets the element at the specified index. Setting a value notifies listeners. </summary>
        /// <param name="index"> The zero-based index of the element to get or set. </param>
        public virtual T this[int index] {
            get => list[index];
            set {
                T current = list[index];
                list[index] = value;
                onRemove.Invoke();
                onRemoveWithValue.Invoke(current);
                onAdd.Invoke();
                onAddWithValue.Invoke(value);
            }
        }
        
        /// <summary> Adds an array of elements to the end of the list and notifies listeners. </summary>
        /// <param name="values"> The elements to add. </param>
        public virtual void Add([NotNull] params T[] values) {
            list.AddRange(values);
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                onAdd.Invoke();
                onAddWithValue.Invoke(values[valueId]);
            }
        }
        
        /// <summary> Adds an element to the end of the list and notifies listeners. </summary>
        /// <param name="value"> The element to add. </param>
        public virtual void Add([NotNull] T value) {
            list.Add(value);
            onAdd.Invoke();
            onAddWithValue.Invoke(value);
        }
        
        /// <summary> Removes the first occurrence of specific elements from the list and notifies listeners. </summary>
        /// <param name="values"> The elements to remove. </param>
        public virtual void Remove([NotNull] params T[] values) {
            foreach (T value in values) {
                int index = list.IndexOf(value);
                
                if (index >= 0) {
                    list.RemoveAt(index);
                    onRemove.Invoke();
                    onRemoveWithValue.Invoke(value);
                }
            }
        }
        
        /// <summary> Removes the first occurrence of a specific element from the list and notifies listeners. </summary>
        /// <param name="value"> The element to remove. </param>
        /// <returns> True if the element was successfully removed; otherwise, false. </returns>
        public virtual bool Remove([NotNull] T value) {
            int index = list.IndexOf(value);
            
            if (index >= 0) {
                list.RemoveAt(index);
                onRemove.Invoke();
                onRemoveWithValue.Invoke(value);
                return true;
            }
            
            return false;
        }
        
        /// <summary> Removes all elements individually and notifies listeners for each removal. </summary>
        public virtual void RemoveAll() {
            for (int i = Count - 1; i >= 0; i--) {
                T value = list[i];
                list.RemoveAt(i);
                onRemove.Invoke();
                onRemoveWithValue.Invoke(value);
            }
        }
        
        /// <summary> Removes all elements from the list and notifies listeners. </summary>
        public virtual void Clear() {
            list.Clear();
            onClear.Invoke();
        }
        
        /// <summary> Determines the index of a specific element in the list. </summary>
        /// <param name="element"> The object to locate in the list. </param>
        /// <returns> The index of the element if found; otherwise, -1. </returns>
        public int IndexOf(T element) => list.IndexOf(element);
        
        /// <summary> Inserts an element into the list at the specified index and notifies listeners. </summary>
        /// <param name="index"> The zero-based index at which the element should be inserted. </param>
        /// <param name="item"> The element to insert. </param>
        public virtual void Insert(int index, T item) {
            list.Insert(index, item);
            onAdd.Invoke();
            onAddWithValue.Invoke(item);
        }
        
        /// <summary> Determines whether the list contains a specific element. </summary>
        /// <param name="element"> The object to locate in the list. </param>
        /// <returns> True if the element is found in the list; otherwise, false. </returns>
        public bool Contains(T element) => list.Contains(element);
        
        /// <summary> Copies the elements of the list to an array, starting at a particular array index. </summary>
        /// <param name="array"> The one-dimensional array that is the destination of the elements. </param>
        /// <param name="arrayIndex"> The zero-based index in the array at which copying begins. </param>
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        
        /// <summary> Removes the element at the specified index of the list and notifies listeners. </summary>
        /// <param name="index"> The zero-based index of the element to remove. </param>
        public virtual void RemoveAt(int index) {
            T element = list[index];
            list.RemoveAt(index);
            onRemove.Invoke();
            onAddWithValue.Invoke(element);
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener(ActionListener listener) {
            onAdd.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onAdd.Add(listener);
            unload.Add(new UnloadAction(() => onAdd.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added, receiving the added value. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener(ActionListener<T> listener) {
            onAddWithValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added, receiving the added value. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onAddWithValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener from the list. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnAddListener(ActionListener listener) {
            onAdd.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener from the list. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnAddListener(ActionListener<T> listener) {
            onAddWithValue.Remove(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener(ActionListener listener) {
            onRemove.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onRemove.Add(listener);
            unload.Add(new UnloadAction(() => onRemove.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed, receiving the removed value. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener(ActionListener<T> listener) {
            onRemoveWithValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed, receiving the removed value. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onRemoveWithValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener for removing an item from the list. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnRemoveListener(ActionListener listener) {
            onRemove.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener for removing an item from the list. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnRemoveListener(ActionListener<T> listener) {
            onRemoveWithValue.Remove(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the list is cleared. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnClearListener(ActionListener listener) {
            onClear.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the list is cleared. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnClearListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onClear.Add(listener);
            unload.Add(new UnloadAction(() => onClear.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added list-clearing listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnClearListener(ActionListener listener) {
            onClear.Remove(listener);
            return this;
        }
        
        /// <summary> Returns an enumerator that iterates through the list. </summary>
        /// <returns> An enumerator for the list. </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary> Returns an enumerator that iterates through the list. </summary>
        /// <returns> An enumerator for the list. </returns>
        public IEnumerator<T> GetEnumerator() {
            foreach (T value in list) {
                yield return value;
            }
        }
        
        /// <summary> Advances the enumerator to the next element of the list. </summary>
        /// <returns> True if the next element exists. </returns>
        public bool MoveNext() {
            _currentId++;
            return _currentId < list.Count;
        }
        
        /// <summary> Resets the enumerator value. </summary>
        public void Reset() => _currentId = -1;
        
        /// <summary> Calls Reset and resets the enumerator value. </summary>
        public virtual void Dispose() => Reset();
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public virtual void Unload() {
            onAdd.Clear();
            onAddWithValue.Clear();
            onRemove.Clear();
            onRemoveWithValue.Clear();
            onClear.Clear();
        }
        
        /// <summary> Returns the string representation of the list type. </summary>
        public override string ToString() => $"ObservedList<{typeof(T).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(ObservedList<T> other) => other != null && other.id == id;
        
        [Obsolete("Can't add nothing!", true)]
        public void Add() { }
        
        [Obsolete("Can't remove nothing!", true)]
        public void Remove() { }
    }
}