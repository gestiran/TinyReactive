using System;
using Sirenix.OdinInspector;

namespace TinyReactive.Fields {
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker]
    public sealed class Observed<T> {
        public T value => _value;
        
        private event Action<T> onChange;
        
        [ShowInInspector, HideLabel, OnValueChanged("@Set(_value)")]
        private T _value;
        
        public Observed() : this(default) { }
        
        public Observed(T value) => _value = value;

        public void Set(T value) {
            _value = value;
            onChange?.Invoke(value);
        }

        public void AddListener(Action<T> listener) => onChange += listener;
        
        public void RemoveListener(Action<T> listener) => onChange -= listener;
        
        public static implicit operator T(Observed<T> value) => value._value;
    }
}