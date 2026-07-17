// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyReactive.Fields {
    public class ObservedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEquatable<ObservedDictionary<TKey, TValue>> {
        public int Count { get; private set; }
        public ICollection<TKey> Keys => dictionary.Keys;
        public ICollection<TValue> Values => dictionary.Values;
        public bool IsReadOnly => false;
        
        internal readonly int id;
        internal readonly LazyList<ActionListener> onAdd;
        internal readonly LazyList<ActionListener<TValue>> onAddWithValue;
        internal readonly LazyList<ActionListener> onRemove;
        internal readonly LazyList<ActionListener<TValue>> onRemoveWithValue;
        internal readonly Dictionary<TKey, TValue> dictionary;
        
        public ObservedDictionary(int capacity = Observed.CAPACITY) : this(new Dictionary<TKey, TValue>(capacity)) { }
        
        public ObservedDictionary(Dictionary<TKey, TValue> value, int capacity = Observed.CAPACITY) {
            dictionary = value;
            id = Observed.GetID();
            onAdd = new LazyList<ActionListener>(capacity);
            onAddWithValue = new LazyList<ActionListener<TValue>>(capacity);
            onRemove = new LazyList<ActionListener>(capacity);
            onRemoveWithValue = new LazyList<ActionListener<TValue>>(capacity);
            Count = value.Count;
        }
        
        public virtual TValue this[TKey key] {
            get => dictionary[key];
            set {
                if (dictionary.TryGetValue(key, out TValue current)) {
                    if (onRemove.isDirty) {
                        onRemove.Apply();
                    }
                    
                    if (onRemoveWithValue.isDirty) {
                        onRemoveWithValue.Apply();
                    }
                    
                    for (int i = 0; i < onRemove.Count; i++) {
                        onRemove[i].Invoke();
                    }
                    
                    for (int i = 0; i < onRemoveWithValue.Count; i++) {
                        onRemoveWithValue[i].Invoke(current);
                    }   
                }
                
                dictionary[key] = value;
                
                if (onAdd.isDirty) {
                    onAdd.Apply();
                }
                
                if (onAddWithValue.isDirty) {
                    onAddWithValue.Apply();
                }
                
                for (int i = 0; i < onAdd.Count; i++) {
                    onAdd[i].Invoke();
                }
                
                for (int i = 0; i < onAddWithValue.Count; i++) {
                    onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        
        public virtual void Add(TKey key, TValue value) {
            if (dictionary.TryAdd(key, value)) {
                Count++;
                
                if (onAdd.isDirty) {
                    onAdd.Apply();
                }
                
                if (onAddWithValue.isDirty) {
                    onAddWithValue.Apply();
                }
                
                for (int i = 0; i < onAdd.Count; i++) {
                    onAdd[i].Invoke();
                }
                
                for (int i = 0; i < onAddWithValue.Count; i++) {
                    onAddWithValue[i].Invoke(value);
                }
            }
        }
        
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        
        public virtual bool Remove(TKey key) {
            if (dictionary.Remove(key, out TValue value)) {
                Count--;
                
                if (onRemove.isDirty) {
                    onRemove.Apply();
                }
                
                if (onRemoveWithValue.isDirty) {
                    onRemoveWithValue.Apply();
                }
                
                for (int i = 0; i < onRemove.Count; i++) {
                    onRemove[i].Invoke();
                }
                
                for (int i = 0; i < onRemoveWithValue.Count; i++) {
                    onRemoveWithValue[i].Invoke(value);
                }
                
                return true;
            }
            
            return false;
        }
        
        public virtual void RemoveRange(List<TValue> values) {
            KeyValuePair<TKey, TValue>[] dataPair = new KeyValuePair<TKey, TValue>[dictionary.Count];
            int dataId = 0;
            
            foreach (KeyValuePair<TKey, TValue> data in dictionary) {
                dataPair[dataId++] = data;
            }
            
            for (int valueId = 0; valueId < values.Count; valueId++) {
                TValue value = values[valueId];
                
                for (dataId = 0; dataId < dataPair.Length; dataId++) {
                    if (dataPair[dataId].Value.Equals(value)) {
                        dictionary.Remove(dataPair[dataId].Key);
                        Count--;
                        
                        if (onRemove.isDirty) {
                            onRemove.Apply();
                        }
                        
                        if (onRemoveWithValue.isDirty) {
                            onRemoveWithValue.Apply();
                        }
                        
                        for (int i = 0; i < onRemove.Count; i++) {
                            onRemove[i].Invoke();
                        }
                        
                        for (int i = 0; i < onRemoveWithValue.Count; i++) {
                            onRemoveWithValue[i].Invoke(value);
                        }
                        
                        break;
                    }
                    
                }
            }
        }
        
        public virtual void Clear() {
            dictionary.Clear();
            Count = 0;
        }
        
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return dictionary.TryGetValue(item.Key, out TValue value) && value != null && value.Equals(item.Value);
        }
        
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        
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
            
            foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
                array[arrayIndex++] = pair;
            }
        }
        
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
                yield return pair;
            }
        }
        
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener listener) {
            onAdd.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onAdd.Add(listener);
            unload.Add(new UnloadAction(() => onAdd.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener<TValue> listener) {
            onAddWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onAddWithValue.Remove(listener)));
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
            onAdd.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener<TValue> listener) {
            onAddWithValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener listener) {
            onRemove.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onRemove.Add(listener);
            unload.Add(new UnloadAction(() => onRemove.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener<TValue> listener) {
            onRemoveWithValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onRemoveWithValue.Remove(listener)));
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
            onRemove.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener<TValue> listener) {
            onRemoveWithValue.Remove(listener);
            return this;
        }
        
        public override int GetHashCode() => id;
        
        public bool Equals(ObservedDictionary<TKey, TValue> other) => other != null && other.id == id;
    }
}