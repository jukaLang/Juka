# Juka - Programming Language 

Official Website: https://jukalang.com

[![Twitter](https://img.shields.io/twitter/follow/jukaLang.svg?style=social)](https://twitter.com/jukaLang)
[![Discord](https://img.shields.io/discord/975787212954275910)](https://discord.gg/MsKWsErzfp)

Nuget: ![Nuget](https://img.shields.io/nuget/dt/JukaCompiler)
Jupyter: ![PyPI - Wheel](https://img.shields.io/pypi/wheel/juka_kernel)

Travis (FreeBSD): [![Build Status](https://app.travis-ci.com/jukaLang/Juka.svg?branch=master)](https://app.travis-ci.com/jukaLang/Juka)

Appveyor (Visual Studio Builds): [![Build status](https://ci.appveyor.com/api/projects/status/nmjmm04xhryx8p54?svg=true)](https://ci.appveyor.com/project/TheAndreiM/juka)

![RunJuka](https://user-images.githubusercontent.com/11934545/178172864-433ab5eb-5b76-4240-a98c-192c76f3b2f5.gif)

## Introduction

Juka's main goal is to be a universal programming language that can run on any platform including mobile devices.

Juka's philosophy is code once, run everywhere.

__Current Supported Device:__
- Windows (x86/x64)
- Windows ARM (ARM64)
- Linux (CentOS, Debian, Fedora, Ubuntu and derivatives)
- Linux ARM (ARM64 and ARM32) (Linux Distributions that run ARM e.g. Raspberry Pi Model 2+)
- MacOS (macOS 10.12+)
- FreeBSD/UNIX (FreeBSD 11+)
- Azure (Microsoft Azure Function)
- HTTPS/API (JukaAPI server)
- Docker (JukaAPI)
- [Web Assembly (CDN/Browser/HTML server)](https://github.com/jukaLang/juka-webassembly)
- [Jupyter via juka_kernel](https://github.com/jukaLang/juka_kernel)

__Coming Soon:__
- Linux 32-bit
- Virtual Box Image
- Android App
- AndroidTV App
- iOS App (iPhone/iPad)
- MacOS App
- Windows App (Windows 10+)
- Tizen (Samsung)


__Potential Future Support:__
- AWS (Amazon Web Service)
- Google Cloud

### Downloading Juka

Please download Juka from https://jukalang.com/download

If you would like to download other versions of Juka, you can download at https://github.com/jukaLang/juka/releases

Find the version that you want and download it to your device. 
The files should be self-contained (you are not required to download any other files)

## Juka API

Juka comes with an API

Deploy a container:
[![Develop on Okteto](https://okteto.com/develop-okteto.svg)](https://cloud.okteto.com/deploy?repository=https://github.com/jukalang/juka&branch=master)

Special thanks to mogenius for hosting our API:
https://jukaapi-prod-juka-5ufe4u.mo1.mogenius.io/


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


