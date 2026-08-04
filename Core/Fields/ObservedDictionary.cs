// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections;
using System.Collections.Generic;

namespace TinyReactive.Fields {
    /// <summary> Observable dictionary, allows subscribing to add and remove operations. </summary>
    /// <typeparam name="TKey"> The type of the keys in the dictionary. </typeparam>
    /// <typeparam name="TValue"> The type of the values in the dictionary. </typeparam>
    public class ObservedDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IEquatable<ObservedDictionary<TKey, TValue>>, IUnload {
        /// <summary> Gets the number of elements contained in the dictionary. </summary>
        public int Count { get; private set; }
        
        /// <summary> Gets a collection containing the keys of the dictionary. </summary>
        public ICollection<TKey> Keys => dictionary.Keys;
        
        /// <summary> Gets a collection containing the values of the dictionary. </summary>
        public ICollection<TValue> Values => dictionary.Values;
        
        /// <summary> Always returns False. </summary>
        public bool IsReadOnly => false;
        
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on add operation. </summary>
        internal readonly LazyList<ActionListener> onAdd;
        
        /// <summary> List of listeners that receive the added value on add operation. </summary>
        internal readonly LazyList<ActionListener<TValue>> onAddWithValue;
        
        /// <summary> List of parameterless listeners invoked on remove operation. </summary>
        internal readonly LazyList<ActionListener> onRemove;
        
        /// <summary> List of listeners that receive the removed value on remove operation. </summary>
        internal readonly LazyList<ActionListener<TValue>> onRemoveWithValue;
        
        /// <summary> Internal dictionary storing the elements. </summary>
        internal readonly Dictionary<TKey, TValue> dictionary;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="capacity"> Initial capacity of the internal dictionary and listener lists. </param>
        public ObservedDictionary(int capacity = Observed.CAPACITY) : this(new Dictionary<TKey, TValue>(capacity)) { }
        
        /// <summary> Creates a new instance from a dictionary and initializes the listener lists. </summary>
        /// <param name="value"> The initial elements. </param>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public ObservedDictionary(Dictionary<TKey, TValue> value, int capacity = Observed.CAPACITY) {
            dictionary = value;
            id = Observed.GetID();
            onAdd = new LazyList<ActionListener>(capacity);
            onAddWithValue = new LazyList<ActionListener<TValue>>(capacity);
            onRemove = new LazyList<ActionListener>(capacity);
            onRemoveWithValue = new LazyList<ActionListener<TValue>>(capacity);
            Count = value.Count;
        }
        
        /// <summary> Gets or sets the value associated with the specified key. Setting a value notifies listeners. </summary>
        /// <param name="key"> The key of the value to get or set. </param>
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
        
        /// <summary> Adds an element with the provided key and value to the dictionary and notifies listeners. </summary>
        /// <param name="item"> The key-value pair to add. </param>
        public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
        
        /// <summary> Adds an element with the provided key and value to the dictionary and notifies listeners. </summary>
        /// <param name="key"> The key of the element to add. </param>
        /// <param name="value"> The value of the element to add. </param>
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
        
        /// <summary> Removes the element with the specified key from the dictionary and notifies listeners. </summary>
        /// <param name="item"> The key-value pair to remove. </param>
        /// <returns> True if the element is successfully removed; otherwise, false. </returns>
        public bool Remove(KeyValuePair<TKey, TValue> item) => Remove(item.Key);
        
        /// <summary> Removes the element with the specified key from the dictionary and notifies listeners. </summary>
        /// <param name="key"> The key of the element to remove. </param>
        /// <returns> True if the element is successfully removed; otherwise, false. </returns>
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
        
        /// <summary> Removes elements matching the specified values from the dictionary and notifies listeners. </summary>
        /// <param name="values"> The values to remove. </param>
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
        
        /// <summary> Removes all elements from the dictionary and resets the count. </summary>
        public virtual void Clear() {
            dictionary.Clear();
            Count = 0;
        }
        
        /// <summary> Determines whether the dictionary contains a specific key-value pair. </summary>
        /// <param name="item"> The key-value pair to locate in the dictionary. </param>
        /// <returns> True if the key-value pair is found; otherwise, false. </returns>
        public bool Contains(KeyValuePair<TKey, TValue> item) {
            return dictionary.TryGetValue(item.Key, out TValue value) && value != null && value.Equals(item.Value);
        }
        
        /// <summary> Gets the value associated with the specified key. </summary>
        /// <param name="key"> The key whose value to get. </param>
        /// <param name="value"> When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the value parameter. </param>
        /// <returns> True if the dictionary contains an element with the specified key; otherwise, false. </returns>
        public bool TryGetValue(TKey key, out TValue value) => dictionary.TryGetValue(key, out value);
        
        /// <summary> Determines whether the dictionary contains the specified key. </summary>
        /// <param name="key"> The key to locate in the dictionary. </param>
        /// <returns> True if the dictionary contains an element with the key; otherwise, false. </returns>
        public bool ContainsKey(TKey key) => dictionary.ContainsKey(key);
        
        /// <summary> Copies the elements of the dictionary to an array, starting at a particular array index. </summary>
        /// <param name="array"> The one-dimensional array that is the destination of the elements. </param>
        /// <param name="arrayIndex"> The zero-based index in the array at which copying begins. </param>
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
        
        /// <summary> Returns an enumerator that iterates through the dictionary. </summary>
        /// <returns> An enumerator for the dictionary. </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            foreach (KeyValuePair<TKey, TValue> pair in dictionary) {
                yield return pair;
            }
        }
        
        /// <summary> Returns an enumerator that iterates through the dictionary. </summary>
        /// <returns> An enumerator for the dictionary. </returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        
        /// <summary> Adds a listener that will be invoked when an element is added. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener listener) {
            onAdd.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onAdd.Add(listener);
            unload.Add(new UnloadAction(() => onAdd.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added, receiving the added value. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener<TValue> listener) {
            onAddWithValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is added, receiving the added value. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onAddWithValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the added value is of the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of TValue that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Adds a listener that will be called when the added value is of the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of TValue that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener listener) {
            onAdd.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener<TValue> listener) {
            onAddWithValue.Remove(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener listener) {
            onRemove.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            onRemove.Add(listener);
            unload.Add(new UnloadAction(() => onRemove.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed, receiving the removed value. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener<TValue> listener) {
            onRemoveWithValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when an element is removed, receiving the removed value. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => onRemoveWithValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the removed value is of the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of TValue that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Adds a listener that will be called when the removed value is of the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of TValue that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener listener) {
            onRemove.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener<TValue> listener) {
            onRemoveWithValue.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists and the internal dictionary. </summary>
        // Resharper disable Unity.ExpensiveCode
        public virtual void Unload() {
            onAdd.Clear();
            onAddWithValue.Clear();
            onRemove.Clear();
            onRemoveWithValue.Clear();
            dictionary.Clear();
        }
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(ObservedDictionary<TKey, TValue> other) => other != null && other.id == id;
    }
}