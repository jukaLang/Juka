# [![Juka Programming Language](https://user-images.githubusercontent.com/11934545/192074923-24c8cfb9-12fc-48c4-8faa-2bbc4c187d83.png)](https://jukalang.com) Juka - Programming Language

Official Website: https://jukalang.com

[![Twitter](https://img.shields.io/twitter/follow/jukaLang.svg?style=social)](https://twitter.com/jukaLang)
[![Discord](https://img.shields.io/discord/975787212954275910)](https://discord.gg/MsKWsErzfp)

PyPi: [![PyPI - Wheel](https://img.shields.io/pypi/wheel/juka-kernel)](https://pypi.org/project/juka-kernel/)

NuGet: [![Nuget](https://img.shields.io/nuget/dt/JukaCompiler)](https://www.nuget.org/packages/JukaCompiler)

CirrusCI (FreeBSD/Linux(x86)): [![Cirrus CI - Base Branch Build Status](https://img.shields.io/cirrus/github/jukaLang/Juka)](https://cirrus-ci.com/github/jukaLang/Juka)

Appveyor (Visual Studio Builds): [![Build status](https://ci.appveyor.com/api/projects/status/nmjmm04xhryx8p54?svg=true)](https://ci.appveyor.com/project/TheAndreiM/juka)

![RunJuka](https://user-images.githubusercontent.com/11934545/192074454-cbcf94d9-1f39-4198-991d-c4f941840395.gif)

## Introduction

Juka's main goal is to be a portable, easy to use, universal programming language that can run on any platform including mobile devices, IOT, and cloud.

Juka's philosophy is code once, run everywhere.

__What are main advantages of Juka?__
- Portable - Juka doesn't need to be installed. That means you can run Juka in environments where you don't have admin priviledges or in environments
- Small - Juka is fairly small in size ~ 30 mb (depending on the OS). This means you can put Juka on your thumb/flash drive. In fact, why not put all versions of Juka on your thumb/flash drive? It won't take much space anyways.
- API - Juka comes with it's own easy to use API. Why not dust off an old machine and run Juka on it? or put Juka in the cloud! We natively support Microsoft Azure's serverless functions.
- Universal - We are trying to support all systems including lesser known ones such as FreeBSD and TempleOS. We also support ARM processors and 32-bit systems such as Linux (x86). Why not install Juka on your Raspberry Pi?
- Jupyter Support - We know a lot of people love Jupyter. We've added an easy way to run Juka (kernel) in Jupyter. Create awesome 
- Runs inside browser/mobile - We've compiled Juka to web assembly and created a progressive app out of it. That means you can just go to https://wasm.jukalang.com and click "add to home screen" which will download and install a Juka version that you can run offline on your mobile device. You can also do it the same on your desktop computer for those that love Chrome/Chromium apps.
- Community Support - Our community is fairly small but we are all very friendly. We welcome everyone with the open arms. Please join our awesome discord channel!

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
- Android App (Can be sideloaded https://jukalang.com/download, but not in PlayStore yet)

**__Coming Soon:__**
- Android App (PlayStore version)
- iOS App (iPhone/iPad)
- Tizen (Samsung)
- MacOS App
- TempleOS
Send us a message or tweet at us to let us know what other systems you want to see!

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


### Microsoft/Linux/MacOS/FreeBSD

On some systems you can just double click Juka and it will launch an interactive terminal (REPL).

If that doesn't work, open a Terminal/Power Shell/Command Prompt and navigate to the location of where Juka is located.

Once you are inside the directory, run the following command to start Juka in REPL mode:
```jsx
./juka
```

If you have a file with Juka code, you can run it with the following command (substitute HelloWorld.juk with your filename):

```jsx
./juka HelloWorld.juk
```

### Microsoft Azure Function

Upload the package to Azure Web Server
Use web deploy to publish Juka (Azure Function) to the cloud.

### Including in C# Project

Once you install Juka via NuGet: Install-Package JukaCompiler

You can pass your code as a string to Juka:

```jsx
new JukaCompiler.Compiler().Go(codeAsString,isFile:false);
```

If you want to pass a filename instead of a string, you can run the following command
```jsx
new JukaCompiler.Compiler().Go(fileName,isFile:true);
```

We welcome any contribution! Thank you so much for checking out Juka!

