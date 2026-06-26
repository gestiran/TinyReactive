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
        
    #if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Send(ref T value) {
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(ref value);
            }
        }
        
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
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
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputChanger<T> AddListener(ValueChanger<T> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputChanger<T> AddListener<TUnload>(ValueChanger<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
    #endregion
        
    #region Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputChanger<T> RemoveListener(ValueChanger<T> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
    #endregion Remove
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Unload() => _listenersValue.Clear();
        
        public override int GetHashCode() => _id;
    }
}