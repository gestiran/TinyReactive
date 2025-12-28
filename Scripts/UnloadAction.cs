// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive {
    /// <summary> Container for Action participating in Unload resources. </summary>
    public sealed class UnloadAction : IUnload {
        private readonly Action _action;
        
        public UnloadAction(Action action) => _action = action;
        
        public static implicit operator Action(UnloadAction unload) => unload._action;
        
        public static implicit operator UnloadAction(Action action) => new UnloadAction(action);
        
        public void Unload() => _action.Invoke();
    }
}