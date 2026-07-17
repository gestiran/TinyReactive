// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
    public sealed class InputChanger<T> : IEquatable<InputChanger<T>>, IUnload where T : unmanaged {
        internal readonly int id;
        internal readonly LazyList<ValueChanger<T>> listeners;
        
        public InputChanger(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ValueChanger<T>>(capacity);
        }
        
        public InputChanger(ValueChanger<T> action) : this() => AddListener(action);
        
        public InputChanger(ValueChanger<T> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
        public void Send(ref T value) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                listeners[i].Invoke(ref value);
            }
        }
        
        public void Send(params T[] values) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                for (int i = 0; i < listeners.Count; i++) {
                    listeners[i].Invoke(ref values[valueId]);
                }
            }
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> AddListener(ValueChanger<T> listener) {
            listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> AddListener<TUnload>(ValueChanger<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> RemoveListener(ValueChanger<T> listener) {
            listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by its <see cref="id"/>. </summary>
        public bool Equals(InputChanger<T> other) =>  other != null && other.id == id;
        
        /// <summary> Compares an object by its <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is InputChanger<T> other && other.id == id;
    }
}