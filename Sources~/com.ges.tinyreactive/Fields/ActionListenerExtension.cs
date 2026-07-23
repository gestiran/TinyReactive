// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive.Fields {
    /// <summary>
    /// Added the option to call <see cref="System.Action.Invoke">Invoke</see>
    /// for all <see cref="System.Action">Action</see>
    /// nested within a <see cref="TinyReactive.Fields.LazyList{T}">LazyList</see>.
    /// </summary>
    public static class ActionListenerExtension {
        /// <summary> Apply the changes to the LazyList and invoke all nested ActionListeners. </summary>
        /// <param name="actions"> Current. </param>
        internal static void Invoke(this LazyList<ActionListener> actions) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.Count; i++) {
                actions[i].Invoke();
            }
        }
        
        /// <summary> Apply the changes to the LazyList and invoke all nested ActionListeners. </summary>
        /// <param name="actions"> Current. </param>
        /// <param name="value"> Passed value. </param>
        internal static void Invoke<T>(this LazyList<ActionListener<T>> actions, T value) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.Count; i++) {
                actions[i].Invoke(value);
            }
        }
        
        /// <summary> Apply the changes to the LazyList and invoke all nested ActionListeners. </summary>
        /// <param name="actions"> Current. </param>
        /// <param name="value1"> Passed value. </param>
        /// <param name="value2"> Passed value. </param>
        internal static void Invoke<T1, T2>(this LazyList<ActionListener<T1, T2>> actions, T1 value1, T2 value2) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.Count; i++) {
                actions[i].Invoke(value1, value2);
            }
        }
        
        /// <summary> Apply the changes to the LazyList and invoke all nested ActionListeners. </summary>
        /// <param name="actions"> Current. </param>
        /// <param name="value1"> Passed value. </param>
        /// <param name="value2"> Passed value. </param>
        /// <param name="value3"> Passed value. </param>
        internal static void Invoke<T1, T2, T3>(this LazyList<ActionListener<T1, T2, T3>> actions, T1 value1, T2 value2, T3 value3) {
            if (actions.isDirty) {
                actions.Apply();
            }
            
            for (int i = 0; i < actions.Count; i++) {
                actions[i].Invoke(value1, value2, value3);
            }
        }
    }
}