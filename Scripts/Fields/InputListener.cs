using System;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker]
#endif
    public sealed class InputListener {
        private event Action onClick;

    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send() => onClick?.Invoke();

        public void AddListener(Action listener) => onClick += listener;

        public void RemoveListener(Action listener) => onClick -= listener;
    }

#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker]
#endif
    public sealed class InputListener<T> {
        private event Action<T> onClick;

    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send(T data) => onClick?.Invoke(data);

        public void AddListener(Action<T> listener) => onClick += listener;

        public void RemoveListener(Action<T> listener) => onClick -= listener;
    }

#if ODIN_INSPECTOR
    [InlineProperty, HideReferenceObjectPicker]
#endif
    public sealed class InputListener<T1, T2> {
        private event Action<T1, T2> onClick;

    #if ODIN_INSPECTOR
        [Button]
    #endif
        public void Send(T1 data1, T2 data2) => onClick?.Invoke(data1, data2);

        public void AddListener(Action<T1, T2> listener) => onClick += listener;

        public void RemoveListener(Action<T1, T2> listener) => onClick -= listener;
    }
}