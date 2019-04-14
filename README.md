# Dream Juka - Dream Programming Language 

[![Build Status](https://travis-ci.com/jukaLang/Juka.svg?branch=master)](https://travis-ci.com/jukaLang/Juka)

Dream Juka is the core component of Juka platform. It can compile programs into executables as well as compile itself into a .NET Standard .dll which can be used in other projects.

Juka library is used for running Juka server.

## Running Juka
Juka can be ran on a server. Check out https://github.com/jukaLang/juka

## Contributing

### src/DreamCompiler
- .NET Standard .dll library that can be used in any C# projects including Xamarin for building iOS/Android Apps, 
.NET Core for building cross platform apps for Mac/OS, Windows Apps, and Windows desktop applications.
It is currently used to compile our Xamarin Forms application.

### Examples
- Provides you examples to get you started on DReAM

### Visual Studio/Development Requirements
##### Make sure you have .NET installed in Visual Studio 2019

The following packages that are required:

- .NET Framework 4.6.1
- Latest version of NuGet
- Microsoft.CodeAnalysis
- Antlr4.Runtime.Standard (latest version downloaded via NuGet)
- MSTest.TestAdapter (latest version downloaded via NuGet)[Unit Test Only]
- MSTest.TestFramework (latest version downloaded via NuGet)[Unit Test Only]


### Running Development version
- Open up DReAM.sln
- Right click on solution and click "Restore NuGet Packages"
- (alternatively you can do this via console in src/ folder)
- Click "Start" button which will compile .dll and the main application.
- Run DreamUnitTests using Test->Run->All Tests
