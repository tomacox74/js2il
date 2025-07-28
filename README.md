
This is an experimental project that compiles JavaScript to .NET IL and creates a .NET assembly that can be executed.


This project is an experiment to see if the performance of .NET execution can come close to the many years of optimizations done in Node. There could be some usefulness in making JavaScript libraries available as native .NET assemblies for other .NET codebases to consume. But that is not the goal of this particular project, only a possible alternate use.

This project has 2 planned phases. The first phase is to implement enough functionality that most JavaScript libraries could be compiled to .NET without optimizations. One glaring exception is that eval support probably will not be implemented for a very long time. In this implementation, eval would mean "compile on the fly" to a .NET module basically.

The second phase would be to apply as many optimizations as possible to see how close the performance can come to Node or if it can exceed Node in any cases. These optimizations would come from both static compile-time analysis (i.e., a value that is always an integer or never a decimal does not need to be boxed nor stored as a float, closures not needed for simple functions, etc.) and runtime (things like shadow classes, etc.).

.NET provides a rich type system, cross-platform support, and an out-of-the-box GC implementation that has benefited from many years of optimizations.

## Performance notes
  - The generic implementation represents all locals as fields on a class to support closures. Analysis could eliminate unnecessary closures or add only the fields to closures that are needed by nested functions and arrow functions.
  - Values are always boxed as objects. Analysis could reveal when a variable is always an integer value; for example, it would be more optimal to always represent it as a simple integer in .NET.
  - Functions are invoked through delegates. Analysis could find places where functions could be invoked directly without the need for abstraction.


## Building


To compile the project locally, run:

```
dotnet build
```


For a release build:

```
dotnet publish -c Release
```


## Release pipeline


When a tag beginning with `v` is pushed, GitHub Actions runs `.github/workflows/release.yml` to build the solution in Release mode and upload the published files as an artifact.
