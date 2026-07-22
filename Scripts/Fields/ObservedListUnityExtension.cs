// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#if UNITASK_ENABLE
using Task = Cysharp.Threading.Tasks.UniTask;
#else
using Task = System.Threading.Tasks.Task;
#endif

namespace TinyReactive.Fields {
    /// <summary> Added async add and remove options to <see cref="TinyReactive.Fields.ObservedList{T}">ObservedList</see> with UniTask. </summary>
    public static class ObservedListUnityExtension {
        /// <summary> Maximum allowable ANR when performing asynchronous operations. </summary>
        public static int asyncAnrMS { get; private set; }
        
        /// <summary> Locking multiple requests to a single object. </summary>
        private static readonly Dictionary<int, bool> _lock;
        
        static ObservedListUnityExtension() {
            _lock = new Dictionary<int, bool>(32);
            asyncAnrMS = 64;
        }
        
        /// <summary> Set the allowable ANR value when performing asynchronous operations. </summary>
        /// <param name="ms"> Value in milliseconds. </param>
        public static void OverrideDefaultANR(int ms) => asyncAnrMS = ms;
        
        [Obsolete("Can't add nothing!", true)]
        public static Task AddAsync<T>(this ObservedList<T> current) => default;
        
        /// <summary> Add items to the list based on the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="values"> The items that need to be added. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task AddAsync<T>(this ObservedList<T> current, [NotNull] params T[] values) {
            return current.AddAsync(asyncAnrMS, CancellationToken.None, values);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static Task AddAsync<T>(this ObservedList<T> current, CancellationToken cancellation) => default;
        
        /// <summary> Add items to the list based on the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="values"> The items that need to be added. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task AddAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] params T[] values) {
            return current.AddAsync(asyncAnrMS, cancellation, values);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static Task AddAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation) => default;
        
        /// <summary> Add items to the list based on the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="anr"> Maximum allowable ANR when performing asynchronous operations. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="values"> The items that need to be added. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static async Task AddAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] params T[] values) {
            if (_lock.TryAdd(current.id, true) == false) {
                return;
            }
            
            current.list.AddRange(values);
            DateTime now = DateTime.Now;
            
            if (current.onAdd.isDirty) {
                current.onAdd.Apply();
            }
            
            if (current.onAddWithValue.isDirty) {
                current.onAddWithValue.Apply();
            }
            
            for (int i = current.onAdd.Count - 1; i >= 0; i--) {
                current.onAdd[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onAddWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    current.onAddWithValue[i].Invoke(values[j]);
                }
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        /// <summary> Add an item to the list, taking into account the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="value"> The item to be added. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task AddAsync<T>(this ObservedList<T> current, [NotNull] T value) {
            return current.AddAsync(asyncAnrMS, CancellationToken.None, value);
        }
        
        /// <summary> Add an item to the list, taking into account the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="value"> The item to be added. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task AddAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] T value) {
            return current.AddAsync(asyncAnrMS, cancellation, value);
        }
        
        /// <summary> Add an item to the list, taking into account the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="anr"> Maximum allowable ANR when performing asynchronous operations. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="value"> The item to be added. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static async Task AddAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] T value) {
            if (_lock.TryAdd(current.id, true) == false) {
                return;
            }
            
            current.list.Add(value);
            DateTime now = DateTime.Now;
            
            if (current.onAdd.isDirty) {
                current.onAdd.Apply();
            }
            
            if (current.onAddWithValue.isDirty) {
                current.onAddWithValue.Apply();
            }
            
            for (int i = current.onAdd.Count - 1; i >= 0; i--) {
                current.onAdd[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onAddWithValue.Count - 1; i >= 0; i--) {
                current.onAddWithValue[i].Invoke(value);
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        /// <summary> Remove items to the list based on the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="values"> The items that need to be removed. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task RemoveAsync<T>(this ObservedList<T> current, [NotNull] params T[] values) {
            return current.RemoveAsync(asyncAnrMS, CancellationToken.None, values);
        }
        
        /// <summary> Remove items to the list based on the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="values"> The items that need to be removed. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task RemoveAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] params T[] values) {
            return current.RemoveAsync(asyncAnrMS, cancellation, values);
        }
        
        /// <summary> Remove items to the list based on the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="anr"> Maximum allowable ANR when performing asynchronous operations. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="values"> The items that need to be removed. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static async Task RemoveAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] params T[] values) {
            if (_lock.TryAdd(current.id, true) == false) {
                return;
            }
            
            for (int i = values.Length - 1; i >= 0; i--) {
                current.list.Remove(values[i]);
            }
            
            DateTime now = DateTime.Now;
            
            if (current.onRemove.isDirty) {
                current.onRemove.Apply();
            }
            
            if (current.onRemoveWithValue.isDirty) {
                current.onRemoveWithValue.Apply();
            }
            
            for (int i = current.onRemove.Count - 1; i >= 0; i--) {
                current.onRemove[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onRemoveWithValue.Count - 1; i >= 0; i--) {
                for (int j = 0; j < values.Length; j++) {
                    current.onRemoveWithValue[i].Invoke(values[j]);
                }
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        /// <summary> Remove an item to the list, taking into account the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="value"> The item to be removed. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task RemoveAsync<T>(this ObservedList<T> current, [NotNull] T value) {
            return current.RemoveAsync(asyncAnrMS, CancellationToken.None, value);
        }
        
        /// <summary> Remove an item to the list, taking into account the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="value"> The item to be removed. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static Task RemoveAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] T value) {
            return current.RemoveAsync(asyncAnrMS, cancellation, value);
        }
        
        /// <summary> Remove an item to the list, taking into account the app's ANR. </summary>
        /// <param name="current"> Current list. </param>
        /// <param name="anr"> Maximum allowable ANR when performing asynchronous operations. </param>
        /// <param name="cancellation"> Stop the operation. </param>
        /// <param name="value"> The item to be removed. </param>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        public static async Task RemoveAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] T value) {
            if (_lock.TryAdd(current.id, true) == false) {
                return;
            }
            
            current.list.Remove(value);
            DateTime now = DateTime.Now;
            
            if (current.onRemove.isDirty) {
                current.onRemove.Apply();
            }
            
            if (current.onRemoveWithValue.isDirty) {
                current.onRemoveWithValue.Apply();
            }
            
            for (int i = current.onRemove.Count - 1; i >= 0; i--) {
                current.onRemove[i].Invoke();
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            for (int i = current.onRemoveWithValue.Count - 1; i >= 0; i--) {
                current.onRemoveWithValue[i].Invoke(value);
                
                if (DateTime.Now.Subtract(now).TotalMilliseconds < anr) {
                    if (cancellation.IsCancellationRequested) {
                        return;
                    }
                    
                    continue;
                }
                
            #if UNITASK_ENABLE
                await Task.Yield(cancellation);
            #else
                await Task.Delay(16, cancellation);
            #endif
                
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
    }
}