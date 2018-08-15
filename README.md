# Juliar DReAM Programming Language  [![Build status](https://ci.appveyor.com/api/projects/status/x8d6308cuevqm4of?svg=true)](https://ci.appveyor.com/project/TheAndreiM/dream)


##### Dream Big
##### мечтать больше
##### Sueña en grand
##### Sonhe grande
##### Rêvez gros
##### Traum groß
##### 더 크게 꿈꾸다
##### 梦想更大
##### ドリームビガー


![DReAM App](https://user-images.githubusercontent.com/11934545/44129730-db5e3e04-a017-11e8-968a-b83a82975f20.png)


DReAM: A C# port of Juliar Programming Language. 

DReAM focuses on fast and rapid prototyping as well as native .NET support.

DReAM uses Xamarin Forms to Cross-compile to iOS, Android, Windows 10, Windows 10 Mobile, XBOX, Hololens, Surface Hub, 
and other IoT.

 

## Download
<strike>Download the latest version at https://ci.appveyor.com/project/TheAndreiM/dream/build/artifacts</strike>

For now, Juliar should be compiled from source code.

## Folder Structure

### src/DreamCompiler
- .NET Standard .dll library that can be used in any C# projects including Xamarin for building iOS/Android Apps, 
.NET Core for building cross platform apps for Mac/OS, Windows Apps, and Windows desktop applications.
It is currently used to compile our Xamarin Forms application.

### src/DReAM
- A Xamarin Cross-Platform  application. This folder contains the main code for GUI for the app.

### Examples
- Provides you examples to get you started on DReAM

### Visual Studio/Development Requirements
#### Make sure you have XAMARIN/NMobile development with .NET installed in Visual Studio 2017

- Xamarin Forms
- Newtonsoft.JSON


- Desktop Tools  [LEGACY]
- .NET Framework 4.6.1
- Latest version of NuGet
- Antlr4.Runtime.Standard (latest version downloaded via NuGet)
- MSTest.TestAdapter (latest version downloaded via NuGet)[Unit Test Only]
- MSTest.TestFramework (latest version downloaded via NuGet)[Unit Test Only]


### Running Development version
- Open up DReAM.sln
- Right click on solution and click "Restore NuGet Packages"
- (alternatively you can do this via console in src/ folder)
- Set start up project. For Windows 7 select DReAM.Android, for Windows 10 select DReaM.UWP, for MacOS select DReAM.iOS,
for Linux select DReAM.Android. NOTE: Most new OSes can run all the projects. We recommend using Windows 10 as it can run
all three platforms.
- Click "Start" button which will compile .dll and the main application.