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
    public sealed class InputSingleListener : IUnload {
        private readonly LazyList<Func<bool>> _listeners;
        
        public InputSingleListener() => _listeners = new LazyList<Func<bool>>(Observed.CAPACITY);
        
        public InputSingleListener(Func<bool> action) : this() => AddListener(action);
        
        public InputSingleListener(Func<bool> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
    #if UNITY_ENGINE && ODIN_INSPECTOR && UNITY_EDITOR
        [Button]
    #endif
    #if UNITY_ENGINE
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Send(bool expectedResult = true) {
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                if (_listeners[i].Invoke() == expectedResult) {
                    return;
                }
            }
        }
        
    #region Add
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputSingleListener AddListener(Func<bool> listener) {
            _listeners.Add(listener);
            return this;
        }
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public InputSingleListener AddListener<TUnload>(Func<bool> listener, TUnload unload) where TUnload : IUnloadLink {
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
        public InputSingleListener RemoveListener(Func<bool> listener) {
            _listeners.Remove(listener);
            return this;
        }
        
    #endregion
        
    #if UNITY_ENGINE
        // Resharper disable Unity.ExpensiveCode
        [HideInCallstack, IgnoredByDeepProfiler]
    #endif
        public void Unload() => _listeners.Clear();
    }
}