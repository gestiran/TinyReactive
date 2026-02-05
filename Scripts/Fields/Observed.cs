// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public class Observed<T> : IEquatable<Observed<T>>, IEquatable<T>, IUnload {
        public T value => _value;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, HorizontalGroup, HideLabel, OnValueChanged("@Set(_value)"), HideDuplicateReferenceBox, HideReferenceObjectPicker]
    #endif
        protected T _value;
        
        private readonly int _id;
        private readonly LazyList<ActionListener> _listeners;
        private readonly LazyList<ActionListener<T>> _listenersValue;
        private readonly LazyList<ActionListener<T, T>> _listenersChange;
        
        public Observed(T data, int capacity = Observed.CAPACITY) {
            _value = data;
            _id = Observed.globalId++;
            _listeners = new LazyList<ActionListener>(capacity);
            _listenersValue = new LazyList<ActionListener<T>>(capacity);
            _listenersChange = new LazyList<ActionListener<T, T>>(capacity);
        }
        
        public Observed() {
            _value = default;
            _id = Observed.globalId++;
            _listeners = new LazyList<ActionListener>(Observed.CAPACITY);
            _listenersValue = new LazyList<ActionListener<T>>(Observed.CAPACITY);
            _listenersChange = new LazyList<ActionListener<T, T>>(Observed.CAPACITY);
        }
        
        public void SetSilent(T newValue) => _value = newValue;
        
        public virtual void Set(T newValue) {
            T current = _value;
            _value = newValue;
            
            if (_listeners.isDirty) {
                _listeners.Apply();
            }
            
            if (_listenersValue.isDirty) {
                _listenersValue.Apply();
            }
            
            if (_listenersChange.isDirty) {
                _listenersChange.Apply();
            }
            
            for (int i = 0; i < _listeners.count; i++) {
                _listeners[i].Invoke();
            }
            
            for (int i = 0; i < _listenersValue.count; i++) {
                _listenersValue[i].Invoke(newValue);
            }
            
            for (int i = 0; i < _listenersChange.count; i++) {
                _listenersChange[i].Invoke(current, newValue);
            }
        }
        
    #region Add
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener listener) {
            _listeners.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener<T> listener) {
            _listenersValue.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener(ActionListener<T, T> listener) {
            _listenersChange.Add(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListener<TUnload>(ActionListener<T, T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListener(listener);
            unload.Add(new UnloadAction(() => _listenersChange.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerValue<TV>(ActionListener listener, IUnloadLink unload) where TV : T {
            AddListener(v =>
            {
                if (v is TV) {
                    listener.Invoke();
                }
            }, unload);
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerValue<TV>(ActionListener<TV> listener, IUnloadLink unload) where TV : T {
            AddListener(v =>
            {
                if (v is TV target) {
                    listener.Invoke(target);
                }
            }, unload);
            
            return this;
        }
        
    #endregion
        
    #region ByPriority
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst(ActionListener listener) {
            if (_listeners.cacheCount > 0) {
                _listeners.Insert(0, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst(ActionListener<T> listener) {
            if (_listenersValue.cacheCount > 0) {
                _listenersValue.Insert(0, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerFirst<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerFirst(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast(ActionListener listener) {
            if (_listeners.cacheCount > 0) {
                _listeners.Insert(_listeners.cacheCount - 1, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => _listeners.Remove(listener)));
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast(ActionListener<T> listener) {
            if (_listenersValue.cacheCount > 0) {
                _listenersValue.Insert(_listenersValue.cacheCount - 1, listener);
            } else {
                AddListener(listener);
            }
            
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> AddListenerLast<TUnload>(ActionListener<T> listener, TUnload unload) where TUnload : IUnloadLink {
            AddListenerLast(listener);
            unload.Add(new UnloadAction(() => _listenersValue.Remove(listener)));
            return this;
        }
        
    #endregion
        
    #region Remove
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener listener) {
            _listeners.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener<T> listener) {
            _listenersValue.Remove(listener);
            return this;
        }
        
        // Resharper disable Unity.ExpensiveCode
        public Observed<T> RemoveListener(ActionListener<T, T> listener) {
            _listenersChange.Remove(listener);
            return this;
        }
        
    #endregion
        
        public virtual void Unload() {
            _listeners.Clear();
            _listenersValue.Clear();
            _listenersChange.Clear();
        }
        
        public static implicit operator T(Observed<T> observed) => observed.value;
        
        public override string ToString() => $"{value}";
        
        public override int GetHashCode() => _id;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        private static readonly bool _isInt = typeof(T) == typeof(int);
        private static readonly bool _isFloat = typeof(T) == typeof(float);
        
        [Button("x2"), HorizontalGroup, ShowIf("_isInt")]
        private void AddInt() {
            if (_value is int intValue) {
                if (intValue == 0) {
                    intValue = 10;
                } else {
                    intValue *= 2;
                }
                
                if (intValue is T result) {
                    Set(result);
                }
            }
        }
        
        [Button("x2"), HorizontalGroup, ShowIf("_isFloat")]
        private void AddFloat() {
            if (_value is float floatValue) {
                if (floatValue == 0f) {
                    floatValue = 10f;
                } else {
                    floatValue *= 2f;
                }
                
                if (floatValue is T result) {
                    Set(result);
                }
            }
        }
        
        [Button("x0.5"), HorizontalGroup, ShowIf("_isInt")]
        private void SubtractInt() {
            if (_value is int intValue) {
                if (intValue > 0 && intValue <= 10) {
                    intValue = 0;
                } else {
                    intValue /= 2;
                }
                
                if (intValue is T result) {
                    Set(result);
                }
            }
        }
        
        [Button("x0.5"), HorizontalGroup, ShowIf("_isFloat")]
        private void SubtractFloat() {
            if (_value is float floatValue) {
                if (floatValue > 0f && floatValue <= 10f) {
                    floatValue = 0f;
                } else {
                    floatValue *= 0.5f;
                }
                
                if (floatValue is T result) {
                    Set(result);
                }
            }
        }
        
    #endif
        public bool Equals(Observed<T> other) => other != null && other._id == _id;
        
        public bool Equals(T other) => other != null && other.Equals(value);
        
        public override bool Equals(object obj) => obj is Observed<T> other && other._id == _id;
    }
    
    internal static class Observed {
        internal static int globalId;
        
        internal const int CAPACITY = 4;
        
        static Observed() => globalId = 0;
    }
}