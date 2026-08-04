// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
    /// <summary> Modifies the value passed by reference. </summary>
    /// <typeparam name="T"> Any unmanaged type. </typeparam>
    public sealed class InputChanger<T> : IEquatable<InputChanger<T>>, IUnload where T : unmanaged {
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<ValueChanger<T>> listeners;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="capacity"> Initial capacity of the <see cref="System.Collections.Generic.List{T}">List</see>. </param>
        public InputChanger(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ValueChanger<T>>(capacity);
        }
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() { }
        
        /// <summary> Invoke all subscribed listeners. </summary>
        /// <param name="value"> The value that needs to be modified. </param>
        public void Send(ref T value) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                listeners[i].Invoke(ref value);
            }
        }
        
        /// <summary> Invoke all subscribed listeners. </summary>
        /// <param name="values"> Values that need to be modified. </param>
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
        
        /// <summary> Adds a listener that will be invoked when a Send request. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> AddListener(ValueChanger<T> listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when a Send request. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> AddListener<TUnload>(ValueChanger<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> RemoveListener(ValueChanger<T> listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        public override string ToString() => $"InputChanger<{typeof(T).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputChanger<T> other) => other != null && other.id == id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is InputChanger<T> other && other.id == id;
    }
}