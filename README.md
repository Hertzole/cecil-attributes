# Cecil Attributes
### A set of Unity attributes to automate your workflow.

## Installation
1. Open up the Unity package manager
2. Click on the plus icon in the top left and "Add package from git url"
3. Paste in `https://github.com/Hertzole/cecil-attributes.git`

Unity should now resolve the packages.

## The Attributes

### Reset Static
Reset static will automatically reset your statics to their default value when the game starts. This is extremely useful for when you have fast enter play mode settings on without domain reload.

⚠ **Only works with simple types like bool, string, int, etc currently! This will be fixed soon.** ⚠

Usage:  
```cs
[ResetStatic]
public static int testValue = 10;
```

You must provide a default value that it will reset too!

## License
MIT - Basically do whatever, I'm just not liable if it causes any damages.
