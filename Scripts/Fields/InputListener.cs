// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Runtime.CompilerServices;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener : IUnload {
        private readonly int _id;
        private readonly LazyList<ActionListener> _listeners;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new LazyList<ActionListener>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send() {
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                _listeners[i].Invoke();
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
        
        public override int GetHashCode() => _id;
    }
    
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T> : IUnload {
        private readonly int _id;
        private readonly LazyList<ActionListener> _listeners;
        private readonly LazyList<ActionListener<T>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new LazyList<ActionListener>(capacity);
            _listenersValue = new LazyList<ActionListener<T>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T> action, UnloadPool unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T value) {
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                _listeners[i].Invoke();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(value);
            }
        }
        
        public void Send(params T[] values) {
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                _listeners[i].Invoke();
            }
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                T value = values[valueId];
                
                for (int i = 0; i < _listenersValue.count; i++) {
                    _listenersValue[i].Invoke(value);
                }
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T> listener) => _listenersValue.Remove(listener);
        
    #endregion Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2> : IUnload {
        private readonly int _id;
        private readonly LazyList<ActionListener> _listeners;
        private readonly LazyList<ActionListener<T1, T2>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new LazyList<ActionListener>(capacity);
            _listenersValue = new LazyList<ActionListener<T1, T2>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 value1, T2 value2) {
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                _listeners[i].Invoke();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(value1, value2);
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T1, T2> listener) => _listenersValue.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputListener<T1, T2, T3> : IUnload {
        private readonly int _id;
        private readonly LazyList<ActionListener> _listeners;
        private readonly LazyList<ActionListener<T1, T2, T3>> _listenersValue;
        
        public InputListener(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listeners = new LazyList<ActionListener>(capacity);
            _listenersValue = new LazyList<ActionListener<T1, T2, T3>>(capacity);
        }
        
        public InputListener(ActionListener action) : this() => AddListener(action);
        
        public InputListener(ActionListener<T1, T2, T3> action) : this() => AddListener(action);
        
        public InputListener(ActionListener action, UnloadPool unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2, T3> action, UnloadPool unload) : this() => AddListener(action, unload);
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        public void Send(T1 value1, T2 value2, T3 value3) {
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                _listeners[i].Invoke();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(value1, value2, value3);
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener) => _listeners.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2, T3> listener) => _listenersValue.Add(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void AddListener(ActionListener<T1, T2, T3> listener, UnloadPool unload) {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener listener) => _listeners.Remove(listener);
        
        // Resharper disable Unity.ExpensiveCode
        public void RemoveListener(ActionListener<T1, T2, T3> listener) => _listenersValue.Remove(listener);
        
    #endregion
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
}