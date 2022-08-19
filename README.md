# [![Juka Programming Language](./logo.png)](https://jukalang.com) Juka - Programming Language

Official Website: https://jukalang.com

[![Twitter](https://img.shields.io/twitter/follow/jukaLang.svg?style=social)](https://twitter.com/jukaLang)
[![Discord](https://img.shields.io/discord/975787212954275910)](https://discord.gg/MsKWsErzfp)

PyPi: [![PyPI - Wheel](https://img.shields.io/pypi/wheel/juka-kernel)](https://pypi.org/project/juka-kernel/)

NuGet: [![Nuget](https://img.shields.io/nuget/dt/JukaCompiler)](https://www.nuget.org/packages/JukaCompiler)

CirrusCI (FreeBSD/Linux(x86)): [![Cirrus CI - Base Branch Build Status](https://img.shields.io/cirrus/github/jukaLang/Juka)](https://cirrus-ci.com/github/jukaLang/Juka)

Appveyor (Visual Studio Builds): [![Build status](https://ci.appveyor.com/api/projects/status/nmjmm04xhryx8p54?svg=true)](https://ci.appveyor.com/project/TheAndreiM/juka)

![RunJuka](https://user-images.githubusercontent.com/11934545/178172864-433ab5eb-5b76-4240-a98c-192c76f3b2f5.gif)

## Introduction

Juka's main goal is to be a universal programming language that can run on any platform including mobile devices.

Juka's philosophy is code once, run everywhere.

__Current Supported Device:__
- Windows (x86/x64)
- Windows ARM (ARM64)
- Linux (x86/x64) (CentOS, Debian, Fedora, Ubuntu, TinyCore, and almost any other type of Linux)
- Linux ARM (ARM64 and ARM32) (Linux Distributions that run ARM e.g. Raspberry Pi Model 2+)
- MacOS (macOS 10.12+)
- FreeBSD/UNIX (FreeBSD 11+)
- Azure (Microsoft Azure Function)
- HTTPS/API (JukaAPI server)
- Docker (JukaAPI)
- Unity Game Engine (via NuGet.org) 
- [Web Assembly (CDN/Browser/HTML server)](https://github.com/jukaLang/juka-webassembly)
- ChromeOS* (see Web Assembly)
- [Jupyter via juka-kernel](https://github.com/jukaLang/juka-kernel)
- [Virtual Box Image (JukaVM)](https://github.com/jukaLang/jukaVM)
- [Windows App (Windows 10+)](https://github.com/jukaLang/JukaApp)

__Coming Soon:__
- Android App
- iOS App (iPhone/iPad)
- MacOS App
- Tizen (Samsung)
- TempleOS

__Potential Future Support:__
- AndroidTV App
- Amazon Web Service (Native Support)
- Google Cloud (Native Support)
- Arduino

### Downloading Juka

Please download Juka from https://jukalang.com/download

If you would like to download other versions of Juka, you can download at https://github.com/jukaLang/juka/releases

Find the version that you want and download it to your device. 
The files should be self-contained (you are not required to download any other files)

## Juka API

Juka comes with an API

Download JukaAPI at https://jukalang.com/download

We are using JukaApi for visitors to test their code online: https://jukalang.com/tryonline

Special thanks to mogenius for hosting our API:
https://api.jukalang.com

If you would like to help improve the api, feel free to deploy a container at:
[![Develop on Okteto](https://okteto.com/develop-okteto.svg)](https://cloud.okteto.com/deploy?repository=https://github.com/jukalang/juka&branch=master)

## Running Juka

### Windows

Run the following command to start the Juka editor:

```jsx
./juka.exe
```

If you want to run Juka code from a file, run the following command (substitute HelloWorld.juk with your filename)

```jsx
./juka.exe HelloWorld.juk
```

### Linux/MacOS/FreeBSD

Run the following command to start the Juka editor:
```jsx
./juka
```

If you want to run Juka code from a file, run the following command (substitute HelloWorld.juk with your filename)

```jsx
./juka HelloWorld.juk
```


### Microsoft Azure Function

Upload the package to Azure Web Server
Use web deploy to publish Juka (Azure Function) on the cloud

### Including in C# Project

Once you install Juka via NuGet: Install-Package JukaCompiler

You can pass a string to Juka:

```jsx
new JukaCompiler.Compiler().Go(sourceAsString,isFile:false);
```

You can also pass a filename:
```jsx
new JukaCompiler.Compiler().Go(sourceAsString,isFile:true);
```


