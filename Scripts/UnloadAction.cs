// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if UNITY_2017_1_OR_NEWER
#define UNITY_ENGINE
#endif

using System;

namespace TinyReactive {
    /// <include file="../Documentation~/Scripts/UnloadAction.xml" path="docs/UnloadAction/*" />
    public sealed class UnloadAction : IUnload {
        /// <include file="../Documentation~/Scripts/UnloadAction.xml" path="docs/fields/_action/*" />
        private readonly Action _action;
        
        /// <include file="../Documentation~/Scripts/UnloadAction.xml" path="docs/constructor/main/*" />
        public UnloadAction(Action action) {
        #if UNITY_ENGINE && UNITY_EDITOR
            if (action == null) {
                throw new NullReferenceException("Action can`t be null!");
            }
        #endif
            
            _action = action;
        }
        
        public static implicit operator Action(UnloadAction unload) => unload._action;
        
        public static implicit operator UnloadAction(Action action) => new UnloadAction(action);
        
        /// <include file="../Documentation~/Scripts/UnloadAction.xml" path="docs/methods/Unload/*" />
        public void Unload() => _action.Invoke();
    }
}