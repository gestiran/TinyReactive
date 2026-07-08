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
                InspectorProperty valueProperty = Property.Children[ObservedDrawer.VALUE];
                
                if (valueProperty == null && Property.Children.Count > 0) {
                    valueProperty = Property.Children[0];
                }
                
                if (valueProperty != null) {
                    if (current is Observed<int> observedInt) {
                        EditorGUILayout.BeginHorizontal();
                        
                        DrawValue(label, valueProperty, current);
                        
                        if (ObservedDrawer.DrawButtonsInt(observedInt, GUILayout.Width(64f))) {
                            ValueEntry.Values.ForceMarkDirty();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    } else if (current is Observed<float> observedFloat) {
                        EditorGUILayout.BeginHorizontal();
                        
                        DrawValue(label, valueProperty, current);
                        
                        if (ObservedDrawer.DrawButtonsFloat(observedFloat, GUILayout.Width(64f))) {
                            ValueEntry.Values.ForceMarkDirty();
                        }
                        
                        EditorGUILayout.EndHorizontal();
                    } else {
                        DrawValue(label, valueProperty, current);
                    }
                }
                
                return;
            }
            
            CallNextDrawer(label);
        }
        
        private void DrawValue(GUIContent label, InspectorProperty property, Observed<T> current) {
            EditorGUI.BeginChangeCheck();
            property.Draw(label);
            
            if (EditorGUI.EndChangeCheck() && property.ValueEntry.WeakSmartValue is T newValue) {
                current.Set(newValue);
                ValueEntry.Values.ForceMarkDirty();
            }
        }
    }
    public static class ObservedDrawer {
        public const string VALUE = "value";
        public const string X2 = "x2";
        public const string X0_5 = "x0.5";
        
        public static bool DrawButtonsInt<T>(T observed, GUILayoutOption width) where T : Observed<int> {
            if (GUILayout.Button(X2, width)) {
                int value = observed.value;
                value = value == 0 ? 10 : value * 2;
                observed.Set(value);
                return true;
            }
            
            if (GUILayout.Button(X0_5, width)) {
                int value = observed.value;
                value = value > 0 && value <= 10 ? 0 : value / 2;
                observed.Set(value);
                return true;
            }
            
            return false;
        }
        
        public static bool DrawButtonsFloat<T>(T observed, GUILayoutOption width) where T : Observed<float> {
            if (GUILayout.Button(X2, width)) {
                float value = observed.value;
                value = value == 0f ? 10f : value * 2f;
                observed.Set(value);
                return true;
            }
            
            if (GUILayout.Button(X0_5, width)) {
                float value = observed.value;
                value = value > 0f && value <= 10f ? 0f : value * 0.5f;
                observed.Set(value);
                return true;
            }
            
            return false;
        }
    }
}