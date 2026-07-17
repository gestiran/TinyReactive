// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

#if UNITASK_ENABLE
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace TinyReactive.Fields {
    public static class ObservedListUnityExtension {
        private static readonly Dictionary<int, bool> _lock;
        
        private const int _ASYNC_ANR_MS = 64;
        
        static ObservedListUnityExtension() {
            _lock = new Dictionary<int, bool>(32);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static UniTask AddAsync<T>(this ObservedList<T> current) => default;
        
        public static UniTask AddAsync<T>(this ObservedList<T> current, [NotNull] params T[] values) {
            return AddAsync(current, _ASYNC_ANR_MS, CancellationToken.None, values);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static UniTask AddAsync<T>(this ObservedList<T> current, CancellationToken cancellation) => default;
        
        public static UniTask AddAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] params T[] values) {
            return AddAsync(current, _ASYNC_ANR_MS, cancellation, values);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static UniTask AddAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation) => default;
        
        public static async UniTask AddAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] params T[] values) {
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
                
                await UniTask.Yield(cancellation);
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
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        public static UniTask AddAsync<T>(this ObservedList<T> current, [NotNull] T value) {
            return AddAsync(current, _ASYNC_ANR_MS, CancellationToken.None, value);
        }
        
        public static UniTask AddAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] T value) {
            return AddAsync(current, _ASYNC_ANR_MS, cancellation, value);
        }
        
        public static async UniTask AddAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] T value) {
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
                
                await UniTask.Yield(cancellation);
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
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        public static UniTask RemoveAsync<T>(ObservedList<T> current, [NotNull] params T[] values) {
            return RemoveAsync(current, _ASYNC_ANR_MS, CancellationToken.None, values);
        }
        
        public static UniTask RemoveAsync<T>(ObservedList<T> current, CancellationToken cancellation, [NotNull] params T[] values) {
            return RemoveAsync(current, _ASYNC_ANR_MS, cancellation, values);
        }
        
        public static async UniTask RemoveAsync<T>(ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] params T[] values) {
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
                
                await UniTask.Yield(cancellation);
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
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
        
        public static UniTask RemoveAsync<T>(this ObservedList<T> current, [NotNull] T value) {
            return RemoveAsync(current, _ASYNC_ANR_MS, CancellationToken.None, value);
        }
        
        public static UniTask RemoveAsync<T>(this ObservedList<T> current, CancellationToken cancellation, [NotNull] T value) {
            return RemoveAsync(current, _ASYNC_ANR_MS, cancellation, value);
        }
        
        public static async UniTask RemoveAsync<T>(this ObservedList<T> current, int anr, CancellationToken cancellation, [NotNull] T value) {
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
                
                await UniTask.Yield(cancellation);
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
                
                await UniTask.Yield(cancellation);
                now = DateTime.Now;
            }
            
            _lock.Remove(current.id);
        }
    }
}
#endif