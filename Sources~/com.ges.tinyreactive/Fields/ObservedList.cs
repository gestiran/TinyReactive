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
#if EXTERNAL_DEPENDENCIES
    [JsonConverter(typeof(ObservedListJsonConverter))]
#endif
    public class ObservedList<T> : IList<T>, IEnumerator<T>, IEquatable<ObservedList<T>> {
        public int Count => list.Count;
        public T Current => list[_currentId];
        object IEnumerator.Current => list[_currentId];
        public bool IsReadOnly => false;
        
        internal readonly int id;
        internal readonly LazyList<ActionListener> onAdd;
        internal readonly LazyList<ActionListener<T>> onAddWithValue;
        internal readonly LazyList<ActionListener> onRemove;
        internal readonly LazyList<ActionListener<T>> onRemoveWithValue;
        internal readonly LazyList<ActionListener> onClear;
        
        internal List<T> list;
        
        private int _currentId;
        
        public ObservedList(int capacity = Observed.CAPACITY) : this(new List<T>(), capacity) { }
        
        public ObservedList(T[] value, int capacity = Observed.CAPACITY) : this(value.ToList(), capacity) { }
        
        public ObservedList(List<T> value, int capacity = Observed.CAPACITY) {
            list = value;
            id = Observed.globalId++;
            onAdd = new LazyList<ActionListener>(capacity);
            onAddWithValue = new LazyList<ActionListener<T>>(capacity);
            onRemove = new LazyList<ActionListener>(capacity);
            onRemoveWithValue = new LazyList<ActionListener<T>>(capacity);
            onClear = new LazyList<ActionListener>(capacity);
            _currentId = -1;
        }
        
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
        
        [Obsolete("Can't add nothing!", true)]
        public void Add() {
            // Do nothing
        }
        
        public virtual void Add([NotNull] params T[] values) {
            list.AddRange(values);
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                onAdd.Invoke();
                onAddWithValue.Invoke(values[valueId]);
            }
        }
        
        public virtual void Add([NotNull] T value) {
            list.Add(value);
            onAdd.Invoke();
            onAddWithValue.Invoke(value);
        }
        
        [Obsolete("Can't remove nothing!", true)]
        public void Remove() {
            // Do nothing
        }
        
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
        
        public virtual void RemoveAll() {
            for (int i = Count - 1; i >= 0; i--) {
                T value = list[i];
                list.RemoveAt(i);
                onRemove.Invoke();
                onRemoveWithValue.Invoke(value);
            }
        }
        
        public virtual void Clear() {
            list.Clear();
            onClear.Invoke();
        }
        
        public int IndexOf(T element) => list.IndexOf(element);
        
        public virtual void Insert(int index, T item) {
            list.Insert(index, item);
            onAdd.Invoke();
            onAddWithValue.Invoke(item);
        }
        
        public bool Contains(T element) => list.Contains(element);
        
        public void CopyTo(T[] array, int arrayIndex) => list.CopyTo(array, arrayIndex);
        
        public virtual void RemoveAt(int index) {
            T element = list[index];
            list.RemoveAt(index);
            onRemove.Invoke();
            onAddWithValue.Invoke(element);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener(ActionListener listener) {
            onAdd.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onAdd.Add(listener);
            unload.Add(new UnloadAction(() => onAdd.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener(ActionListener<T> listener) {
            onAddWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnAddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onAddWithValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnAddListener(ActionListener listener) {
            onAdd.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnAddListener(ActionListener<T> listener) {
            onAddWithValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener(ActionListener listener) {
            onRemove.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onRemove.Add(listener);
            unload.Add(new UnloadAction(() => onRemove.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener(ActionListener<T> listener) {
            onRemoveWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnRemoveListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onRemoveWithValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnRemoveListener(ActionListener listener) {
            onRemove.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnRemoveListener(ActionListener<T> listener) {
            onRemoveWithValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnClearListener(ActionListener listener) {
            onClear.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> AddOnClearListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onClear.Add(listener);
            unload.Add(new UnloadAction(() => onClear.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedList<T> RemoveOnClearListener(ActionListener listener) {
            onClear.Remove(listener);
            return this;
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        public IEnumerator<T> GetEnumerator() {
            foreach (T value in list) {
                yield return value;
            }
        }
        
        public bool MoveNext() {
            _currentId++;
            return _currentId < list.Count;
        }
        
        public void Reset() => _currentId = -1;
        
        public void Dispose() {
            Reset();
            list = null;
            onAdd.Clear();
            onAddWithValue.Clear();
            onRemove.Clear();
            onRemoveWithValue.Clear();
            onClear.Clear();
        }
        
        public override string ToString() => $"ObservedList<{typeof(T).Name}>";
        
        public override int GetHashCode() => id;
        
        public bool Equals(ObservedList<T> other) => other != null && other.id == id;
    }
}