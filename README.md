# TinyReactive

A lightweight reactive programming library for C#, optimized for use in Unity and other .NET environments. It provides a convenient toolkit for working with reactive data and events.

**Key Features:**
- Notification of all subscribers when data changes
- Use of the `Unload` collection for automatic unsubscribe and prevention of memory leaks
- Support for passing `unmanaged` data by reference
- Serialization to a base data format

## Table of Contents

- [Installation](#installation)
    - [Unity](#unity-openupm-or-tgz)
    - [NuGet](#nuget---without-unity-dependency)
- [Usage](#usage)
    - [`Observed<T>`](#observedt)
    - [`InputListener` and `InputListener<T>`](#inputlistener-and-inputlistenert)
    - [`InputChanger<T>`](#inputchangert)
    - [`ObservedList<T>`](#observedlistt)
    - [`ObservedDictionary<TKey, TValue>`](#observeddictionarytkey-tvalue)
- [Serialization](#serialization)
- [Tools and Extensions](#tools-and-extensions)

## Installation

> You should replace the "1.0.0" version shown in the examples with the current or available version!

### Unity (OpenUPM or TGZ)

This package is available on [OpenUPM](https://openupm.com/packages/com.ges.tinyreactive/) for Unity.

`Project/Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.ges.tinyreactive": "1.0.0"
  },
  "scopedRegistries": [
    {
      "name": "package.openupm.com",
      "url": "https://package.openupm.com",
      "scopes": [
        "com.ges"
      ]
    }
  ]
}
```

It is also available in `.tgz` format in the current [releases](https://github.com/gestiran/TinyReactive/releases). After downloading, place the `.tgz` archive in your project's `Packages` folder, and add a reference to it in your `manifest.json` file.

`Project/Packages/manifest.json`:

```json
{
  "dependencies": {
    "com.ges.tinyreactive": "file:com.ges.tinyreactive-1.0.0.tgz"
  }
}
```

### NuGet - Without Unity dependency

To install from [NuGet](https://www.nuget.org/packages/com.ges.tinyreactive), there is a library without a Unity dependency.

```xml
<PackageReference Include="com.ges.tinyreactive" Version="1.0.0" />
```

## Usage

### `Observed<T>`

Allows tracking changes to a `T` value.

```csharp
// Create Observed field
Observed<int> counter = new Observed<int>(100);

// Add listeners
counter.AddListener(ListenValue);
counter.AddListener(ListenValueChange);
counter.AddListener(ListenChange);

// Get current value
Console.WriteLine($"Counter current value: {counter.value}");

// Change value
counter.Set(90);

// Will be ignored because the current value is already 90
counter.TrySet(90);

// Change value without invoke listeners
counter.SetSilent(60);

// Remove listeners
counter.RemoveListener(ListenValue);
counter.RemoveListener(ListenValueChange);
counter.RemoveListener(ListenChange);

// Change value without any listeners
counter.Set(0);

// Listen value change
void ListenValue(int newValue) {
	Console.WriteLine($"Counter changed: {newValue}");
}

// Listen value change with current value
void ListenValueChange(int currentValue, int newValue) {
	Console.WriteLine($"Counter changed: from {currentValue} to {newValue}");
}

// Listen any value change
void ListenChange() {
	Console.WriteLine("Counter changed");
}
```

```cmd
Counter current value: 100
Counter changed: 90
Counter changed: from 100 to 90
Counter changed
```

### `InputListener` and `InputListener<T>`

A trackable event without storing the current value.

```csharp
// Create InputListener field
InputListener inputEvent = new InputListener();

// Add listeners
inputEvent.AddListener(ListenChange);

// Send event
inputEvent.Send();

// Remove listeners
inputEvent.RemoveListener(ListenChange);

// Listen any change
void ListenChange() {
	Console.WriteLine("Input Event");
}
```

```cmd
Input Event
```

Variants with additional parameters are also available.

```csharp
// Create InputListener field
InputListener<int> inputEvent = new InputListener<int>();

// Add listeners
inputEvent.AddListener(ListenValue);
inputEvent.AddListener(ListenChange);

// Send event
inputEvent.Send(10);

// Remove listeners
inputEvent.RemoveListener(ListenValue);
inputEvent.RemoveListener(ListenChange);

// Listen value change
void ListenValue(int newValue) {
	Console.WriteLine($"Input Event: {newValue}");
}

// Listen any change
void ListenChange() {
	Console.WriteLine("Input Event");
}
```

```cmd
Input Event: 10
Input Event
```

### `InputChanger<T>`

Exists for tracking and modifying `unmanaged` values.

```csharp
// Create InputChanger field
InputChanger<int> changeEvent = new InputChanger<int>();

// Add listeners
changeEvent.AddListener(ValueModify);

// Change value
int currentValue = 100;
changeEvent.Send(ref currentValue);
Console.WriteLine($"Changed first: {currentValue}");

// Remove listeners
changeEvent.RemoveListener(ValueModify);

// Change value without any listeners
changeEvent.Send(ref currentValue);
Console.WriteLine($"Changed second: {currentValue}");

// Listen value change
void ValueModify(ref int value) {
	value += 10;
}
```

```cmd
Changed first: 110
Changed second: 110
```

### `ObservedList<T>`

Tracks the addition and removal of values in a `List<T>`, as well as its clearing.

```csharp
// Create ObservedList field
ObservedList<int> values = new ObservedList<int>();

// Add listeners
values.AddOnAddListener(OnValueAdd);
values.AddOnRemoveListener(OnValueRemove);

// Change values
values.Add(15);
values.Add(25);
values.Remove(15);
values.Add(40);

Console.WriteLine($"Current values: {values.Count}");

foreach(int item in values) {
	Console.WriteLine($"Value: {item}");
}

// Remove listeners
values.RemoveOnAddListener(OnValueAdd);
values.RemoveOnRemoveListener(OnValueRemove);

// Listen value add
void OnValueAdd(int value) {
	Console.WriteLine($"Added: {value}");
}

// Listen value add
void OnValueRemove(int value) {
	Console.WriteLine($"Removed: {value}");
}

```

```cmd
Added: 15
Added: 25
Removed: 15
Added: 40

Current values: 2
Value: 25
Value: 40
```

### `ObservedDictionary<TKey, TValue>`

...

## Serialization

The `Observed<T>` and `ObservedList<T>` classes have built-in converters to the base type `T` and `List<T>` respectively. Implemented via `System.Text.Json`.

## Tools and Extensions

...
