// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
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
                    ObservedDrawerSettingsAttribute settings = Property.GetAttribute<ObservedDrawerSettingsAttribute>();
                    
                    if (settings != null) {
                        if (settings.ShowButtons) {
                            if (current is Observed<int> observedInt) {
                                if (ObservedDrawer.DrawValueAndButtonsInt(observedInt, label)) {
                                    ValueEntry.Values.ForceMarkDirty();
                                }
                            } else if (current is Observed<float> observedFloat) {
                                if (ObservedDrawer.DrawValueAndButtonsFloat(observedFloat, label)) {
                                    ValueEntry.Values.ForceMarkDirty();
                                }
                            } else {
                                DrawValue(label, valueProperty, current);
                            }
                        } else {
                            DrawValue(label, valueProperty, current);
                        }
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
        public const float BUTTONS_WIDTH = 32f;
        
        private const float _SPACING = 2f;
        
        public static bool DrawValueAndButtonsInt<T>(T observed, GUIContent label) where T : Observed<int> {
            GetControlRects(label, out Rect valueRect, out Rect x2Rect, out Rect x05Rect);
            
            bool changed = false;
            
            EditorGUI.BeginChangeCheck();
            int value = SirenixEditorFields.IntField(valueRect, GetFieldLabel(label), observed.value);
            
            if (EditorGUI.EndChangeCheck()) {
                observed.Set(value);
                changed = true;
            }
            
            if (GUI.Button(x2Rect, X2)) {
                value = observed.value;
                observed.Set(value == 0 ? 10 : value * 2);
                changed = true;
            }
            
            if (GUI.Button(x05Rect, X0_5)) {
                value = observed.value;
                observed.Set(value > 0 && value <= 10 ? 0 : value / 2);
                changed = true;
            }
            
            return changed;
        }
        
        public static bool DrawValueAndButtonsFloat<T>(T observed, GUIContent label) where T : Observed<float> {
            GetControlRects(label, out Rect valueRect, out Rect x2Rect, out Rect x05Rect);
            
            bool changed = false;
            
            EditorGUI.BeginChangeCheck();
            float value = SirenixEditorFields.FloatField(valueRect, GetFieldLabel(label), observed.value);
            
            if (EditorGUI.EndChangeCheck()) {
                observed.Set(value);
                changed = true;
            }
            
            if (GUI.Button(x2Rect, X2)) {
                value = observed.value;
                observed.Set(value == 0f ? 10f : value * 2f);
                changed = true;
            }
            
            if (GUI.Button(x05Rect, X0_5)) {
                value = observed.value;
                observed.Set(value > 0f && value <= 10f ? 0f : value * 0.5f);
                changed = true;
            }
            
            return changed;
        }
        
        private static void GetControlRects(GUIContent label, out Rect valueRect, out Rect x2Rect, out Rect x05Rect) {
            GUIContent fieldLabel = GetFieldLabel(label);
            float buttonsWidth = BUTTONS_WIDTH * 2f + _SPACING * 2f;
            Rect rect = EditorGUILayout.GetControlRect(fieldLabel != null, GUILayout.MinWidth(buttonsWidth), GUILayout.ExpandWidth(true));
            
            valueRect = rect;
            valueRect.width = Mathf.Max(0f, rect.width - buttonsWidth);
            
            x2Rect = rect;
            x2Rect.x = valueRect.xMax + _SPACING;
            x2Rect.width = BUTTONS_WIDTH;
            
            x05Rect = rect;
            x05Rect.x = x2Rect.xMax + _SPACING;
            x05Rect.width = BUTTONS_WIDTH;
        }
        
        private static GUIContent GetFieldLabel(GUIContent label) {
            return label == null || string.IsNullOrEmpty(label.text) ? null : label;
        }
    }
}