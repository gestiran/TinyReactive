// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    public static class InputListenerExtension {
        public static void AddListenerValue<T1, T2>(this InputListener<T1> obj, ActionListener<T2> listener, IUnloadLink unload) where T2 : T1 {
            obj.AddListener(value =>
            {
                if (value is T2 target) {
                    listener.Invoke(target);
                }
            }, unload);
        }
    }
}