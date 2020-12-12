# Cecil Attributes
### A set of Unity attributes to automate your workflow.

## Installation
1. Open up the Unity package manager
2. Click on the plus icon in the top left and "Add package from git url"
3. Paste in `https://github.com/Hertzole/cecil-attributes.git`

Unity should now resolve the packages.

## The Attributes

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
	//TODO: Code
	// Will log 'MyMethod took <ms> milliseconds (<ticks> ticks)'
}
```

## License
MIT - Basically do whatever, I'm just not liable if it causes any damages.
