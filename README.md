# [![Juka Programming Language](https://user-images.githubusercontent.com/11934545/192074923-24c8cfb9-12fc-48c4-8faa-2bbc4c187d83.png)](https://jukalang.com) Juka - Programming Language

Official Website: <https://jukalang.com>

Welcome to the official repository of Juka, a portable, easy-to-use, and universal programming language that can run on any platform. Our philosophy is "code once, run everywhere."

[![Twitter](https://img.shields.io/twitter/follow/jukaLang.svg?style=social)](https://twitter.com/jukaLang)
[![Discord](https://img.shields.io/discord/975787212954275910)](https://discord.gg/MsKWsErzfp)
[![PyPI - Wheel](https://img.shields.io/pypi/wheel/juka-kernel)](https://pypi.org/project/juka-kernel/)
[![Nuget](https://img.shields.io/nuget/dt/JukaCompiler)](https://www.nuget.org/packages/JukaCompiler)

Build Status: [![Cirrus CI - Base Branch Build Status](https://img.shields.io/cirrus/github/jukaLang/Juka)](https://cirrus-ci.com/github/jukaLang/Juka)
[![Build status](https://ci.appveyor.com/api/projects/status/nmjmm04xhryx8p54?svg=true)](https://ci.appveyor.com/project/TheAndreiM/juka)

[![RunJuka](https://github.com/jukaLang/Juka/assets/11934545/fff8c2c3-e89a-4262-99c0-acc98d3ac6a2)](https://jukalang.com)

## 🌐 Quick Links

- **Official Website:** [jukalang.com](http://jukalang.com)
- **Main Git Repository:** [github.com/jukaLang/juka](https://github.com/jukaLang/juka)
- **Mirror Git Repository:** [codeberg.org/JukaLang/Juka](https://codeberg.org/JukaLang/Juka)

## 📣 Follow Us On

[![Facebook](https://img.shields.io/badge/JukaLang-white?logo=Facebook)](https://www.facebook.com/jukaLang/)
[![Instagram](https://img.shields.io/badge/jukalanguage-white?logo=Instagram)](https://www.instagram.com/jukalanguage/)
[![Twitter](https://img.shields.io/badge/@jukaLang-white?logo=Twitter)](https://twitter.com/jukaLang/)
[![YouTube](https://img.shields.io/badge/@jukaLang-red?logo=YouTube)](https://www.youtube.com/@jukaLang)
[![Discord](https://img.shields.io/badge/jukaLang-white?logo=Discord)](https://discord.gg/MsKWsErzfp)

## 🤝 Support Us

We accept donations via Bitcoin and Ethereum:

- **Bitcoin Address:**
  3MqJ2pwcuqh2W5mUPZUcKMVzxgTKcjD8ET

- **Ethereum (Ethereum Network) Address:**
  0xB56F6aff7a84935E5AF9D93b6d7db0e4F4F26B39

## 💭 Introduction

Juka is a portable, easy-to-use, universal programming language that can run on any platform, including mobile devices, IoT devices, and the cloud. Its philosophy is "code once, run everywhere."

## 💭 About Juka

Juka is a general-purpose language designed to be used in a wide variety of tasks. It's cloud-native, has native support for Jupyter Notebooks, can be run in a web browser, and comes with an easy-to-use package manager. Juka is a community-driven project with a growing community of users and developers.

## 💻 Supported Systems

**Current Supported Device:**

- Windows (x86/x64)
- Windows ARM (ARM64)
- Linux (x86/x64) (CentOS, Debian, Fedora, Ubuntu, TinyCore, and almost any other type of Linux)
- Linux ARM (ARM64 and ARM32) (Linux Distributions that run ARM e.g. Raspberry Pi Model 2+)
- MacOS (macOS 10.12+)
- FreeBSD/UNIX (FreeBSD 11+)
- Azure (Microsoft Azure Function)
- HTTPS/API (JukaAPI server)
- Docker (JukaAPI)
- Unity Game Engine ([via NuGet.org](https://www.nuget.org/packages/JukaCompiler))
- [Web Assembly (CDN/Browser/HTML server/AndroidTV)](https://github.com/jukaLang/juka-webassembly)
- ChromeOS\* (see Web Assembly)
- [Jupyter via juka-kernel](https://github.com/jukaLang/juka-kernel)
- [Virtual Box Image (JukaVM)](https://github.com/jukaLang/jukaVM)
- [Windows App (Windows 10+)](https://github.com/jukaLang/JukaApp)
- Android App (Can be sideloaded <https://jukalang.com/download>, but not in PlayStore yet)
- Discord Bot (Working, but is not currently hosted)
- Telegram Bot (Working, but is not currently hosted)
- Amazon Fire (HD) (Can be sideloaded <https://jukalang.com/download>, but not in Amazon AppStore yet)

**Coming Soon:**

- Ethereum Network
- Android App (Google Play version)
- iOS App (iPhone/iPad AppStore version)
- Tizen (Samsung)
- MacOS App (AppStore version)
- TempleOS

Send us a message or Tweet at us to let us know what other systems you want to see!

**Potential Future Support:**

- AndroidTV App (Google Play version)
- Amazon Web Service (Native Support)
- Google Cloud (Native Support)
- Arduino

## 📜 To Do

- [x] Print/PrintLine
- [x] Variable Declaration
- [x] Basic Operations
- [x] Functions
- [x] Classes
- [x] Loops
  - [x] While Statement
  - [x] For Statement
- [x] Native Functions
  - [x] GetAvailableMemory
  - [x] SystemClock
- [x] csharp() command (execute C# code)
- [ ] Tail Recursion
- [ ] Array
- [ ] Dynamic List
- [ ] Get (for importing files)
- [ ] Try and Catch

## 📚 Documentation

You can find our dynamic documentation at jukalang.com/docs <https://jukalang.com/docs>.
We also provide a PDF version of our documentation which can be downloaded at <https://github.com/jukaLang/juka-website/releases/download/JukaLang/jukadocs.pdf>

## 📦 Downloading Juka

Please download the latest version of Juka from our official website: <https://jukalang.com/download>

If you are an advanced user and want to download other versions of Juka, visit our releases page <https://github.com/jukaLang/juka/releases>

## ⌛ Running Juka

Juka can be run in various ways depending on your system. Detailed instructions can be found in our documentation.

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

### Juka API

Juka comes with an API

Download JukaAPI at <https://jukalang.com/download>

We are using JukaApi for visitors to test their code online: <https://jukalang.com/tryonline>

Special thanks to mogenius for hosting our API:
<https://api.jukalang.com>

If you would like to help improve the api, feel free to deploy a container at:

[![Develop on Okteto](https://okteto.com/develop-okteto.svg)](https://cloud.okteto.com/deploy?repository=https://github.com/jukalang/juka&branch=master)

### Microsoft Azure Function

Upload the package to Azure Web Server
Use web deploy to publish Juka (Azure Function) to the cloud.

### Including in C# Project

Once you install Juka via NuGet: Install-Package JukaCompiler

You can pass your code as a string to Juka:

```jsx
new JukaCompiler.Compiler().Go(codeAsString, (isFile: false));
```

If you want to pass a filename instead of a string, you can run the following command

```jsx
new JukaCompiler.Compiler().Go(fileName, (isFile: true));
```

We welcome any contribution! Thank you so much for checking out Juka!
