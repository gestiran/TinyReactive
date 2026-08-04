// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using TinyReactive.Exceptions;
using TinyUtilities.Logger;

namespace TinyReactive.Fields {
    /// <summary>
    /// Extension methods for <see cref="TinyReactive.UnloadPool">UnloadPool</see> and <see cref="TinyReactive.IUnload">IUnload</see>.
    /// </summary>
    public static class UnloadExtension {
        /// <summary> Unloads the list if needed. </summary>
        /// <param name="unload"> List for unloading. </param>
        /// <returns> Returns False if the list does not exist or has been unloaded earlier. </returns>
        public static bool TryUnload(this UnloadPool unload) {
            if (unload == null) {
                return false;
            }
            
            if (unload.isUnloaded) {
                return false;
            }
            
            unload.Unload();
            return true;
        }
        
        /// <summary> Unload all items in the collection, if needed. </summary>
        /// <param name="collection"> Collection for verification and unloading. </param>
        /// <typeparam name="T"> Any object type. </typeparam>
        public static void TryUnload<T>(this ICollection<T> collection) {
            foreach (T obj in collection) {
                obj.TryUnloadSingle();
            }
        }
        
        /// <summary> Unload all items in the collection. </summary>
        /// <param name="collection"> Collection for unloading. </param>
        /// <typeparam name="TUnload"> Type to be unloaded. </typeparam>
        public static void Unload<TUnload>(this ICollection<TUnload> collection) where TUnload : IUnload {
            foreach (TUnload obj in collection) {
                try {
                    obj.Unload();
                } catch (Exception exception) {
                    DebugUtility.LogError(new UnloadException($"{obj}", exception));
                }
            }
        }
        
        /// <summary> Unload all keys and values in the dictionary. </summary>
        /// <param name="dictionary"> Dictionary for unloading. </param>
        /// <typeparam name="TUnload"> Type to be unloaded. </typeparam>
        public static void Unload<TUnload>(this Dictionary<TUnload, TUnload> dictionary) where TUnload : IUnload {
            foreach (KeyValuePair<TUnload, TUnload> unload in dictionary) {
                try {
                    unload.Key.Unload();
                    unload.Value.Unload();
                } catch (Exception exception) {
                    DebugUtility.LogError(new UnloadException($"{unload}", exception));
                }
            }
        }
        
        /// <summary> Unload all keys in the dictionary, if needed. </summary>
        /// <param name="dictionary"> Dictionary for unloading. </param>
        /// <typeparam name="T1"> Any object type. </typeparam>
        /// <typeparam name="T2"> Any object type. </typeparam>
        public static void TryUnloadKeys<T1, T2>(this Dictionary<T1, T2> dictionary) {
            foreach (T1 unload in dictionary.Keys) {
                unload.TryUnloadSingle();
            }
        }
        
        /// <summary> Unload all keys in the dictionary. </summary>
        /// <param name="dictionary"> Dictionary for unloading. </param>
        /// <typeparam name="TUnload"> Type to be unloaded. </typeparam>
        /// <typeparam name="T"> Any object type. </typeparam>
        public static void UnloadKeys<TUnload, T>(this Dictionary<TUnload, T> dictionary) where TUnload : IUnload {
            foreach (TUnload unload in dictionary.Keys) {
                try {
                    unload.Unload();
                } catch (Exception exception) {
                    DebugUtility.LogError(new UnloadException($"{unload}", exception));
                }
            }
        }
        
        /// <summary> Unload all values in the dictionary, if needed. </summary>
        /// <param name="dictionary"> Dictionary for unloading. </param>
        /// <typeparam name="T1"> Any object type. </typeparam>
        /// <typeparam name="T2"> Any object type. </typeparam>
        public static void TryUnloadValues<T1, T2>(this Dictionary<T1, T2> dictionary) {
            foreach (T2 unload in dictionary.Values) {
                unload.TryUnloadSingle();
            }
        }
        
        /// <summary> Unload all values in the dictionary. </summary>
        /// <param name="dictionary"> Dictionary for unloading. </param>
        /// <typeparam name="T"> Any object type. </typeparam>
        /// <typeparam name="TUnload"> Type to be unloaded. </typeparam>
        public static void UnloadValues<T, TUnload>(this Dictionary<T, TUnload> dictionary) where TUnload : IUnload {
            foreach (TUnload unload in dictionary.Values) {
                unload.TryUnloadSingle();
            }
        }
        
        /// <summary> Unload the current item, if needed. </summary>
        /// <param name="obj"> Any object. </param>
        /// <typeparam name="T"> Any object type. </typeparam>
        public static void TryUnloadSingle<T>(this T obj) {
            if (obj is IUnload other) {
                try {
                    other.Unload();
                } catch (Exception exception) {
                    DebugUtility.LogError(new UnloadException($"{obj}", exception));
                }
            }
        }
        
        /// <summary> Unloads the current list and returns null. </summary>
        /// <param name="unload"> List for unloading. </param>
        /// <returns> Null. </returns>
        [Pure]
        public static UnloadPool Reset(this UnloadPool unload) {
            if (unload != null) {
                unload.Unload();
            }
            
            return null;
        }
        
        /// <summary> Unload the current list, if it exists, and return a new object. </summary>
        /// <param name="unload"> List for unloading. </param>
        /// <param name="result"> A new or cleared unload list. </param>
        public static void Recreate(this UnloadPool unload, out UnloadPool result) {
            if (unload != null) {
                unload.Unload();
                unload.ResetStatus();
                result = unload;
            } else {
                result = new UnloadPool();
            }
        }
        
        /// <summary> Unload the current list and return the reference. </summary>
        /// <param name="unload"> List for unloading. </param>
        /// <param name="result"> The reference unload list. </param>
        /// <param name="reference"> Reference list. </param>
        public static void Update(this UnloadPool unload, out UnloadPool result, in UnloadPool reference) {
            if (unload != null) {
                unload.Unload();
            }
            
            result = reference;
        }
        
        /// <summary>
        /// Unload the current list and return the reference.
        /// To simplify potential single-line expressions.
        /// </summary>
        /// <param name="unload"> List for unloading. </param>
        /// <param name="reference"> Reference list. </param>
        /// <returns> The reference unload list. </returns>
        [Pure]
        public static UnloadPool Update(this UnloadPool unload, in UnloadPool reference) {
            if (unload != null) {
                unload.Unload();
            }
            
            return reference;
        }
        
        /// <summary> Unload the current list, if it exists, and return a new object. </summary>
        /// <param name="unload"> List for unloading. </param>
        /// <returns> A new or cleared unload list. </returns>
        [Pure]
        public static UnloadPool Recreate(this UnloadPool unload) {
            if (unload == null) {
                return new UnloadPool();
            }
            
            unload.Unload();
            unload.ResetStatus();
            return unload;
        }
        
        /// <summary> Add the item to the list for unloading. </summary>
        /// <param name="obj"> Current item. </param>
        /// <param name="unload"> A reference to the list for unloading. </param>
        /// <typeparam name="TUnload"> Type to be unloaded. </typeparam>
        /// <returns> Current. </returns>
        public static TUnload WithUnload<TUnload>(this TUnload obj, IUnloadLink unload) where TUnload : IUnload {
            unload.Add(obj);
            return obj;
        }
    }
}