## Overview
Welcome to the CommerceHub .NET Interview exercise.  Within this directory you will find a Visual Studio solution, projects, and build utilities.  
Targeting .NET Core 2.2, you can use Visual Studio 2017 (or greater) as well as Visual Studio Code to develop your implementations.

### Build
This project uses the Cake build system. To build in Windows Powershell:

```powershell
X:\Working > .\build.ps1
```

**NOTE:** It is important to run the build this way as it performs a targeted restore of vendored NuGet packages located at `./src/.packages`.


### The Problems
Under `./src/Exercise` you will find the individual problem definitions in their respective `readme.md` files.

An empty xUnit project has been provided at `./src/Exercise.Tests`.  You may use this to author any that that you might feel are desirable or necessary.

### Your Submission
Once you are satisfied with your implementations, please submit the `Submission_{DateTime}.zip` file that's created under the `./BuildArtifacts` folder as part of the build script.  If that file fails to be created, simply create a zip archive of the working directory, less the `BuildArtifacts`, `tools`, and any `bin` or `obj` output.
