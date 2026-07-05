// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
    public sealed class InputSingleListener : IUnload {
        private readonly LazyList<Func<bool>> _listeners;
        
        public InputSingleListener() => _listeners = new LazyList<Func<bool>>(Observed.CAPACITY);
        
        public InputSingleListener(Func<bool> action) : this() => AddListener(action);
        
        public InputSingleListener(Func<bool> action, IUnloadLink unload) : this() => AddListener(action, unload);
        
        // Resharper disable Unity.ExpensiveCode
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
        
        // Resharper disable Unity.ExpensiveCode
        public InputSingleListener AddListener(Func<bool> listener) {
            _listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputSingleListener AddListener<TUnload>(Func<bool> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputSingleListener RemoveListener(Func<bool> listener) {
            _listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listeners.Clear();
    }
}