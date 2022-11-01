# Cecil Attributes

[![openupm](https://img.shields.io/npm/v/se.hertzole.cecil-attributes?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/se.hertzole.cecil-attributes/)
![Unity 2019.4 tests](https://github.com/Hertzole/cecil-attributes/actions/workflows/test_2019.yml/badge.svg)
![Unity 2020.3 tests](https://github.com/Hertzole/cecil-attributes/actions/workflows/test_2020.yml/badge.svg)
![Unity 2021.2 tests](https://github.com/Hertzole/cecil-attributes/actions/workflows/test_2021.yml/badge.svg)

## 📕 Table of Contents
- [What, why, and how?](#-what-why-and-how)
   - [What](#what)
   - [Why](#why)
   - [How](#how)
- [Installation](#-installation)
   - [OpenUPM (Recommended)](#openupm-recommended)
   - [Unity package manager through git](#unity-package-manager-through-git)
- [The Attributes](#-the-attributes)
   - [Reset Static](#reset-static)
   - [Log Called](#log-called)
   - [Find Property](#find-property)
   - [Timed](#timed)
   - [Mark In Profiler](#mark-in-profiler)
   - [Get Component](#get-component)
   - [Required](#required)
- [License](#-license)

## ❓ What, why, and how?

### What
Cecil attributes allows you to use normal C# attributes to automatically inject code into your compiled code. That way you can automate small repetitive tasks with a single attribute.

### Why
There are several small tasks that aren't necessarily difficult, but time consuimg to do, like [adding a log message for when a method is called](#log-called). I wanted to automate some of these tasks so I can just add an attribute and it will generate the code for me, like small code snippets!

### How
Cecil Attributes uses [Mono.Cecil](https://github.com/jbevain/cecil) and IL weaving to inject code into your compiled assemblies. By carefully building the instructions for fields, methods, and classes it can create brand new code and alter existing code.   
You may be thinking, why not source generators? Simple: it can't modify existing code, which is required for some of these attributes. That way they can much more easily adapt to your code instead of you having to work around the code it creates.

## 📦 Installation

Cecil Attributes supports all Unity versions from **Unity 2019.4** and onward.

### OpenUPM (Recommended)
1. Add the OpenUPM reigstry.   
   Click in the menu bar Edit → Project Settings... → Package Manager
   Add a new scoped registry with the following parameters:  
   Name: `OpenUPM`  
   URL: `https://package.openupm.com`  
   Scopes:  
   - `com.openupm`  
   - `se.hertzole.cecil-attributes`
2. Click apply and close the project settings.
3. Open up the package manager.  
   Click in the menu bar Window → Package Manager
4. Select `Packages: My Registries` in the menu bar of the package manager window.
5. You should see Cecil Attributes under the `Hertzole` section. Click on it and then press Install in the bottom right corner.

### Unity package manager through git
1. Open up the Unity package manager
2. Click on the plus icon in the top left and "Add package from git url"
3. Paste in `https://github.com/Hertzole/cecil-attributes.git`  
   You can also paste in `https://github.com/Hertzole/cecil-attributes.git#develop` if you want the latest (but unstable!) changes.

Unity should now resolve the packages.

## 🎇 The Attributes

### Reset Static
**Applies to classes, fields, properties, and events**

Reset static will automatically reset your statics to their default value when the game starts. The default value can be either a value you've specified (the default assign value) or just the default value for that type. This is extremely useful for when you have fast enter play mode settings on without domain reload. Putting it on a class will reset all static fields/properties/events in that class.

Usage:  
```cs
[ResetStatic]
public static int testValue = 10;
```

### Log Called
**Applies to methods and properties**

Log called will automatically put a Debug.Log message in your methods/properties to see when they are called.  
In methods it will display all the parameters along with the name of the method.  
In properties it will display the property name, Get/Set and the value. If it's a get it will show the return value. If it's a set it will show the old value and the new value. You can also turn off logging for both get/set per attribute.

Usage:
```cs
[LogCalled(logPropertyGet: true, logPropertySet: true)]
public int MyProperty { get; set; }

[LogCalled]
public void MyMethod(int para1, string para2)
{
   // Will log "MyMethod (para1: <value>, para2: <value>)
}
```

### Find Property
**Applies to fields and properties**

Find property will automatically find serialized properties for you and make sure they exist. By default it searches with the same name as the field/property in your editor script, but a custom name/path can be specified to make it search with another name or within another serialized property.

Usage:  
```cs
[FindProperty]
private SerializedProperty myProperty; // Will look for a property called 'myProperty'.
[FindProperty("customName")]
private SerializedProperty notMyName; // Will look for a property called 'customName'.
[FindProperty("firstProperty/secondProperty")]
private SerializedProperty nested; // Will first look for a property called 'firstProperty' and then 'secondProperty' on the first property.
```

### Timed
**Applies to methods and properties**

Timed will automatically put your entire method inside a stopwatch and log at the end of the method how long it took to execute in both milliseconds and ticks.

Usage:  
```cs
[Timed]
private void MyMethod()
{
   // TODO: Code
   // Will log 'MyMethod took <ms> milliseconds (<ticks> ticks)'
}
```

### Mark In Profiler  
**Applies to methods**

Mark in profiler will put your method inside a big Profiler.BeginSample(<method name>) and Profiler.EndSample to make it show up in the Unity profiler.

Usage:  
```cs
[MarkInProfiler]
private void MyMethod() // This will show up in the Unity profiler.
{
   // TODO: Code
}
```

### Get Component
**Applies to serialized object reference fields**

Get component will automatically get your components for you in editor time on prefabs and scene objects. **It does not fetch them at runtime!** A target can be specified to get components in children or the parent.

Usage:  
```cs
[SerializeField]
[GetComponent]
private MyComponent myComponent; // Will call GetComponent(s)

[SerializeField]
[GetComponent(target = GetComponentTarget.Parent)]
private MyComponent myParentComponent; // Will call GetComponent(s)InParent

[SerializeField]
[GetComponent(target = GetComponentTarget.Children)]
private MyComponent myChildrenComponent; // Will call GetComponent(s)InChildren
```

![Editor icons](https://user-images.githubusercontent.com/5569364/199358582-d3ea55b6-3078-4d19-b9e4-f6d34ca54217.png)

### Required
**Applies to serialized object reference fields**

Required will inject a null check in your Awake method to make sure you've assigned all the required objects. If any of the objects are not met, it will not call the rest of your awake method.

It will also display a small icon in your inspector if it's required.
![Editor icons](https://user-images.githubusercontent.com/5569364/194349761-3933a29f-15c0-4852-a656-348f7f8ddbd8.png)

Usage:
```cs
[SerializeField]
[Required]
private Animator animator;
   
private void Awake()
{
    // Will not be called in 'animator' is null.
    Debug.Log("Awake");
}
```

## 📜 License
MIT - Basically do whatever, I'm just not liable if it causes any damages.
