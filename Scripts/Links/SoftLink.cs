using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using UnityObject = UnityEngine.Object;

namespace TinyReactive.Links {
#if ODIN_INSPECTOR
    [InlineProperty]
#endif
    [Serializable]
    public sealed class SoftLink<T> : SmartLink<T> where T : MonoBehaviour {
    #if ODIN_INSPECTOR
        [HorizontalGroup, SuffixLabel("Root", true), HideLabel, ChildGameObjectsOnly(IncludeInactive = true), Required]
    #endif
        [SerializeField]
        private Transform _root;

        public T GetInstance() => GetInstance(_ => { });

        public T GetInstance(Action<T> initialization) => getInstance(_root, initialization);
    }
}