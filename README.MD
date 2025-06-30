![Dino Attack Logo](./Assets/LlamAcademy/Dinos/UI/Textures/Dinos%20Logo.png "Dino Attack Logo")

Dino Attack is a free and open source reverse-tower-defense microgame aimed at helping you learn different concepts in Unity 6. 
The full compiled game is available on [Itch](https://llamacademy.itch.io/dino-attack) for free.
Video Tutorials on how to make different aspects of the game are available on the [LlamAcademy YouTube Channel](https://youtube.com/@llamacademy).

Art Assets gracefully provided by [polyperfect](https://polyperfect.com). See below for an important licensing notice.

## Important Licensing Notice
The **code** is licensed under the MIT license. However, the **ART** assets (all provided by [polyperfect](https://www.polyperfect.com/)) are available under an **educational use only** license.
This means that you **may not redistribute these art assets** and **may not use them in any project that is not strictly for educational purposes**.
See [the full license details](./Assets/LlamAcademy/Dinos/Vendor/polyperfect/LICENSE) before proceeding.

If you would like to use these art assets in your own game, consider using the [LlamAcademy Asset Store Affiliate URL](https://assetstore.unity.com/publishers/19123?aid=1101l9QvC) to do your purchase.

## About the Game
Humans have stolen the dinosaur eggs and it's your job to help get them back! This is a reverse tower defense, meaning you spawn the units in, and the AI tries to defend the eggs by building units and upgrading their building defenses.

## Quickstart
1. Clone the project
2. Open the project in Unity 6 or later (6.2 beta was used for development)
3. Open `/Assets/LlamAcademy/Dinos/Scenes/Dino Test 1.unity`
4. Click play!

## Challenges

See [Challenges](./CHALLENGES.MD) for ways to expand your knowledge.

## Slow Start

### Project Structure
Under `/Assets/LlamAcademy/Dinos` you'll find the main project code and assets.

```
|- Animations - contains primarily Unity 6.2 AI generated humanoid animations for the enemy AI.
|- Armor Types - contains ScriptableObjects that are ArmorTypes, used to determine which attacks should be used on which enemies.
|- Enemy - contains the main Enemy scripts.
|- Behavior - contains scripts to be used in the Unity Behavior system. These are bootstraped from Unity Behavior's "Create ..." system.
|- Config - contains core ScriptableObject definitions.
|- Dinos - contains assets related to the Dinos, including animation controllers, scriptable objects, Behavior Trees, and prefabs. The core models remain in the Vendor/polyperfect/... folders.
|- Editor - contains editor scripts
|- Enemy - contains scripts mostly related to "enemy" units and main AI.
|- Enemy AI - contains assets related to the main enemy AI and defending units and walls, including animation controllers, scriptable objects, and prefabs.
|- Materials - contains common materials, some that ended up not being used. 
|- Models - contains primarily combined meshes to lower scene file size.
|- Player - contains scripts focused on player / player interaction.
|- Prefabs - contains prefabs used in multiple areas.
|- Resource SOs - contains Resource ScriptableObjects.
|- RoundManagement - contains scripts related to how the rounds progress and some helpers.
|- Scenes - contains the only scene.
|- Shaders - contains Shader Graphs that were either used in development or fully upon release. Play with some of them for a different look.
|- Textures - contains common textures, some were cut from the final release
|- UI - contains scripts, UXML, and USS related to the UI.
|- Unit - contains scripts used by units.
|- Utility - contains utility scripts.
|- Vendor - contains 3rd party assets. Double check the licenses in these folders because most of them have an EDUCATIONAL USE ONLY license, meaning you can't use them.
```
### Adding Features
If you'd like to add features (maybe based on the [challenges](./CHALLENGES.MD)), a few things to take note of:
1. The way walls/buildings are placed assume all upgrades have the exact same size and the buildings are just placed where the previous wall was.
2. Walls are not destroyed on death, only disabled so we can "remember" the proper position & rotation. Everything else is destroyed on death.
3. Enemy AI waypoints are manually placed around the buildings.

Make sure to review either the [YouTube Playlist](https://www.youtube.com/playlist?list=PLllNmP7eq6TRC1QtErorKKXUOtlxGEsem) (new videos are still being released) and/or the existing demo scene to see how all the parts connect.

### Creating New Scenes
In addition to the above notes, make sure you have in your new scene:
1. Dino Spawner & connect references
2. Round Manager & connect references
3. Dino Visualizer & connect references
4. Enemy AI Controller & connect references
5. NavMeshManager & connect NavMeshSurfaces.
6. HealthBarCanvas (this is using UGUI, the rest of the game is on UITK)
7. Runtime UI

The game does not currently easily support dynamic levels, because the enemy AI just repairs/upgrades existing buildings, so once you've placed all the buildings to start, that should be the extent of what can be destroyed.

## Support This Project & LlamAcademy
There are many ways to show your support for LlamAcademy and polyperfect. The easiest way to support both of us is to purchase the art assets using the following affiliate URLs: [Dinosaurs]((https://assetstore.unity.com/publishers/19123?aid=1101l9QvC)) & [Ultimate Pack](https://assetstore.unity.com/publishers/19123?aid=1101l9QvC).

You can also check out the rest of [polyperfect's assets here](https://assetstore.unity.com/publishers/19123?aid=1101l9QvC) (also affiliate link).

You can directly show your support for this project by making a one-time donation here on [GitHub](https://github.com/sponsors/llamacademy?frequency=one-time) or the [Dino Attack Itch Page](https://llamacademy.itch.io/dino-attack)

If you got value from this tutorial microgame, consider a donation on Itch, or becoming a [Patreon Supporter](https://patreon.com/llamacademy), joining as a [YouTube Member](https://www.youtube.com/channel/UCnWm6pMD38R1E2vCAByGb6w/join), or even become a [GitHub Sponsor](https://github.com/sponsors/llamacademy) to show your support.

Of course if you love the art assets as well, pick up a copy using the [LlamAcademy Asset Store Affiliate URL](https://assetstore.unity.com/publishers/19123?aid=1101l9QvC) to help support polyperfect and LlamAcademy.

Supporters during the active lifespan of the microgame tutorial creation are shown below. Thank you all 🧡!

## Phenomenal Supporters
* **YOUR NAME** HERE!

## Tremendous Supporters
* **YOUR NAME** HERE!

### Awesome Supporter TierRuntimeUI
* Ivan
* Iffy Obelus
* Jason Hansen
* Snedden
* Trey Briggs
* Will B.
* Mustafa
* Nick 5454
* Pixel Wizards
* **YOUR NAME** HERE!

### Supporters
* AudemKay
* Matt Sponholz
* Tarik
* Elijah Singer
* Bruno Bozic
* Christiaan Van Steenwijk
* Strange Swelter
* Warspan
* Mike_H
* Meryl Stenhouse
* Morvister
* Anstudio
* Wendy Whitner
* Wing Lo
* **YOUR NAME** HERE!

## Other Projects
Interested in other Topics in Unity?

* [Check out the LlamAcademy YouTube Channel](https://youtube.com/c/LlamAcademy)!
* [Check out the LlamAcademy GitHub for more projects](https://github.com/llamacademy)

## Requirements
* Unity 6.0 or later. Built with 6.2 beta.
* Universal Render Pipeline
