// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;

namespace TinyReactive {
    /// <summary> Container for <see cref="System.Action">action</see> participating in <see cref="TinyReactive.IUnload">unload</see> resources. </summary>
    public sealed class UnloadAction : IUnload {
        /// <summary> Reference to the unload event. </summary>
        private readonly Action _action;
        
        /// <summary> Create <see cref="System.Action">action</see> container. </summary>
        /// <param name="action"> Default <see cref="System.Action">action</see>, can`t be null. </param>
        public UnloadAction(Action action) {
            if (action == null) {
                throw new NullReferenceException("Action can`t be null!");
            }
            
            _action = action;
        }
        
        public static implicit operator Action(UnloadAction unload) => unload._action;
        
        public static implicit operator UnloadAction(Action action) => new UnloadAction(action);
        
        /// <summary> Invoke referenced <see cref="_action">action</see> event. </summary>
        public void Unload() => _action.Invoke();
    }
}