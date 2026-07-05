// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using Sirenix.OdinInspector.Editor;
using TinyReactive.Fields;
using UnityEditor;
using UnityEngine;

namespace TinyReactive.Editor.Fields {
    [DrawerPriority(0, 10, 0)]
    public sealed class ObservedDrawer<T> : OdinValueDrawer<Observed<T>> {
        protected override void DrawPropertyLayout(GUIContent label) {
            Observed<T> current = ValueEntry.SmartValue;
            
            if (current != null) {
                InspectorProperty valueProperty = Property.Children[ObservedLabels.VALUE];
                
                if (valueProperty == null && Property.Children.Count > 0) {
                    valueProperty = Property.Children[0];
                }
                
                if (valueProperty != null) {
                    EditorGUILayout.BeginHorizontal();
                    
                    EditorGUI.BeginChangeCheck();
                    valueProperty.Draw(label);
                    
                    if (EditorGUI.EndChangeCheck() && valueProperty.ValueEntry.WeakSmartValue is T newValue) {
                        current.Set(newValue);
                        ValueEntry.Values.ForceMarkDirty();
                    }
                    
                    if (current is Observed<int> observedInt) {
                        if (DrawButtonsInt(observedInt, GUILayout.Width(64f))) {
                            ValueEntry.Values.ForceMarkDirty();
                        }
                    } else if (current is Observed<float> observedFloat) {
                        if (DrawButtonsFloat(observedFloat, GUILayout.Width(64f))) {
                            ValueEntry.Values.ForceMarkDirty();
                        }
                    }
                    
                    EditorGUILayout.EndHorizontal();
                }
                
                return;
            }
            
            CallNextDrawer(label);
        }
        
        private bool DrawButtonsInt(Observed<int> observedInt, GUILayoutOption width) {
            if (GUILayout.Button(ObservedLabels.X2, width)) {
                int value = observedInt.value;
                value = value == 0 ? 10 : value * 2;
                observedInt.Set(value);
                return true;
            }
            
            if (GUILayout.Button(ObservedLabels.X0_5, width)) {
                int value = observedInt.value;
                value = value > 0 && value <= 10 ? 0 : value / 2;
                observedInt.Set(value);
                return true;
            }
            
            return false;
        }
        
        private bool DrawButtonsFloat(Observed<float> observedFloat, GUILayoutOption width) {
            if (GUILayout.Button(ObservedLabels.X2, width)) {
                float value = observedFloat.value;
                value = value == 0f ? 10f : value * 2f;
                observedFloat.Set(value);
                return true;
            }
            
            if (GUILayout.Button(ObservedLabels.X0_5, width)) {
                float value = observedFloat.value;
                value = value > 0f && value <= 10f ? 0f : value * 0.5f;
                observedFloat.Set(value);
                return true;
            }
            
            return false;
        }
    }
    public static class ObservedLabels {
        public const string VALUE = "value";
        public const string X2 = "x2";
        public const string X0_5 = "x0.5";
    }
}