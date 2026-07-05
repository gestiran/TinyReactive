// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using TinyReactive.Fields;
using UnityEngine;

namespace TinyReactive.Editor.Fields {
    [DrawerPriority(0, 10, 0)]
    public sealed class InputListenerDrawer : OdinValueDrawer<InputListener> {
        protected override void DrawPropertyLayout(GUIContent label) {
            InputListener current = ValueEntry.SmartValue;
            
            if (current != null) {
                SirenixEditorGUI.BeginHorizontalPropertyLayout(label);
                
                if (GUILayout.Button("Send", GUILayout.ExpandWidth(true))) {
                    current.Send();
                }
                
                SirenixEditorGUI.EndHorizontalPropertyLayout();
                return;
            }
            
            CallNextDrawer(label);
        }
    }
}