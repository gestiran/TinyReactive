using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TinyReactive.Links {
    [Serializable, InlineProperty]
    public abstract class SmartLink<T> where T : MonoBehaviour {
        public bool isCreated { get; private set; }
        
        [HorizontalGroup, SuffixLabel("Prefab", true), HideLabel, SerializeField, DisableIf(nameof(isCreated)), Required]
        private T _prefab;
        
        protected Func<Transform, Action<T>, T> getInstance;

        protected SmartLink() => getInstance = Initialize;
        
        public bool TryGetInstance(out T instance) {
            instance = _prefab;
            return isCreated;
        }

        private T Initialize(Transform parent, Action<T> initialization) {
            _prefab = CreateInstance(_prefab, parent);
            initialization(_prefab);
            isCreated = true;
            getInstance = (_,_) => _prefab;

            return _prefab;
        }

        private T CreateInstance(T prefab, Transform root) => Object.Instantiate(prefab, root);
    }
}