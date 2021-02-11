## [0.4.1-preview] - 2021-02-11
### Changed
- Updated Mono.Cecil dependency.

## [0.4.0-preview] - 2021-01-12
### Added
- Timed attribute to automatically show how long a method or property takes to execute.
- MarkInProfiler attribute to make methods show up in the Unity profiler.
- Added %class_full%, %method_full%, and %property_full% for messages.

### Changed
- ResetStatic no longer needs default values.

### Fixed
- Fixed ResetStatic breaking if it's put on a generic field.
- Fixed code being weaved multiple times.

## [0.3.0-preview] - 2020-12-08
### Added
- FindProperty attribute to automatically find serialized properties in your editor scripts.

### Changed
- Moved settings asset into a Packages folder in ProjectSettings to be consistent with other packages.

### Fixed
- Fixed LogCalled not working on static methods.
- Fixed LogCalled failing due to index out of bounds exceptions.

## [0.2.0-preview] - 2020-11-04
### Added
- LogCalled attribute to make a log message appear when calling a method/property.
- Settings panel for changing settings related to the attributes.
- Support for not including generated code from ResetStatic in build.
- You can now change the initialization type of the generated ResetStatic method.

### Fixed
- Fixed attributes not working in the editor.

## [0.1.1-preview] - 2020-11-03
### Added 
- Support for using reset static with classes to reset all static members.

### Fixed
- Fixed reset static saying it could work on structs.
- Fixed attributes needing to be on MonoBehaviours.
- Fixed reset static not working with debug mode code.
- Fixed reset static not working with anything else than simple primitive types.

## [0.1.0-preview] - 2020-11-01
### Added
- Reset static attribute
