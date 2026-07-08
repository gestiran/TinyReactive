// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Reflection;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using TinyReactive.Fields;

namespace TinyReactive.Editor.Fields {
    public sealed class ObservedAttributeProcessor<T> : OdinAttributeProcessor<Observed<T>> {
        public override void ProcessChildMemberAttributes(InspectorProperty parentProperty, MemberInfo member, List<Attribute> attributes) {
            if (member.Name == ObservedDrawer.VALUE) {
                attributes.Add(new ShowInInspectorAttribute());
            }
        }
    }
}