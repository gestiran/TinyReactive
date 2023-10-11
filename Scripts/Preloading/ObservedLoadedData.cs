using System;

namespace TinyReactive.Preloading {
    public sealed class ObservedLoadedData<T> : LoadedData<T> {
        private event Action<T> onChange;
        
        public ObservedLoadedData(T value) : base(value) { }

        public ObservedLoadedData(Func<T> loadAction) : base(loadAction) { }

        protected override void SetValue(T newValue) {
            base.SetValue(newValue);
            onChange?.Invoke(newValue);
        }
        
        public void AddListener(Action<T> listener) => onChange += listener;
        
        public void RemoveListener(Action<T> listener) => onChange -= listener;
    }
}