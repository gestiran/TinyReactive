using System;
using Sirenix.OdinInspector;
using UnityEngine;

using UnityObject = UnityEngine.Object;

namespace TinyReactive.Links {
    [Serializable, InlineProperty]
    public sealed class SoftLink<T> : SmartLink<T> where T : MonoBehaviour {
        [HorizontalGroup, SuffixLabel("Root", true), HideLabel, SerializeField, ChildGameObjectsOnly(IncludeInactive = true), Required]
        private Transform _root;
        
        public T GetInstance() => GetInstance(_ => { });

        public T GetInstance(Action<T> initialization) => getInstance(_root, initialization);
    }
}