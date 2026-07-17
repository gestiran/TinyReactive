// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

#if EXTERNAL_DEPENDENCIES
using System.Text.Json.Serialization;
using TinyReactive.JsonConverters;
#endif

namespace TinyReactive.Fields {
    /// <summary> Observable value T, allows subscribing to changes. </summary>
    /// <typeparam name="T"> The type of the stored value. </typeparam>
#if EXTERNAL_DEPENDENCIES
    [JsonConverter(typeof(ObservedJsonConverter))]
#endif
    public class Observed<T> : IValue<T>, IEquatable<Observed<T>>, IEquatable<T>, IUnload {
        /// <summary> Current value, can be changed via <see cref="Set"/> or <see cref="SetSilent"/>. </summary>
        public T value { get; protected internal set; }
        
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on any value change. </summary>
        internal readonly LazyList<ActionListener> listeners;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<ActionListener<T>> listenersValue;
        
        /// <summary> List of listeners that receive both the old and new value on change. </summary>
        internal readonly LazyList<ActionListener<T, T>> listenersChange;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="data"> The initial <see cref="value"/>. </param>
        /// <param name="capacity"> Initial capacity of the <see cref="System.Collections.Generic.List{T}">List</see>. </param>
        public Observed(T data = default, int capacity = Observed.CAPACITY) {
            value = data;
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T>>(capacity);
            listenersChange = new LazyList<ActionListener<T, T>>(capacity);
        }
        
        /// <summary> Sets a new value without notifying any listeners. </summary>
        public void SetSilent(T newValue) => value = newValue;
        
        /// <summary> Sets a new value and notifies all subscribed listeners. </summary>
        public virtual void Set(T newValue) {
            T current = value;
            value = newValue;
            listeners.Invoke();
            listenersValue.Invoke(newValue);
            listenersChange.Invoke(current, newValue);
        }
        
        /// <summary> Adds a listener that will be invoked when the value changes. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the value changes. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener<T> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the value changes. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener<T, T> listener) {
            listenersChange.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener<T, T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersChange.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the value changes to the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of T that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddListener(newValue =>
                        {
                            if (newValue is TV) {
                                listener.Invoke();
                            }
                        },
                        unload);
            
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the value changes to the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of T that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddListener(newValue =>
                        {
                            if (newValue is TV target) {
                                listener.Invoke(target);
                            }
                        },
                        unload);
            
            return this;
        }
        
        /// <summary> Adds the listener to the beginning of the list that will be called when the value changes. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst(ActionListener listener) {
            if (listeners.CountCache > 0) {
                listeners.Insert(0, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        /// <summary> Adds the listener to the beginning of the list that will be called when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds the listener to the beginning of the list that will be called when the value changes. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst(ActionListener<T> listener) {
            if (listenersValue.CountCache > 0) {
                listenersValue.Insert(0, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        /// <summary> Adds the listener to the beginning of the list that will be called when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener to the end of the list, which will be triggered when the value changes. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast(ActionListener listener) {
            if (listeners.CountCache > 0) {
                listeners.Insert(listeners.CountCache - 1, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        /// <summary> Adds a listener to the end of the list, which will be triggered when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener to the end of the list, which will be triggered when the value changes. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast(ActionListener<T> listener) {
            if (listenersValue.CountCache > 0) {
                listenersValue.Insert(listenersValue.CountCache - 1, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        /// <summary> Adds a listener to the end of the list, which will be triggered when the value changes. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener<T> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener<T, T> listener) {
            listenersChange.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        public virtual void Unload() {
            listeners.Clear();
            listenersValue.Clear();
            listenersChange.Clear();
        }
        
        /// <summary> Returns the current <see cref="value"/>. </summary>
        public static implicit operator T(Observed<T> observed) => observed.value;
        
        /// <summary> Returns the string representation of the current <see cref="value"/>. </summary>
        public override string ToString() => $"{value}";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by its <see cref="id"/>. </summary>
        public bool Equals(Observed<T> other) => other != null && other.id == id;
        
        /// <summary> Compares an object by <see cref="value"/>. </summary>
        public bool Equals(T other) => other != null && other.Equals(value);
        
        /// <summary> Compares an object by its <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is Observed<T> other && other.id == id;
    }
    
    /// <summary>
    /// Helper class holding the global identifier counter shared by all <see cref="Observed{T}">observed</see> instances
    /// and the default listener list capacity constant.
    /// </summary>
    internal static class Observed {
        /// <summary> Global counter used to assign unique identifiers. </summary>
        private static int _globalId;
        
        /// <summary> Default capacity for the internal listener lists. </summary>
        public const int CAPACITY = 4;
        
        static Observed() => _globalId = 0;
        
        public static int GetID() => _globalId++;
    }
}