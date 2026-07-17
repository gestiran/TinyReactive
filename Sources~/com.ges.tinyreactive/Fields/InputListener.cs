// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
    public sealed class InputListener : IUnload, IEquatable<InputListener> {
        internal readonly int id;
        internal readonly LazyList<ActionListener> listeners;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public void Send() => listeners.Invoke();
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        public override int GetHashCode() => id;
        
        public bool Equals(InputListener other) => other != null && other.id == id;
    }
    
    public sealed class InputListener<T> : IUnload, IEquatable<InputListener<T>> {
        internal readonly int id;
        internal readonly LazyList<ActionListener> listeners;
        internal readonly LazyList<ActionListener<T>> listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
        public void Send(T value) {
            listeners.Invoke();
            listenersValue.Invoke(value);
        }
        
        public void Send(params T[] values) {
            for (int valueId = 0; valueId < values.Length; valueId++) {
                listeners.Invoke();
                listenersValue.Invoke(values[valueId]);
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener(ActionListener<T> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddListener(value =>
                        {
                            if (value is TV) {
                                listener.Invoke();
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddListener(value =>
                        {
                            if (value is TV target) {
                                listener.Invoke(target);
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> RemoveListener(ActionListener<T> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            listeners.Clear();
            listenersValue.Clear();
        }
        
        public override int GetHashCode() => id;
        
        public bool Equals(InputListener<T> other) => other != null && other.id == id;
    }
    
    public sealed class InputListener<T1, T2> : IUnload, IEquatable<InputListener<T1, T2>> {
        internal readonly int id;
        internal readonly LazyList<ActionListener> listeners;
        internal readonly LazyList<ActionListener<T1, T2>> listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T1, T2>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public void Send(T1 value1, T2 value2) {
            listeners.Invoke();
            listenersValue.Invoke(value1, value2);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener(ActionListener<T1, T2> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener<TUnload>(ActionListener<T1, T2> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListenerValue<TV1, TV2>(ActionListener listener, IUnloadLink unload) where TV1 : T1 where TV2 : T2 {
            AddListener((value1, value2) =>
                        {
                            if (value1 is TV1 && value2 is TV2) {
                                listener.Invoke();
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListenerValue<TV1, TV2>(ActionListener<TV1, TV2> listener, IUnloadLink unload) where TV1 : T1 where TV2 : T2 {
            AddListener((value1, value2) =>
                        {
                            if (value1 is TV1 target1 && value2 is TV2 target2) {
                                listener.Invoke(target1, target2);
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> RemoveListener(ActionListener<T1, T2> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            listeners.Clear();
            listenersValue.Clear();
        }
        
        public override int GetHashCode() => id;
        
        public bool Equals(InputListener<T1, T2> other) => other != null && other.id == id;
    }
    
    public sealed class InputListener<T1, T2, T3> : IUnload, IEquatable<InputListener<T1, T2, T3>> {
        internal readonly int id;
        internal readonly LazyList<ActionListener> listeners;
        internal readonly LazyList<ActionListener<T1, T2, T3>> listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T1, T2, T3>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2, T3> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2, T3> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public void Send(T1 value1, T2 value2, T3 value3) {
            listeners.Invoke();
            listenersValue.Invoke(value1, value2, value3);
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener(ActionListener<T1, T2, T3> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener<TUnload>(ActionListener<T1, T2, T3> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListenerValue<TV1, TV2, TV3>(ActionListener listener, IUnloadLink unload)
            where TV1 : T1 where TV2 : T2 where TV3 : T3 {
            AddListener((value1, value2, value3) =>
                        {
                            if (value1 is TV1 && value2 is TV2 && value3 is TV3) {
                                listener.Invoke();
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListenerValue<TV1, TV2, TV3>(ActionListener<TV1, TV2, TV3> listener, IUnloadLink unload)
            where TV1 : T1 where TV2 : T2 where TV3 : T3 {
            AddListener((value1, value2, value3) =>
                        {
                            if (value1 is TV1 target1 && value2 is TV2 target2 && value3 is TV3 target3) {
                                listener.Invoke(target1, target2, target3);
                            }
                        },
                        unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> RemoveListener(ActionListener<T1, T2, T3> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            listeners.Clear();
            listenersValue.Clear();
        }
        
        public override int GetHashCode() => id;
        
        public bool Equals(InputListener<T1, T2, T3> other) => other != null && other.id == id;
    }
}