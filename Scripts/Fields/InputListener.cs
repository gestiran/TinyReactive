using System;
using Sirenix.OdinInspector;

namespace TinyReactive.Fields {
    [InlineProperty, HideReferenceObjectPicker]
    public sealed class InputListener {
        private event Action onClick;

        [Button]
        public void Send() => onClick?.Invoke();
        
        public void AddListener(Action listener) => onClick += listener;
        
        public void RemoveListener(Action listener) => onClick -= listener;
    }
    
    [InlineProperty, HideReferenceObjectPicker]
    public sealed class InputListener<T> {
        private event Action<T> onClick;

        [Button]
        public void Send(T data) => onClick?.Invoke(data);
        
        public void AddListener(Action<T> listener) => onClick += listener;
        
        public void RemoveListener(Action<T> listener) => onClick -= listener;
    }
    
    [InlineProperty, HideReferenceObjectPicker]
    public sealed class InputListener<T1,T2> {
        private event Action<T1,T2> onClick;

        [Button]
        public void Send(T1 data1, T2 data2) => onClick?.Invoke(data1, data2);
        
        public void AddListener(Action<T1,T2> listener) => onClick += listener;
        
        public void RemoveListener(Action<T1,T2> listener) => onClick -= listener;
    }
}