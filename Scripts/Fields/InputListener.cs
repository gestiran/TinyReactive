// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if UNITY_2017_1_OR_NEWER
#define UNITY_ENGINE
#endif

using System;

#if UNITY_ENGINE
using Unity.Profiling;
using UnityEngine;
#endif

#if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
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
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
    #if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener AddListener(ActionListener listener) {
            _listeners.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
    #endregion
        
    #region Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener RemoveListener(ActionListener listener) {
            _listeners.Remove(listener);
            return this;
        }
        
    #endregion
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Unload() => _listeners.Clear();
        
        public override int GetHashCode() => _id;
    }
    
#if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
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
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
    #if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
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
        
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T> AddListener(ActionListener listener) {
            _listeners.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T> AddListener(ActionListener<T> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T> AddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #endregion
        
    #region Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T> RemoveListener(ActionListener listener) {
            _listeners.Remove(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T> RemoveListener(ActionListener<T> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
    #endregion Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
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
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
    #if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2> AddListener(ActionListener listener) {
            _listeners.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2> AddListener(ActionListener<T1, T2> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2> AddListener<TUnload>(ActionListener<T1, T2> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #endregion
        
    #region Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2> RemoveListener(ActionListener listener) {
            _listeners.Remove(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2> RemoveListener(ActionListener<T1, T2> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
    #endregion
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
    
#if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
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
        
        public InputListener(ActionListener action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        public InputListener(ActionListener<T1, T2, T3> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
    #if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2, T3> AddListener(ActionListener listener) {
            _listeners.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2, T3> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2, T3> AddListener(ActionListener<T1, T2, T3> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2, T3> AddListener<TUnload>(ActionListener<T1, T2, T3> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #endregion
        
    #region Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2, T3> RemoveListener(ActionListener listener) {
            _listeners.Remove(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputListener<T1, T2, T3> RemoveListener(ActionListener<T1, T2, T3> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
    #endregion
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
        }
        
        public override int GetHashCode() => _id;
    }
}