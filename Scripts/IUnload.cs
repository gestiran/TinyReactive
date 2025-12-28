// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

namespace TinyReactive {
    /// <summary> A flag for unsubscribing from events and stopping asynchronous actions in case of scene unloading. </summary>
    public interface IUnload {
        void Unload();
    }
}