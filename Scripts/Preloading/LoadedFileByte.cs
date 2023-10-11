using System;

namespace TinyReactive.Preloading {
    public class LoadedData<T> : Loaded<T> {
        private Func<T> _loadAction;
        
        public LoadedData(T value) : base(value) { }
        
        public LoadedData(Func<T> loadAction) => _loadAction = loadAction;

        protected override T Load() {
            T newValue = _loadAction();
            _loadAction = null;
            return newValue;
        }
    }
}