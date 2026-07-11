// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    public static class ActionListenerExtension {
        internal static void Invoke(this LazyList<ActionListener> actions) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.count; i++) {
                actions[i].Invoke();
            }
        }
        
        internal static void Invoke<T>(this LazyList<ActionListener<T>> actions, T value) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.count; i++) {
                actions[i].Invoke(value);
            }
        }
        
        internal static void Invoke<T1, T2>(this LazyList<ActionListener<T1, T2>> actions, T1 value1, T2 value2) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.count; i++) {
                actions[i].Invoke(value1, value2);
            }
        }
        
        internal static void Invoke<T1, T2, T3>(this LazyList<ActionListener<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.count; i++) {
                actions[i].Invoke(value1, value2, value3);
            }
        }
    }
}