// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using Unity.Profiling;
using UnityEngine;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class InputChanger<T> : IUnload where T : unmanaged {
        private readonly int _id;
        private readonly LazyList<ValueChanger<T>> _listenersValue;
        
        public InputChanger(int capacity = Observed.CAPACITY) {
            _id = Observed.globalId++;
            _listenersValue = new LazyList<ValueChanger<T>>(capacity);
        }
        
        public InputChanger(ValueChanger<T> action) : this() => AddListener(action);
        
        public InputChanger(ValueChanger<T> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        [Obsolete("Can't use without parameters!", true)]
        public void Send() {
            // Do nothing
        }
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
        [HideInCallstack, IgnoredByDeepProfiler]
        public void Send(ref T value) {
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(ref value);
            }
        }
        
        [HideInCallstack, IgnoredByDeepProfiler]
        public void Send(params T[] values) {
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int valueId = 0; valueId < values.Length; valueId++) {
                for (int i = 0; i < _listenersValue.count; i++) {
                    _listenersValue[i].Invoke(ref values[valueId]);
                }
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
        public InputChanger<T> AddListener(ValueChanger<T> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
        public InputChanger<T> AddListener<TUnload>(ValueChanger<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
        public InputChanger<T> RemoveListener(ValueChanger<T> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
    #endregion Remove
        
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
        public void Unload() => _listenersValue.Clear();
        
        public override int GetHashCode() => _id;
    }
}