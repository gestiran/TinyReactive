// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;

namespace TinyReactive.Fields {
    public sealed class InputComposition<T> : IEquatable<InputComposition<T>>, IUnload {
        internal readonly int id;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<CompositionListener<T>> listeners;
        
        /// <summary> Creates a new instance and initializes the listener list. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener list. </param>
        public InputComposition(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<CompositionListener<T>>(capacity);
        }
        
        public IEnumerable<T> Send() {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                foreach (T current in listeners[i].Invoke()) {
                    yield return current;
                }
            }
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T> AddListener(CompositionListener<T> listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T> AddListener<TUnload>(CompositionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T> RemoveListener(CompositionListener<T> listener) {
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
        public bool Equals(InputComposition<T> other) => other != null && other.id == id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is InputComposition<T> other && other.id == id;
    }
    
    public sealed class InputComposition<T, TResult> : IEquatable<InputComposition<T, TResult>>, IUnload {
        internal readonly int id;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<CompositionListener<T, TResult>> listeners;
        
        /// <summary> Creates a new instance and initializes the listener list. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener list. </param>
        public InputComposition(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<CompositionListener<T, TResult>>(capacity);
        }
        
        public IEnumerable<TResult> Send(T value) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                foreach (TResult current in listeners[i].Invoke(value)) {
                    yield return current;
                }
            }
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T, TResult> AddListener(CompositionListener<T, TResult> listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T, TResult> AddListener<TUnload>(CompositionListener<T, TResult> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T, TResult> RemoveListener(CompositionListener<T, TResult> listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        public override string ToString() => $"InputChanger<{typeof(T).Name}, {typeof(TResult).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputComposition<T, TResult> other) => other != null && other.id == id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is InputComposition<T, TResult> other && other.id == id;
    }
    
    public sealed class InputComposition<T1, T2, TResult> : IEquatable<InputComposition<T1, T2, TResult>>, IUnload {
        internal readonly int id;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<CompositionListener<T1, T2, TResult>> listeners;
        
        /// <summary> Creates a new instance and initializes the listener list. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener list. </param>
        public InputComposition(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<CompositionListener<T1, T2, TResult>>(capacity);
        }
        
        public IEnumerable<TResult> Send(T1 value1, T2 value2) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                foreach (TResult current in listeners[i].Invoke(value1, value2)) {
                    yield return current;
                }
            }
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T1, T2, TResult> AddListener(CompositionListener<T1, T2, TResult> listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T1, T2, TResult> AddListener<TUnload>(CompositionListener<T1, T2, TResult> listener, TUnload unload)
            where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T1, T2, TResult> RemoveListener(CompositionListener<T1, T2, TResult> listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        public override string ToString() => $"InputChanger<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(TResult).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputComposition<T1, T2, TResult> other) => other != null && other.id == id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is InputComposition<T1, T2, TResult> other && other.id == id;
    }
    
    public sealed class InputComposition<T1, T2, T3, TResult> : IEquatable<InputComposition<T1, T2, T3, TResult>>, IUnload {
        internal readonly int id;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<CompositionListener<T1, T2, T3, TResult>> listeners;
        
        /// <summary> Creates a new instance and initializes the listener list. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener list. </param>
        public InputComposition(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<CompositionListener<T1, T2, T3, TResult>>(capacity);
        }
        
        public IEnumerable<TResult> Send(T1 value1, T2 value2, T3 value3) {
            if (listeners.isDirty) {
                listeners.Apply();
            }
            
            for (int i = 0; i < listeners.Count; i++) {
                foreach (TResult current in listeners[i].Invoke(value1, value2, value3)) {
                    yield return current;
                }
            }
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T1, T2, T3, TResult> AddListener(CompositionListener<T1, T2, T3, TResult> listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T1, T2, T3, TResult> AddListener<TUnload>(CompositionListener<T1, T2, T3, TResult> listener, TUnload unload)
            where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputComposition<T1, T2, T3, TResult> RemoveListener(CompositionListener<T1, T2, T3, TResult> listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        public override string ToString() => $"InputChanger<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}, {typeof(TResult).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputComposition<T1, T2, T3, TResult> other) => other != null && other.id == id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public override bool Equals(object obj) => obj is InputComposition<T1, T2, T3, TResult> other && other.id == id;
    }
}