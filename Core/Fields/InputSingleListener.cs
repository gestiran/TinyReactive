// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
    /// <summary> Invokes listeners sequentially until one returns the expected result. </summary>
    public sealed class InputSingleListener : IEquatable<InputSingleListener>, IUnload {
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of listeners that return a boolean value. </summary>
        internal readonly LazyList<Func<bool>> listeners;
        
        /// <summary> Creates a new instance and initializes the listener list. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener list. </param>
        public InputSingleListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<Func<bool>>(capacity);
        }
        
        /// <summary> Invokes all subscribed listeners until one returns the expected result. </summary>
        /// <param name="expectedResult"> The boolean result that stops further listener invocation. </param>
        // Resharper disable Unity.ExpensiveCode
        public void Send(bool expectedResult = true) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                if (listeners[i].Invoke() == expectedResult) {
                    return;
                }
            }
        }
        
        /// <summary> Adds a listener that will be invoked. </summary>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputSingleListener AddListener(Func<bool> listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputSingleListener AddListener<TUnload>(Func<bool> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputSingleListener RemoveListener(Func<bool> listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputSingleListener other) => other != null && other.id == id;
    }
}