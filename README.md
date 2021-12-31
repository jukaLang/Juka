# Juka - Programming Language 

[![Build Status](https://app.travis-ci.com/jukaLang/Juka.svg?branch=master)](https://app.travis-ci.com/jukaLang/Juka)

Juka's goal is to be a universal programming language that can run on any platform.
Juka is being built on top of .NET network and runs on any platform that supports .NET.

Juka can be run as a function for Microsoft's Azure Cloud Server.
Juka can also compile programs into executables.

### Downloading Juka
Latest Juka version can be found at https://github.com/jukaLang/juka/releases

Juka's source can be downloaded at https://github.com/jukaLang/juka

## Running Juka
Juka can be run on a Microsoft Azure's cloud, 
or it can be run as a standalone application

## Contributing
- Create a new branch, work on it, and create a pull request once you are done working on the feature.


## Folder Structure:
### ./examples
- Provides you examples to get you started on using Juka

### ./src/JukaAzureFunction
- Azure Function runtime code. Used to run Juka Azure function on Microsoft's Azure Cloud Server

### ./src/JukaCompiler
- The core of the language. The code compiles into .NET .dll library that can be used in any C# projects including Xamarin/MAUI for building iOS/Android Apps, 
It is mainly used to build cross-platform apps for Mac/OS, Windows Apps, and Windows desktop applications.

### ./src/JukaUnitTest
- Unit tests to test JukaCompiler

### ./src/Juka
- Contains GUI (coming soon)

### Visual Studio/Development Requirements
##### Make sure you have the latest Visual Studio installed

The following Visual Studio packages required to run and develop Juka:

- Azure development
- .NET desktop development


### Running Development version of Juka's Azure function
- Open up Juka.sln
- Run DreamUnitTests using Test->Run->All Tests to make sure all tests are passed.
- Click "Start AzureJukaFunction" button which will run an Azure emulator locally.
- Use Postman to send functions to the Azure server in "body" as raw request in the following format:
```json
{
    "code": "function main() = {}"
}
```

