# GDIM33 Vertical Slice [impact zone]

## Milestone 1 Devlog


1. Going into this, I thought Visual Scripting would be easier than writing everything in C#, but that didnt really end up being the case. Troubleshooting the graph was honestly more confusing than I expected, mostly because the errors arent as clear as they are in code. With C#, you usually get a pretty direct message about whats wrong, but with graphs it felt more like guessing sometimes. The graph I made runs every frame using an On Update event and checks the players current state from the PlayerController script. From there, it grabs the players renderer and uses a Select On Enum node tied to my PlayerState enum to decide what color the player should be. Each state has its own color blue for Idle, green for Move, red for Attack, and yellow for Dash. Using color instead of animations was a really practical decision at this stage. It let me clearly see when states were changing without having to deal with importing and setting up animations yet. It made the state machine a lot easier to understand while I was building and testing everything.

2. The biggest change I made to my breakdown since the pitch was adding the player state machine, which kind of became the core of how the Player system works. Like I mentioned in the other part Im using four states right now Idle, Move, Attack, and Dash and each one controls what the player can and cant do at that moment. Switching between them is based on input, so clicking Mouse1 while moving or idle puts you into Attack, pressing Space triggers Dash, and letting go of movement sends you back to Idle. Treating each state as its own behavior instead of just flipping booleans made things a lot easier to manage, especially for stuff like preventing a dash during an attack or making sure the hitbox only activates at the right time.

The Attack state is what triggers the hitbox, which is how enemies actually take damage, so that ties directly into the NPC system. The Dash state connects to the HUD with a cooldown indicator so the player can tell when its ready again, which links it to the UI. On top of that, enemies are always pathfinding toward the player using NavMesh, no matter what state the player is in, so how the player moves still affects how enemies react and reposition. I left environmental hazards out for now on purpose since I see them as the complicating factor for the vertical slice, and this milestone is really just about getting the core gameplay working first.

updated breakdown 
https://docs.google.com/drawings/d/1DXblBBuUiLAm_IhuKVVHFLm77iD_1kcDLqdRKewiVvY/edit?usp=sharing 

## Milestone 2 Devlog
So, I technically used the W5 activity to work on my proper complicating factor mentioned in my vertical slice document, and I scrambled for an idea that I should use Milestone 2 to work on. So I decided to work on a new 'push' mechanic that I didn't think would get into the game afterall. 

### Devlog Q1 
The complicating factor I'm building for Milestone 2 is a directional push mechanic bound to Mouse2. The push acts as a short range shove that launches enemies away from the player in the direction the reticle is aiming, mirroring how the standard attack works directionally. The intent is to give the player a tool to manage crowding, when multiple enemies are closing in from different angles, the push creates breathing room without dealing damage. It's designed to feel like a shorter, outward version of the dash, and it sets up the environmental hazards by giving the player a way to actively knock enemies into them.

1. Add the Push State and Input
    - Add a Push state to the PlayerState enum alongside Idle, Move, Attack and Dash
    - Bind the push action to Mouse2 in PlayerController
    - Define the push duration and a cooldown so it can't be spammed
    - Ensure the push state integrates cleanly into the existing state machine transitions

2. Implement the Directional Knockback on Enemies
    - Use the reticle's aim direction to determine the push direction
    - Use Physics or an impulse force to launch enemies in that direction on contact
    - Add a knockback method to EnemyController that receives a direction and force 
    - Tune the push distance to feel distinct from the dash but impactful enough to be useful

## Milestone 3 Devlog
Milestone 3 Devlog goes here.
## Milestone 4 Devlog
Milestone 4 Devlog goes here.
## Final Devlog
Final Devlog goes here.
## Open-source assets
- Cite any external assets used here!
