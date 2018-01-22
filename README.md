[logo]: https://raw.githubusercontent.com/Geeksltd/{Plugin.Name}/master/Shared/NuGet/Icon.png "{Plugin.Name}"


## {Zebble.Speech}

![logo]

To get the device operating system to read a piece of text out loud in Zebble apps.


[![NuGet](https://img.shields.io/nuget/v/{Plugin.Name}.svg?label=NuGet)](https://www.nuget.org/packages/{Plugin.Name}/)

> Speech make you able to read piece of text in lots of language by accessing to operation system ability and setting some setting to control volume and speed of reading of machine.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/{Plugin.Name}/](https://www.nuget.org/packages/{Plugin.Name}/)
* Install in your platform client projects.
* Available for iOS, Android and UWP.
<br>


### Api Usage
Call Zebble.Device.Speech from any project to gain access to APIs.

##### To read the text:
```csharp
await Device.Speech.Speak("Hello world!");
```
##### To stop reading:
```csharp
Device.Speech.Stop();
```
##### Get available languages:
```csharp
var languages = Device.SpeechLanguage.GetInstalledLanguages();
```
##### Set some setting:
```csharp
var settings = new Device.SpeechSettings
{
    Pitch = 1.2, 
    Speed = 1.5,
    Volume = 0.8,
    Language = new Device.SpeechLanguage("fr")
};

await Device.Speech.Speak("Hello world!", settings);
```

<br>

### Methods
| Method       | Return Type  | Parameters                          | Android | iOS | Windows |
| :----------- | :----------- | :-----------                        | :------ | :-- | :------ |
| Speak        | Task         | text -> string<br> setting -> SpeechSettings| x       | x   | x       |
| Stop         | void         | -                                   | x     | x   | x
