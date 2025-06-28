This is a experimental project that compiles javascript to dotnet il and creates a dotnet assembly that can be executed

The ultimate purpose is to see how close performance can be to the v8 engine used by node though it is understand that optimizations such has shadow classes will have to addded to be at least compariable in performance features.

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
