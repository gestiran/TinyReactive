// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyReactive.Fields {
    public class ObservedDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        public int Count { get; private set; }
        public ICollection<TKey> Keys => root.Keys;
        public ICollection<TValue> Values => root.Values;
        public bool IsReadOnly => false;
        
        internal readonly Dictionary<TKey, TValue> root;
        
        private readonly LazyList<ActionListener> _onAdd;
        private readonly LazyList<ActionListener<TValue>> _onAddWithValue;
        private readonly LazyList<ActionListener> _onRemove;
        private readonly LazyList<ActionListener<TValue>> _onRemoveWithValue;
        
        public ObservedDictionary(int capacity = Observed.CAPACITY) : this(new Dictionary<TKey, TValue>(capacity)) { }
        
        public ObservedDictionary(Dictionary<TKey, TValue> value, int capacity = Observed.CAPACITY) {
            root = value;
            _onAdd = new LazyList<ActionListener>(capacity);
            _onAddWithValue = new LazyList<ActionListener<TValue>>(capacity);
            _onRemove = new LazyList<ActionListener>(capacity);
            _onRemoveWithValue = new LazyList<ActionListener<TValue>>(capacity);
            Count = value.Count;
        }
        
        public TValue this[TKey key] {
            get => root[key];
            set {
                Remove(key);
                Add(key, value);
            }
        }
        
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        
        public void Add(TKey key, TValue value) {
            if (root.TryAdd(key, value)) {
                Count++;
                
                if (_onAdd.isDirty) {
                    _onAdd.Apply();
                }
                
                if (_onAddWithValue.isDirty) {
                    _onAddWithValue.Apply();
                }
                
                for (int i = 0; i < _onAdd.count; i++) {
                    _onAdd[i].Invoke();
                }
                
                for (int i = 0; i < _onAddWithValue.count; i++) {
                    _onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        
        public bool Remove(TKey key) {
            if (root.Remove(key, out TValue value)) {
                Count--;
                
                if (_onRemove.isDirty) {
                    _onRemove.Apply();
                }
                
                if (_onRemoveWithValue.isDirty) {
                    _onRemoveWithValue.Apply();
                }
                
                for (int i = 0; i < _onRemove.count; i++) {
                    _onRemove[i].Invoke();
                }
                
                for (int i = 0; i < _onRemoveWithValue.count; i++) {
                    _onRemoveWithValue[i].Invoke(value);
                }
                
                return true;
            }
            
            return false;
        }
        
        public void RemoveRange(List<TValue> values) {
            KeyValuePair<TKey, TValue>[] dataPair = new KeyValuePair<TKey, TValue>[root.Count];
            int dataId = 0;
            
            foreach (KeyValuePair<TKey, TValue> data in root) {
                dataPair[dataId++] = data;
            }
            
            for (int valueId = 0; valueId < values.Count; valueId++) {
                TValue value = values[valueId];
                
                for (dataId = 0; dataId < dataPair.Length; dataId++) {
                    if (dataPair[dataId].Value.Equals(value)) {
                        root.Remove(dataPair[dataId].Key);
                        Count--;
                        
                        if (_onRemove.isDirty) {
                            _onRemove.Apply();
                        }
                        
                        if (_onRemoveWithValue.isDirty) {
                            _onRemoveWithValue.Apply();
                        }
                        
                        for (int i = 0; i < _onRemove.count; i++) {
                            _onRemove[i].Invoke();
                        }
                        
                        for (int i = 0; i < _onRemoveWithValue.count; i++) {
                            _onRemoveWithValue[i].Invoke(value);
                        }
                        
                        break;
                    }
                    
                }
            }
        }
        
        public void Clear() {
            root.Clear();
            Count = 0;
        }
        
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return root.TryGetValue(item.Key, out TValue value) && value != null && value.Equals(item.Value);
        }
        
        public bool TryGetValue(TKey key, out TValue value) => root.TryGetValue(key, out value);
        
        public bool ContainsKey(TKey key) => root.ContainsKey(key);
        
        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            if (array == null) {
                throw new ArgumentNullException(nameof(array));
            }
            
            if (arrayIndex < 0) {
                throw new ArgumentOutOfRangeException(nameof(arrayIndex));
            }
            
            if (array.Length - arrayIndex < Count) {
                throw new ArgumentException("Destination array is not long enough.");
            }
            
            foreach (KeyValuePair<TKey, TValue> pair in root) {
                array[arrayIndex++] = pair;
            }
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            foreach (KeyValuePair<TKey, TValue> pair in root) {
                yield return pair;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener listener) {
            _onAdd.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            _onAdd.Add(listener);
            unload.Add(new UnloadAction(() => _onAdd.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener<TValue> listener) {
            _onAddWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            _onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onAddWithValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : TValue {
            AddOnAddListener(v =>
                             {
                                 if (v is TV) {
                                     listener.Invoke();
                                 }
                             },
                             unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : TValue {
            AddOnAddListener(v =>
                             {
                                 if (v is TV target) {
                                     listener.Invoke(target);
                                 }
                             },
                             unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener listener) {
            _onAdd.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener<TValue> listener) {
            _onAddWithValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener listener) {
            _onRemove.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            _onRemove.Add(listener);
            unload.Add(new UnloadAction(() => _onRemove.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener<TValue> listener) {
            _onRemoveWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            _onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onRemoveWithValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : TValue {
            AddOnRemoveListener(v =>
                                {
                                    if (v is TV) {
                                        listener.Invoke();
                                    }
                                },
                                unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : TValue {
            AddOnRemoveListener(v =>
                                {
                                    if (v is TV target) {
                                        listener.Invoke(target);
                                    }
                                },
                                unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener listener) {
            _onRemove.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener<TValue> listener) {
            _onRemoveWithValue.Remove(listener);
            return this;
        }
    }
}