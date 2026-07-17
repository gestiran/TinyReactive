// Copyright (c) 2023 Derek Sliman
// Licensed under the MIT License. See LICENSE.md for details.

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TinyUtilities;

namespace TinyReactive.Fields {
    /// <summary> Extension methods for <see cref="Observed{T}">Observed</see>. </summary>
    public static class ObservedExtension {
        /// <summary> Increases the current value by 1. </summary>
        /// <param name="obj"> Current. </param>
        public static void Increment(this Observed<int> obj) => obj.Set(obj.value + 1);
        
        /// <summary> Increases the current value by 1. </summary>
        /// <param name="obj"> Current. </param>
        public static void Increment(this Observed<float> obj) => obj.Set(obj.value + 1);
        
        /// <summary> Decreases the current value by 1. </summary>
        /// <param name="obj"> Current. </param>
        public static void Decrement(this Observed<int> obj) => obj.Set(obj.value - 1);
        
        /// <summary> Decreases the current value by 1. </summary>
        /// <param name="obj"> Current. </param>
        public static void Decrement(this Observed<float> obj) => obj.Set(obj.value - 1);
        
        /// <summary> Increases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        public static void AddValue(this Observed<int> obj, int value) => obj.Set(obj.value + value);
        
        /// <summary> Increases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        public static void AddValueSilent(this Observed<int> obj, int value) => obj.SetSilent(obj.value + value);
        
        /// <summary> Increases the current value if it is not equal to 0. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        public static void AddValueIfAvailable(this Observed<int> obj, int value) {
            if (value == 0) {
                return;
            }
            
            obj.AddValue(value);
        }
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<int> obj) {
            // Do nothing
        }
        
        [Obsolete("Replaced to Equals!", true)]
        public static bool IsCurrent(this Observed<int> obj, int current) => obj.Equals(current);
        
        [Obsolete("Can't add nothing!", true)]
        public static void AddValue(this Observed<float> obj) {
            // Do nothing
        }
        
        /// <summary> Increases the current value to the specified maximum limit. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        /// <param name="maxValue"> Maximum value. </param>
        public static void AddValueMax(this Observed<int> obj, int value, int maxValue) => obj.Set(MathfUtility.Min(obj.value + value, maxValue));
        
        /// <summary> Increases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Amounts to add. </param>
        public static void AddValue(this Observed<int> obj, [NotNull] params int[] values) {
            int value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            obj.Set(value);
        }
        
        /// <summary> Increases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        public static void AddValue(this Observed<float> obj, float value) => obj.Set(obj.value + value);
        
        /// <summary> Increases the current value if it is not equal to 0. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        public static void AddValueIfAvailable(this Observed<float> obj, float value) {
            if (value == 0) {
                return;
            }
            
            obj.AddValue(value);
        }
        
        /// <summary> Increases the current value to the specified maximum limit. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to add. </param>
        /// <param name="maxValue"> Maximum value. </param>
        public static void AddValueMax(this Observed<float> obj, float value, float maxValue) => obj.Set(MathfUtility.Min(obj.value + value, maxValue));
        
        /// <summary> Increases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Amounts to add. </param>
        public static void AddValue(this Observed<float> obj, [NotNull] float[] values) {
            float value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value += values[i];
            }
            
            obj.Set(value);
        }
        
        [Obsolete("Can't subtract nothing!", true)]
        public static void SubtractValue(this Observed<int> obj) {
            // Do nothing
        }
        
        /// <summary> Decreases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        public static void SubtractValue(this Observed<int> obj, int value) => obj.Set(obj.value - value);
        
        /// <summary> Decreases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        public static void SubtractValueSilent(this Observed<int> obj, int value) => obj.SetSilent(obj.value - value);
        
        /// <summary> Decreases the current value if it is not equal to 0. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        public static void SubtractValueIfAvailable(this Observed<int> obj, int value) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValue(value);
        }
        
        /// <summary> Decreases the current value to the specified minimum limit if it is not equal to 0. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        /// <param name="minValue"> Minimum value. </param>
        public static void SubtractValueIfAvailableLimit(this Observed<int> obj, int value, int minValue) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValueLimit(value, minValue);
        }
        
        /// <summary> Decreases the current value to the specified minimum limit. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        /// <param name="minValue"> Minimum value. </param>
        public static void SubtractValueLimit(this Observed<int> obj, int value, int minValue) {
            obj.Set(MathfUtility.Max(obj.value - value, minValue));
        }
        
        /// <summary> Decreases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Amounts to subtract. </param>
        public static void SubtractValue(this Observed<int> obj, [NotNull] params int[] values) {
            int value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            obj.Set(value);
        }
        
        /// <summary> Decreases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        public static void SubtractValue(this Observed<float> obj, float value) => obj.Set(obj.value - value);
        
        /// <summary> Decreases the current value if it is not equal to 0. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        public static void SubtractValueIfAvailable(this Observed<float> obj, float value) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValue(value);
        }
        
        /// <summary> Decreases the current value to the specified minimum limit if it is not equal to 0. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        /// <param name="minValue"> Minimum value. </param>
        public static void SubtractValueIfAvailableLimit(this Observed<float> obj, float value, float minValue) {
            if (value == 0) {
                return;
            }
            
            obj.SubtractValueLimit(value, minValue);
        }
        
        /// <summary> Decreases the current value to the specified minimum limit. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Amount to subtract. </param>
        /// <param name="minValue"> Minimum value. </param>
        public static void SubtractValueLimit(this Observed<float> obj, float value, float minValue) {
            obj.Set(MathfUtility.Max(obj.value - value, minValue));
        }
        
        /// <summary> Decreases the current value. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Amounts to subtract. </param>
        public static void SubtractValue(this Observed<float> obj, [NotNull] params float[] values) {
            float value = obj.value;
            
            for (int i = 0; i < values.Length; i++) {
                value -= values[i];
            }
            
            obj.Set(value);
        }
        
        /// <summary> Get the value after it has been changed. </summary>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        /// <param name="obj"> Current. </param>
        /// <param name="cancellation"> Token to cancel the wait. </param>
        /// <returns> A new value. </returns>
        public static async Task<T> GetFirstAsync<T>(this Observed<T> obj, CancellationToken cancellation) {
            T result = default;
            bool isCompleted = false;
            
            UnloadPool unload = new UnloadPool();
            
            obj.AddListener(value =>
                            {
                                result = value;
                                isCompleted = true;
                            },
                            unload);
            
            try {
                while (isCompleted == false) {
                    await Task.Delay(1, cancellation);
                }
            } finally {
                unload.Unload();
            }
            
            return result;
        }
        
        /// <summary> Set a new value if it is not equal to the current one. </summary>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Target value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySet<T>(this Observed<T> obj, T value) {
            if (EqualityComparer<T>.Default.Equals(obj.value, value)) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        /// <summary> Subtract the specified number of seconds. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="seconds"> Number of seconds. </param>
        public static void SubtractSecond(this Observed<TimeSpan> obj, int seconds) {
            obj.Set(obj.value.Subtract(new TimeSpan(0, 0, seconds)));
        }
        
        /// <summary> Set a new value if the current value is equal to the expected value. </summary>
        /// <typeparam name="T"> The type of the stored value. </typeparam>
        /// <param name="obj"> Current. </param>
        /// <param name="expected"> Expected value. </param>
        /// <param name="value"> Target value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetWhen<T>(this Observed<T> obj, T expected, T value) {
            if (EqualityComparer<T>.Default.Equals(obj.value, expected) == false) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        /// <summary> Set the value if it is greater than the current one. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Target value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetNext(this Observed<int> obj, int value) {
            if (obj.value >= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        /// <summary> Set the value if it is greater than the current one. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Target value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetNext(this Observed<float> obj, float value) {
            if (obj.value >= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        [Obsolete("Can't use without parameters!", true)]
        public static bool TrySetMin(this Observed<int> obj) => false;
        
        [Obsolete("Can't use without parameters!", true)]
        public static bool TrySetMin(this Observed<float> obj) => false;
        
        /// <summary> Set the minimum value from the passed array. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Target values. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetMin(this Observed<float> obj, params float[] values) => obj.TrySet(MathfUtility.Min(values));
        
        /// <summary> Set the minimum value from the passed array. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Target values. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetMin(this Observed<int> obj, params int[] values) => obj.TrySet(MathfUtility.Min(values));
        
        /// <summary> Set the value to a value between the specified values. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Target value. </param>
        /// <param name="min"> Minimum value. </param>
        /// <param name="max"> Maximum value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetClamp(this Observed<int> obj, int value, int min, int max) => obj.TrySet(MathfUtility.Clamp(value, min, max));
        
        [Obsolete("Can't use without parameters!", true)]
        public static bool TrySetMax(this Observed<int> obj) => false;
        
        [Obsolete("Can't use without parameters!", true)]
        public static bool TrySetMax(this Observed<float> obj) => false;
        
        /// <summary> Set the maximum value from the passed array. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Target values. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetMax(this Observed<float> obj, params float[] values) => obj.TrySet(MathfUtility.Max(values));
        
        /// <summary> Set the maximum value from the passed array. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="values"> Target values. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetMax(this Observed<int> obj, params int[] values) => obj.TrySet(MathfUtility.Max(values));
        
        /// <summary> Set the value if it is less than the current one. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Target value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetPrevious(this Observed<int> obj, int value) {
            if (obj.value <= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        /// <summary> Set the value if it is less than the current one. </summary>
        /// <param name="obj"> Current. </param>
        /// <param name="value"> Target value. </param>
        /// <returns> Operation status. </returns>
        public static bool TrySetPrevious(this Observed<float> obj, float value) {
            if (obj.value <= value) {
                return false;
            }
            
            obj.Set(value);
            return true;
        }
        
        /// <summary> Flips the current boolean value. </summary>
        /// <param name="obj"> Current. </param>
        public static void Toggle(this Observed<bool> obj) {
            if (obj.value) {
                obj.Set(false);
            } else {
                obj.Set(true);
            }
        }
    }
}