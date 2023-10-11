using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker]
#endif
    public sealed class Observed<T> {
        public T value => _value;

        private event Action<T> onChange;

    #if ODIN_INSPECTOR
        [ShowInInspector, HideLabel, OnValueChanged("@" + nameof(Set) + "(" + nameof(_value) + ")")]
    #endif
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