[logo]: https://raw.githubusercontent.com/Geeksltd/Zebble.Speech/master/Shared/NuGet/Icon.png "Zebble.Speech"


## Zebble.Speech

![logo]

A Zebble plugin To get the device operating system to read a piece of text out loud or recongnize the spoken words.


[![NuGet](https://img.shields.io/nuget/v/Zebble.Speech.svg?label=NuGet)](https://www.nuget.org/packages/Zebble.Speech/)

> With the user's permission, get recognition of live and prerecorded speech, and receive transcriptions. Also you'll be able to convert text to spoken language.

<br>


### Setup
* Available on NuGet: [https://www.nuget.org/packages/Zebble.Speech/](https://www.nuget.org/packages/Zebble.Speech/)
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
