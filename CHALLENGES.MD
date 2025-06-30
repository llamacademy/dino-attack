# Challenges

Since this is an education-oriented repository, challenge your understanding of different concepts by completing some of the below tasks.

Please **do not** open pull requests into this repository with your solution to the challenges. Feel free to fork the repository and implement it there. Make note of the different licenses for the art in this project.

If you have suggestions for some challenges, feel free to open a PR with those!

## Easier
- Add a new basic dinosaur.
- Add a new basic enemy defender.
- Add a Main Menu where the player can select their enemy AI mode (difficulty & type).
- Add sound effects to the enemy units & dinosaurs. Consider both attacking & general sounds.
- Add round win & lose sound effects.
- Play some fancy special effects when the player wins or loses.
- Add the ability to rotate / zoom the camera.
- Object Pool Units & Dinos.

## Harder
- Migrate to Unity Animation Events to properly synchronize the dinos & units attacking animations and projectile spawning.
- Optimize NavMesh rebaking when a building is destroyed ([hint](https://www.youtube.com/watch?v=RuoK7w1OIT0&ref=github.com)).
- Add a flying dinosaur ([hint](https://www.youtube.com/watch?v=cN837GYgxUI)).
- Create a new level to play on.
- Show the likely path a dinosaur will take if it's placed at the player's current cursor (ignore any repathing/targeting changes due to AI behavior).
- Add a destruction animation for walls instead of just immediately destroying them. Make sure to synchronize the deaths of units on top!
- When a dinosaur is on top of a wall that is destroyed, have them gracefully be moved to the ground or nearby valid NavMesh position (or die).

## More Advanced
- Add the ability for the enemy AI to upgrade their units' damage and/or attack speed.
- Add the ability for the _player_ to upgrade their dinos (maybe the Velociraptor can be upgraded to jump up?).
- Create a dynamically sized safe zone around the enemy AI's buildings. Make sure it still renders appropriately.
- Create attacking towers for the enemy AI to use
- Create an adaptive Main Enemy AI that considers how the player is playing and updates its play style (consider a Utility AI system).
