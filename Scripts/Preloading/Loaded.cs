using System;

namespace TinyReactive.Preloading {
    public abstract class Loaded<T> : IEquatable<T>, IEquatable<Loaded<T>> {
        public T value {
            get => _getAction();
            set => _setAction(value);
        }

        private T _value;
        private Func<T> _getAction;
        private Action<T> _setAction;

        public Loaded(T value) {
            _value = value;
            _getAction = GetValue;
            _setAction = SetValue;
        }
        
        protected Loaded() {
            _getAction = LoadAndChangeState;
            _setAction = SetAndChangeState;
        }

        private T LoadAndChangeState() {
            _value = Load();
            _getAction = GetValue;

            return _value;
        }
        
        private void SetAndChangeState(T newValue) {
            _value = newValue;
            _getAction = GetValue;
            _setAction = SetValue;
        }

        protected virtual T GetValue() => _value;
        
        protected virtual void SetValue(T newValue) => _value = newValue;

        protected abstract T Load();

        public static bool operator ==(Loaded<T> a, Loaded<T> b) => a.Equals(b);

        public static bool operator !=(Loaded<T> a, Loaded<T> b) => a.Equals(b) == false;
        
        public static bool operator ==(Loaded<T> a, T b) => a.Equals(b);

        public static bool operator !=(Loaded<T> a, T b) => a.Equals(b) == false;
        
        public static bool operator ==(T a, Loaded<T> b) =>  b.Equals(a);

        public static bool operator !=(T a, Loaded<T> b) => b.Equals(a) == false;

        public static implicit operator T(Loaded<T> value) => value.value;

        public bool Equals(Loaded<T> other) => other != null && other.value.Equals(value);

        public bool Equals(T other) => other != null && other.Equals(value);

        public override bool Equals(object obj) {
        #if UNITY_EDITOR
            if (obj == null) {
                return false;
            }
        #endif
            
            return obj is Loaded<T> other && other.value.Equals(value);
        }

        public override int GetHashCode() => value.GetHashCode();

        public override string ToString() => value.ToString();
    }
}