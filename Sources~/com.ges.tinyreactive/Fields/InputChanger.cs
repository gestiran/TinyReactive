// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive.Fields {
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
        
        public void Send(ref T value) {
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(ref value);
            }
        }
        
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
        
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> AddListener(ValueChanger<T> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> AddListener<TUnload>(ValueChanger<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public InputChanger<T> RemoveListener(ValueChanger<T> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public void Unload() => _listenersValue.Clear();
        
        public override int GetHashCode() => _id;
    }
}