// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System.Runtime.CompilerServices;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyReactive.Editor.Fields {
    [DrawerPriority(0, 10, 0)]
    public sealed class InputChangerDrawer<T> : OdinValueDrawer<InputChanger<T>> where T : unmanaged {
        private static readonly ConditionalWeakTable<InspectorProperty, DrawerState> _cache;
        
        static InputChangerDrawer() {
            _cache = new ConditionalWeakTable<InspectorProperty, DrawerState>();
        }
        
        private sealed class ValueWrapper {
            public T value;
        }
        
        private sealed class DrawerState {
            public PropertyTree tree;
            
            public readonly ValueWrapper wrapper;
            
            public DrawerState() {
                wrapper = new ValueWrapper();
            }
        }
        
        protected override void DrawPropertyLayout(GUIContent label) {
            InputChanger<T> current = ValueEntry.SmartValue;
            
            if (current != null) {
                if (_cache.TryGetValue(Property, out DrawerState state) == false) {
                    state = new DrawerState();
                    state.tree = PropertyTree.Create(state.wrapper);
                    _cache.Add(Property, state);
                }
                
                state.tree.UpdateTree();
                
                SirenixEditorGUI.BeginHorizontalPropertyLayout(label);
                
                state.tree.RootProperty.Children[0].Draw(GUIContent.none);
                
                if (GUILayout.Button("Send", GUILayout.Width(131f))) {
                    T value = state.wrapper.value;
                    current.Send(ref value);
                    state.wrapper.value = value;
                }
                
                SirenixEditorGUI.EndHorizontalPropertyLayout();
                
                return;
            }
            
            CallNextDrawer(label);
        }
    }
}