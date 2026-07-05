// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Reflection;
using JetBrains.Annotations;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyReactive.Fields;

namespace TinyReactive.Editor.Fields {
    [ResolverPriority(1000), UsedImplicitly]
    public sealed class ObservedListMemberResolver<T> : OdinPropertyResolver<ObservedList<T>> {
        private InspectorPropertyInfo _listPropertyInfo;
        
        protected override void Initialize() {
            FieldInfo listField = typeof(ObservedList<T>).GetField("list", BindingFlags.Instance | BindingFlags.NonPublic);
            
            string label = Property.NiceName;
            
            Attribute[] attributes = {
                new ShowInInspectorAttribute(),
                new HideInEditorModeAttribute(),
                new HideReferenceObjectPickerAttribute(),
                new HideDuplicateReferenceBoxAttribute(),
                new LabelTextAttribute(label),
                new SearchableAttribute(),
                new ListDrawerSettingsAttribute {
                    HideAddButton = true,
                    HideRemoveButton = true,
                    DraggableItems = false,
                    DefaultExpandedState = false,
                    ListElementLabelName = "@ToString()"
                }
            };
            
            _listPropertyInfo = InspectorPropertyInfo.CreateForMember(Property, listField, true, attributes);
        }
        
        public override int ChildNameToIndex(string name) => name == "list" ? 0 : -1;
        
        public override InspectorPropertyInfo GetChildInfo(int childIndex) => _listPropertyInfo;
        
        protected override int GetChildCount(ObservedList<T> value) => 1;
    }
}