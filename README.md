# [![Juka Programming Language](https://user-images.githubusercontent.com/11934545/192074923-24c8cfb9-12fc-48c4-8faa-2bbc4c187d83.png)](https://jukalang.com) Juka - Programming Language

Official Website: <https://jukalang.com>

[![Twitter](https://img.shields.io/twitter/follow/jukaLang.svg?style=social)](https://twitter.com/jukaLang)
[![Discord](https://img.shields.io/discord/975787212954275910)](https://discord.gg/MsKWsErzfp)
[![PyPI - Wheel](https://img.shields.io/pypi/wheel/juka-kernel)](https://pypi.org/project/juka-kernel/)
[![Nuget](https://img.shields.io/nuget/dt/JukaCompiler)](https://www.nuget.org/packages/JukaCompiler)

Build Status: [![Cirrus CI - Base Branch Build Status](https://img.shields.io/cirrus/github/jukaLang/Juka)](https://cirrus-ci.com/github/jukaLang/Juka)
[![Build status](https://ci.appveyor.com/api/projects/status/nmjmm04xhryx8p54?svg=true)](https://ci.appveyor.com/project/TheAndreiM/juka)

[![RunJuka](https://user-images.githubusercontent.com/11934545/212342916-bc9b61b9-59f2-46d1-b416-c51e1b83375a.png)](https://jukalang.com)

Main Git Repository: <https://github.com/jukaLang/juka>

Mirror Git Repository: <https://codeberg.org/JukaLang/Juka>

**Follow us on:**

[![Facebook](https://img.shields.io/badge/JukaLang-white?logo=Facebook)](https://www.facebook.com/jukaLang/)
[![Instagram](https://img.shields.io/badge/jukalanguage-white?logo=Instagram)](https://www.instagram.com/jukalanguage/)
[![Twitter](https://img.shields.io/badge/@jukaLang-white?logo=Twitter)](https://twitter.com/jukaLang/)
[![YouTube](https://img.shields.io/badge/@jukaLang-red?logo=YouTube)](https://www.youtube.com/@jukaLang)
[![Discord](https://img.shields.io/badge/jukaLang-white?logo=Discord)](https://discord.gg/MsKWsErzfp)

## 🤝 Donate

**Bitcoin Address:** 3MqJ2pwcuqh2W5mUPZUcKMVzxgTKcjD8ET

**Ethereum (Ethereum Network) Address:** 0xB56F6aff7a84935E5AF9D93b6d7db0e4F4F26B39

## 💭 Introduction

Juka's main goal is to be a portable, easy to use, universal programming language that can run on any platform including mobile devices, IOT, and cloud.

Juka's philosophy is code once, run everywhere.

**What are main advantages of Juka?**

- **Portable** - Juka doesn't need to be installed! That means you can run Juka in environments where you don't have admin priviledges or in environments where you don't have access to a hard drive. Juka is standalone and doesn't require any other application to be installed. Run it on your Flash/Thumb Drive!
- **Small in Size** - Juka is fairly small in size ~ 30 mb (depending on the OS). This means that you can put Juka on your Thumb/Flash Drive. In fact, why not put all versions of Juka on your Thumb/Flash Drive? It won't take much space anyways.
- **Works on "All" Operating Systems** - We are trying to support all systems including lesser known ones such as FreeBSD and TempleOS. We also support ARM processors and 32-bit systems such as Linux (x86). Why not install Juka on your Raspberry Pi or run it in ChromeOS Flex?
- **Universal** - Juka can be used for all types of projects including server side coding, client side coding, native coding, web development, API development, data science, network security, and quantum computing. Our goal is to create a programming language that can run on any platform and we mean any!
- **Cloud Friendly** - We created JukaAPI so that you can run it on any cloud server. This will allow one to use REST API to run code. Feel free to test it at <https://api.jukalang.com>. You can also run Juka on Microsoft's Azure Server by using Juka's Azure serverless function. Compile the code quickly, or let the others run the code. Download latest Azure Function from <https://jukalang.com/download>.
  NOTE: you need an active Microsoft Azure account to host.
- **Jupyter Support** - We know a lot of people love to use Jupyter Notebook/Lab. We've added an easy way to run Juka (kernel) in Jupyter. Do your data analysis in the software that you are used to!
- **Runs inside a Web Browser** - Run Juka inside a Web Browser! Visit <https://ide.jukalang.com> and install the App. On Mobile, it's simply adding the website to your home screen!
- **Easy to Use Package Manager** - All of the packages are hosted on GitHub. That's right! We made it simple to install and contribute to the package development. Our hope is that by hosting all packages on Github, security issues in the packages can be fixed quickly by the community. The packages contain a simple config file, making it simple for anyone to develop Juka packages.
- **Community Support** - Join our ever growing Juka community (<https://discord.gg/MsKWsErzfp>). We use "Discord" for managing the community as it provides an easy way to stay connected. In order to join the community, you should click our invitation link and register a Discord account. If you already have an account, just join the "JukaLang" group. It's public and anyone can join!
- **Contribute to the Source** - Juka is built by the community, for the community. Please consider contributing to the Juka GitHub repository at <https://github.com/jukaLang> When you create new features or modify the existing ones, please follow the guidelines specified in the project documentation.

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
- [Web Assembly (CDN/Browser/HTML server)](https://github.com/jukaLang/juka-webassembly)
- ChromeOS\* (see Web Assembly)
- [Jupyter via juka-kernel](https://github.com/jukaLang/juka-kernel)
- [Virtual Box Image (JukaVM)](https://github.com/jukaLang/jukaVM)
- [Windows App (Windows 10+)](https://github.com/jukaLang/JukaApp)
- Android App (Can be sideloaded <https://jukalang.com/download>, but not in PlayStore yet)
- Discord Bot  (Working, but is not currently hosted)
- Telegram Bot  (Working, but is not currently hosted)
- Amazon Fire (HD) (Can be sideloaded <https://jukalang.com/download>, but not in Amazon AppStore yet)

**Coming Soon:**

- Ethereum Network
- Android App (PlayStore version)
- iOS App (iPhone/iPad AppStore version)
- Tizen (Samsung)
- MacOS App (AppStore version)
- TempleOS
 
Send us a message or Tweet at us to let us know what other systems you want to see!

**Potential Future Support:**

- AndroidTV App (PlayStore version)
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

- Dynamic: <https://jukalang.com/docs>
- PDF: <https://github.com/jukaLang/juka-website/releases/download/JukaLang/jukadocs.pdf>

## 📦 Downloading Juka

Please download the latest version of Juka from <https://jukalang.com/download>

If you are an advanced user and wants to download other versions of Juka, visit <https://github.com/jukaLang/juka/releases>
and select appropriate version.
Find the version that you want and download it to your device.
The files should be self-contained (you are not required to download any other files)

## ⌛ Running Juka

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
