// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Collections.Generic;

#if ODIN_INSPECTOR && UNITY_EDITOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR && UNITY_EDITOR
    [ShowInInspector, HideReferenceObjectPicker, HideDuplicateReferenceBox]
#endif
    public sealed class ObservedDictionary<TKey, TValue> {
        public int count => root.Count;
        
        private readonly LazyList<ActionListener> _onAdd;
        private readonly LazyList<ActionListener<TValue>> _onAddWithValue;
        private readonly LazyList<ActionListener> _onRemove;
        private readonly LazyList<ActionListener<TValue>> _onRemoveWithValue;
        
    #if ODIN_INSPECTOR && UNITY_EDITOR
        [ShowInInspector, LabelText("Elements")]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true, DraggableItems = false, ListElementLabelName = "@GetType().Name")]
        private List<TValue> _inspectorDisplay;
    #endif
        
        internal readonly Dictionary<TKey, TValue> root;
        
        public ObservedDictionary(int capacity = Observed.CAPACITY) : this(new Dictionary<TKey, TValue>(capacity)) { }
        
        public ObservedDictionary(Dictionary<TKey, TValue> value, int capacity = Observed.CAPACITY) {
            root = value;
            _onAdd = new LazyList<ActionListener>(capacity);
            _onAddWithValue = new LazyList<ActionListener<TValue>>(capacity);
            _onRemove = new LazyList<ActionListener>(capacity);
            _onRemoveWithValue = new LazyList<ActionListener<TValue>>(capacity);
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay = new List<TValue>();
            
            foreach (TValue data in value.Values) {
                _inspectorDisplay.Add(data);
            }
            
        #endif
        }
        
        public void Add(TKey key, TValue value) {
            root.Add(key, value);
            
            if (_onAdd.isDirty) {
                _onAdd.Apply();
            }
            
            if (_onAddWithValue.isDirty) {
                _onAddWithValue.Apply();
            }
            
            for (int i = 0; i < _onAdd.count; i++) {
                _onAdd[i].Invoke();
            }
            
            for (int i = 0; i < _onAddWithValue.count; i++) {
                _onAddWithValue[i].Invoke(value);
            }
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay.Add(value);
        #endif
        }
        
        public bool Remove(TKey key) {
            if (root.Remove(key, out TValue value) == false) {
                return false;
            }
            
            if (_onRemove.isDirty) {
                _onAdd.Apply();
            }
            
            if (_onRemoveWithValue.isDirty) {
                _onRemoveWithValue.Apply();
            }
            
            for (int i = 0; i < _onRemove.count; i++) {
                _onRemove[i].Invoke();
            }
            
            for (int i = 0; i < _onRemoveWithValue.count; i++) {
                _onRemoveWithValue[i].Invoke(value);
            }
            
        #if ODIN_INSPECTOR && UNITY_EDITOR
            _inspectorDisplay.Remove(value);
        #endif
            
            return true;
        }
        
        public void RemoveRange(List<TValue> values) {
            KeyValuePair<TKey, TValue>[] dataPair = new KeyValuePair<TKey, TValue>[root.Count];
            int dataId = 0;
            
            foreach (KeyValuePair<TKey, TValue> data in root) {
                dataPair[dataId++] = data;
            }
            
            for (int valueId = 0; valueId < values.Count; valueId++) {
                TValue value = values[valueId];
                
                for (dataId = 0; dataId < dataPair.Length; dataId++) {
                    if (!dataPair[dataId].Value.Equals(value)) {
                        continue;
                    }
                    
                    root.Remove(dataPair[dataId].Key);
                    
                    if (_onRemove.isDirty) {
                        _onAdd.Apply();
                    }
                    
                    if (_onRemoveWithValue.isDirty) {
                        _onRemoveWithValue.Apply();
                    }
                    
                    for (int i = 0; i < _onRemove.count; i++) {
                        _onRemove[i].Invoke();
                    }
                    
                    for (int i = 0; i < _onRemoveWithValue.count; i++) {
                        _onRemoveWithValue[i].Invoke(value);
                    }
                    
                #if ODIN_INSPECTOR && UNITY_EDITOR
                    _inspectorDisplay.Remove(value);
                #endif
                    break;
                }
            }
        }
        
        public bool TryGetValue(TKey key, out TValue value) => root.TryGetValue(key, out value);
        
        public bool ContainsKey(TKey key) => root.ContainsKey(key);
        
        public IEnumerator<TKey> ForEachKeys() {
            TKey[] keys = new TKey[root.Count];
            int keyId = 0;
            
            foreach (TKey key in root.Keys) {
                keys[keyId++] = key;
            }
            
            foreach (TKey key in keys) {
                yield return key;
            }
        }
        
        public IEnumerable<TValue> ForEachValues() {
            TValue[] values = new TValue[root.Count];
            int valueId = 0;
            
            foreach (TValue value in root.Values) {
                values[valueId++] = value;
            }
            
            foreach (TValue value in values) {
                yield return value;
            }
        }
        
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener listener) {
            _onAdd.Add(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            _onAdd.Add(listener);
            unload.Add(new UnloadAction(() => _onAdd.Remove(listener)));
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnAddListener(ActionListener<TValue> listener) {
            _onAddWithValue.Add(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnAddListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            _onAddWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onAddWithValue.Remove(listener)));
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener listener) {
            _onAdd.Remove(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> RemoveOnAddListener(ActionListener<TValue> listener) {
            _onAddWithValue.Remove(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener listener) {
            _onRemove.Add(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener listener, TUnload unload) where TUnload : IUnloadLink {
            _onRemove.Add(listener);
            unload.Add(new UnloadAction(() => _onRemove.Remove(listener)));
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener(ActionListener<TValue> listener) {
            _onRemoveWithValue.Add(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> AddOnRemoveListener<TUnload>(ActionListener<TValue> listener, TUnload unload) where TUnload : IUnloadLink {
            _onRemoveWithValue.Add(listener);
            unload.Add(new UnloadAction(() => _onRemoveWithValue.Remove(listener)));
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener listener) {
            _onRemove.Remove(listener);
            return this;
        }
        
        public ObservedDictionary<TKey, TValue> RemoveOnRemoveListener(ActionListener<TValue> listener) {
            _onRemoveWithValue.Remove(listener);
            return this;
        }
    }
}