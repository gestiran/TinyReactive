using System;
using System.Diagnostics.CodeAnalysis;

namespace TinyReactive.Fields {
    [AttributeUsage(AttributeTargets.Field)]
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public sealed class ObservedDrawerSettingsAttribute : Attribute {
        /// <summary> Adds the display of multiplier buttons "x0.5" and "x2".<br/> Default is False. </summary>
        public bool ShowButtons { get; set; }
    }
}