# Juliar DReAM

##### Dream Big
##### мечтать больше
##### Sueña en grand
##### Sonhe grande
##### Rêvez gros
##### Traum groß
##### 더 크게 꿈꾸다
##### 梦想更大
##### ドリームビガー


![DReAM App](https://user-images.githubusercontent.com/11934545/39957654-6c169638-55c4-11e8-9300-62264743c6ce.png)

[![Build status](https://ci.appveyor.com/api/projects/status/x8d6308cuevqm4of?svg=true)](https://ci.appveyor.com/project/TheAndreiM/dream)

DReAM: A new C# port of Juliar Programming Language. DReAM focuses on fast and rapid prototyping as well as native .NET support.

## Download
Download the latest version at https://ci.appveyor.com/project/TheAndreiM/dream/build/artifacts

## Folder Structure

### src/DreamCompiler
- .NET Standard .dll library that can be used in any C# projects including Xamarin for building iOS/Android Apps, .NET Core for building cross platform apps for Mac/OS, Windows Apps, and Windows desktop applications.

### src/DReAM
- A Windows GUI application that you can use to build and run DReAM/Juliar applications.

### Examples
- Provides you examples to get you started on DReAM

### Visual Studio/Development Requirements

- Desktop Tools
- .NET Framework 4.6.1
- Latest version of NuGet
- Antlr4.Runtime.Standard (latest version downloaded via NuGet)
- MSTest.TestAdapter (latest version downloaded via NuGet)[Unit Test Only]
- MSTest.TestFramework (latest version downloaded via NuGet)[Unit Test Only]


### Running Development version
- Open up DReAM.sln
- Right click on solution and click "Restore NuGet Packages"
- (alternatively you can do this via console in src/ folder)
- Click "Start" button which will compile .dll and the main application (Make sure the DReAM application is selected).