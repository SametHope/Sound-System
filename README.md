# Sound System

 Versatile, efficient and simple audio system for Unity. 
 
## Features

- **No Configuration Required:** You don't need to do any configuration before using it, just include the files and you are ready to go.
- **Pooling:** Both relevant gameobjects and coroutines are recycled when needed without destruction or initialization.
- **Custom Component(s):** Ever wondered why Unity's Audio Source component only has the option to play sound on Awake? Yeah me too.
- **Versatile:** With a single Sound scriptable object, configure audio clips, pitch range, cooldown, volume, pooling and more. 
- **Mixer Group Support:** Unlike most audio solutions, Sound System supports both Unity's mixer groups and custom volume and pitch settings.
- **Contained:** With its own assembly definition and pooling system, it doesn't clutter your project or increase compile times.
- **Fully Documented:** All code you can access is documented clearly and accurately.
- **Custom Component Icons:** They are pretty cool.
- **Open Source:** The code is open source.

## Setup

1. Get the scripts inside your project, ideally to the `Assets/Plugins/SametHope/SoundSystem` folder.
2. Done.

## Code Usage

```csharp
// Play sounds with Sound scriptable objects (recommended)
SoundMaster.Play(SomeSound);

// Play sounds with Sound scriptable objects at position
SoundMaster.Play(SomeSound, transform.position);

// Play sound with its name
SoundMaster.Play("My Sound Scriptable Object");

// Play sound with its name at position
SoundMaster.Play("My Sound Scriptable Object", transform.position);

// Cache the audio source
AudioSource source = SoundMaster.Play(SomeSound);

// Return a new Audio Source without playing or pooling it
AudioSource mySpecial = SoundMaster.GetSpecial(SomeSound);

// Return a new Audio Source with its name without playing or pooling it
AudioSource myOtherSpecial = SoundMaster.GetSpecial("My Special Sound");

// Stop all playing sounds
SoundMaster.Instance.ActiveSounds.ForEach(tuple => tuple.source.Stop());

// Filter active sounds
SoundMaster.Instance.ActiveSounds.First(soundDataTuple => soundDataTuple.sound.name == "Ambiance").source.Stop();
```
---
## Component Usage

![2023-12-21 15_25_49-Window](https://github.com/SametHope/Sound-System/assets/85421686/398223e2-4e10-435f-a963-7fe57ea6517d)


![2023-12-21 15_24_57-Window](https://github.com/SametHope/Sound-System/assets/85421686/837c9862-26a7-48c9-9b82-59b261c2714c)

## Sound Scriptable Object

![2023-12-21 15_42_28-Window](https://github.com/SametHope/Sound-System/assets/85421686/e8ff8eb9-818b-4059-ab65-83fd8bf17cbd)

![2023-12-21 15_25_25-Window](https://github.com/SametHope/Sound-System/assets/85421686/98aea982-9eac-4928-a3a4-977399c8796c)

## Automatic Pooling

![2023-12-21 15_26_38-Window](https://github.com/SametHope/Sound-System/assets/85421686/a8f7a782-a101-4867-831a-21d4393ae733)
