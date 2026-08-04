// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
    /// <summary> A parameterless listener that allows subscribing and triggering actions. </summary>
    public sealed class InputListener : IEquatable<InputListener>, IUnload {
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on any value change. </summary>
        internal readonly LazyList<ActionListener> listeners;
        
        /// <summary> Creates a new instance and initializes the listener list. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener list. </param>
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
        }
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Invokes all subscribed listeners. </summary>
        public void Send() => listeners.Invoke();
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => listeners.Clear();
        
        /// <summary> Returns the string representation of the listener. </summary>
        public override string ToString() => "InputListener";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputListener other) => other != null && other.id == id;
    }
    
    /// <summary> A listener that allows subscribing and triggering actions with a single value. </summary>
    /// <typeparam name="T"> The type of the value. </typeparam>
    public sealed class InputListener<T> : IEquatable<InputListener<T>>, IUnload {
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on any value change. </summary>
        internal readonly LazyList<ActionListener> listeners;
        
        /// <summary> List of listeners that receive the new value on change. </summary>
        internal readonly LazyList<ActionListener<T>> listenersValue;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T>>(capacity);
        }
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener<T> action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener<T> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Invokes all subscribed listeners with the specified value. </summary>
        /// <param name="value"> The value to pass to the listeners. </param>
        public void Send(T value) {
            listeners.Invoke();
            listenersValue.Invoke(value);
        }
        
        /// <summary> Invokes all subscribed listeners for each specified value. </summary>
        /// <param name="values"> The values to pass to the listeners. </param>
        public void Send(params T[] values) {
            for (int valueId = 0; valueId < values.Length; valueId++) {
                listeners.Invoke();
                listenersValue.Invoke(values[valueId]);
            }
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered, receiving the value. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener(ActionListener<T> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered, receiving the value. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> AddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the triggered value is of the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of T that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Adds a listener that will be called when the triggered value is of the specified type. </summary>
        /// <typeparam name="TV"> The expected subtype of T that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T> RemoveListener(ActionListener<T> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            listeners.Clear();
            listenersValue.Clear();
        }
        
        /// <summary> Returns the string representation of the listener. </summary>
        public override string ToString() => $"InputListener<{typeof(T).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputListener<T> other) => other != null && other.id == id;
        
        /// <summary> Obsolete method. Cannot send without parameters. </summary>
        [Obsolete("Can't use without parameters!", true)]
        public void Send() { }
    }
    
    /// <summary> A listener that allows subscribing and triggering actions with two values. </summary>
    /// <typeparam name="T1"> The type of the first value. </typeparam>
    /// <typeparam name="T2"> The type of the second value. </typeparam>
    public sealed class InputListener<T1, T2> : IEquatable<InputListener<T1, T2>>, IUnload {
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on any value change. </summary>
        internal readonly LazyList<ActionListener> listeners;
        
        /// <summary> List of listeners that receive the new values on change. </summary>
        internal readonly LazyList<ActionListener<T1, T2>> listenersValue;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T1, T2>>(capacity);
        }
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener<T1, T2> action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener<T1, T2> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Invokes all subscribed listeners with the specified values. </summary>
        /// <param name="value1"> The first value to pass to the listeners. </param>
        /// <param name="value2"> The second value to pass to the listeners. </param>
        public void Send(T1 value1, T2 value2) {
            listeners.Invoke();
            listenersValue.Invoke(value1, value2);
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered, receiving the values. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener(ActionListener<T1, T2> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered, receiving the values. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> AddListener<TUnload>(ActionListener<T1, T2> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the triggered values are of the specified types. </summary>
        /// <typeparam name="TV1"> The expected subtype of T1 that triggers the listener. </typeparam>
        /// <typeparam name="TV2"> The expected subtype of T2 that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Adds a listener that will be called when the triggered values are of the specified types. </summary>
        /// <typeparam name="TV1"> The expected subtype of T1 that triggers the listener. </typeparam>
        /// <typeparam name="TV2"> The expected subtype of T2 that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2> RemoveListener(ActionListener<T1, T2> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            listeners.Clear();
            listenersValue.Clear();
        }
        
        /// <summary> Returns the string representation of the listener. </summary>
        public override string ToString() => $"InputListener<{typeof(T1).Name}, {typeof(T2).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputListener<T1, T2> other) => other != null && other.id == id;
    }
    
    /// <summary> A listener that allows subscribing and triggering actions with three values. </summary>
    /// <typeparam name="T1"> The type of the first value. </typeparam>
    /// <typeparam name="T2"> The type of the second value. </typeparam>
    /// <typeparam name="T3"> The type of the third value. </typeparam>
    public sealed class InputListener<T1, T2, T3> : IEquatable<InputListener<T1, T2, T3>>, IUnload {
        /// <summary> Unique identifier automatically assigned to this instance. </summary>
        internal readonly int id;
        
        /// <summary> List of parameterless listeners invoked on any value change. </summary>
        internal readonly LazyList<ActionListener> listeners;
        
        /// <summary> List of listeners that receive the new values on change. </summary>
        internal readonly LazyList<ActionListener<T1, T2, T3>> listenersValue;
        
        /// <summary> Creates a new instance and initializes the listener lists. </summary>
        /// <param name="capacity"> Initial capacity of the internal listener lists. </param>
        public InputListener(int capacity = Observed.CAPACITY) {
            id = Observed.GetID();
            listeners = new LazyList<ActionListener>(capacity);
            listenersValue = new LazyList<ActionListener<T1, T2, T3>>(capacity);
        }
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance and adds the specified listener. </summary>
        /// <param name="action"> The listener to add. </param>
        public InputListener(ActionListener<T1, T2, T3> action) : this() => AddListener(action);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Creates a new instance, adds the specified listener, and links it to an unload pool. </summary>
        /// <param name="action"> The listener to add. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        public InputListener(ActionListener<T1, T2, T3> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        /// <summary> Invokes all subscribed listeners with the specified values. </summary>
        /// <param name="value1"> The first value to pass to the listeners. </param>
        /// <param name="value2"> The second value to pass to the listeners. </param>
        /// <param name="value3"> The third value to pass to the listeners. </param>
        public void Send(T1 value1, T2 value2, T3 value3) {
            listeners.Invoke();
            listenersValue.Invoke(value1, value2, value3);
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener(ActionListener listener) {
            listeners.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listeners.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered, receiving the values. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener(ActionListener<T1, T2, T3> listener) {
            listenersValue.Add(listener);
            return this;
        }
        
        /// <summary> Adds a listener that will be invoked when triggered, receiving the values. </summary>
        /// <typeparam name="TUnload"> <see cref="TinyReactive.IUnloadLink">Unload</see> pool type. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> AddListener<TUnload>(ActionListener<T1, T2, T3> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => listenersValue.Remove(listener)));
            return this;
        }
        
        /// <summary> Adds a listener that will be called when the triggered values are of the specified types. </summary>
        /// <typeparam name="TV1"> The expected subtype of T1 that triggers the listener. </typeparam>
        /// <typeparam name="TV2"> The expected subtype of T2 that triggers the listener. </typeparam>
        /// <typeparam name="TV3"> The expected subtype of T3 that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Adds a listener that will be called when the triggered values are of the specified types. </summary>
        /// <typeparam name="TV1"> The expected subtype of T1 that triggers the listener. </typeparam>
        /// <typeparam name="TV2"> The expected subtype of T2 that triggers the listener. </typeparam>
        /// <typeparam name="TV3"> The expected subtype of T3 that triggers the listener. </typeparam>
        /// <param name="listener"> Listener that will be invoked. </param>
        /// <param name="unload"> Unload pool for automatic unsubscription. </param>
        /// <returns> Current instance. </returns>
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
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> RemoveListener(ActionListener listener) {
            listeners.Remove(listener);
            return this;
        }
        
        /// <summary> Removes a previously added listener. </summary>
        /// <returns> Current instance. </returns>
        // Resharper disable Unity.ExpensiveCode
        public InputListener<T1, T2, T3> RemoveListener(ActionListener<T1, T2, T3> listener) {
            listenersValue.Remove(listener);
            return this;
        }
        
        /// <summary> Clears all listener lists. </summary>
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            listeners.Clear();
            listenersValue.Clear();
        }
        
        /// <summary> Returns the string representation of the listener. </summary>
        public override string ToString() => $"InputListener<{typeof(T1).Name}, {typeof(T2).Name}, {typeof(T3).Name}>";
        
        /// <summary> Returns the current unique <see cref="id"/>. </summary>
        public override int GetHashCode() => id;
        
        /// <summary> Compares an object by <see cref="id"/>. </summary>
        public bool Equals(InputListener<T1, T2, T3> other) => other != null && other.id == id;
    }
}