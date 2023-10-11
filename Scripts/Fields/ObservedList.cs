#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

namespace TinyReactive.Fields {
#if ODIN_INSPECTOR
    [ShowInInspector, InlineProperty, HideReferenceObjectPicker]
#endif
    public sealed class ObservedList<T> { }
}