// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

#if EXTERNAL_DEPENDENCIES
using System.Text.Json.Serialization;
using TinyReactive.JsonConverters;
#endif

namespace TinyReactive.Fields {
#if EXTERNAL_DEPENDENCIES
    [JsonConverter(typeof(ObservedJsonConverter))]
#endif
    public class Observed<T> : IValue<T>, IEquatable<Observed<T>>, IEquatable<T>, IUnload {
        public T value { get; protected internal set; }
        
        internal readonly int id;
        internal readonly LazyList<ActionListener> listeners;
        internal readonly LazyList<ActionListener<T>> listenersValue;
        internal readonly LazyList<ActionListener<T, T>> listenersChange;
        
        public Observed(T data, int capacity = Observed.CAPACITY) {
            value = data;
            id = Observed.globalId++;
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T>>(capacity);
            listenersChange = new LazyList<ActionListener<T, T>>(capacity);
        }
        
        public Observed() {
            value = default;
            id = Observed.globalId++;
            listeners = new LazyList<ActionListener>(Observed.CAPACITY);
            listenersValue = new LazyList<ActionListener<T>>(Observed.CAPACITY);
            listenersChange = new LazyList<ActionListener<T, T>>(Observed.CAPACITY);
        }
        
        public void SetSilent(T newValue) => value = newValue;
        
        public virtual void Set(T newValue) {
            T current = value;
            value = newValue;
            listeners.Invoke();
            listenersValue.Invoke(newValue);
            listenersChange.Invoke(current, newValue);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener<T> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener<T, T> listener) {
            listenersChange.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener<T, T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersChange.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddListener(v =>
                        {
                            if (v is TV) {
                                listener.Invoke();
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddListener(v =>
                        {
                            if (v is TV target) {
                                listener.Invoke(target);
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst(ActionListener listener) {
            if (listeners.cacheCount > 0) {
                listeners.Insert(0, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst(ActionListener<T> listener) {
            if (listenersValue.cacheCount > 0) {
                listenersValue.Insert(0, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast(ActionListener listener) {
            if (listeners.cacheCount > 0) {
                listeners.Insert(listeners.cacheCount - 1, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast(ActionListener<T> listener) {
            if (listenersValue.cacheCount > 0) {
                listenersValue.Insert(listenersValue.cacheCount - 1, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener<T> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener<T, T> listener) {
            listenersChange.Remove(listener);
            return this;
        }
        
        public virtual void Unload() {
            listeners.Clear();
            listenersValue.Clear();
            listenersChange.Clear();
        }
        
        public static implicit operator T(Observed<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => id;
        
        public bool Equals(Observed<T> other) => other != null && other.id == id;
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is Observed<T> other && other.id == id;
    }
    
    internal static class Observed {
        internal static int globalId;
        
        internal const int CAPACITY = 4;
        
        static Observed() => globalId = 0;
    }
}