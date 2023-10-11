using System;
using UnityEngine;

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Links {
#if ODIN_INSPECTOR
    [InlineProperty]
#endif
    [Serializable]
    public sealed class HardLink<T> : SmartLink<T> where T : MonoBehaviour {
        public T GetInstance(Transform parent) => GetInstance(parent, _ => { });

        public T GetInstance(Transform parent, Action<T> initialization) => getInstance(parent, initialization);
    }
}