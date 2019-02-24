# Juka DReAM Programming Language 


<table>
    <tr>
        <td>Dream Big</td>
        <td rowspan="9"><img src="https://user-images.githubusercontent.com/11934545/44129730-db5e3e04-a017-11e8-968a-b83a82975f20.png" width="400"></td>
    </tr>
    <tr>
      <td>мечтать больше</td>
    </tr>
    <tr>
      <td>Sueña en grand</td>
    </tr>
    <tr>
        <td>Sonhe grande</td>
    </tr>
    <tr>
      <td>Rêvez gros</td>
    </tr>
    <tr>
      <td>Traum groß</td>
    </tr>
    <tr>
        <td>더 크게 꿈꾸다</td>
    </tr>
    <tr>
      <td>梦想更大</td>
    </tr>
    <tr>
      <td>ドリームビガー</td>
    </tr>
  <tr>
</tr>
</table>

DReAM: A C# port of Juka Programming Language. 

DReAM focuses on fast and rapid prototyping as well as native .NET support.

DReAM uses Xamarin Forms to Cross-compile to iOS, Android, Windows 10, Windows 10 Mobile, XBOX, Hololens, Surface Hub, 
and other IoT devices.

 

## Download
Juka DReAM should be compiled directly from source code.

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
##### Make sure you have XAMARIN/NMobile development with .NET installed in Visual Studio 2017

- Xamarin Forms
- Newtonsoft.JSON
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
