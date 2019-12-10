# To Do

## Code

 - Add unit tests
 - Particle effects when picking up a Power Shard
 - NPE in LevelCompleteMenuController.Start()
 - NPE when collecting a Power Shard

### Turrets

 - Use custom logic rather than cron job to fire
   - This will let us cease fire if we are out of the turret's field of fire
     - E.g. below its minimum depression angle

### Player

 - Player model should bob up and down slightly when grounded
 - Separate PlayerController into multiple controllers (single responsibility!)
 - Change the "start lifetime" property of the Player exhaust based on acceleration; keep emission rate constant
 - Get the player's collider bounds in a rotation-agnostic manner on init
 - Handle player's velocity vector using local coords, not world
   - i.e. convert at start of FixedUpdate, do calculations, and convert back at end
 - Support multiple ground planes by using collisions instead of inferring from altitude
 - In flight mode, lateral "friction" should still be applied (currently momentum is not diminished)

### Camera

#### Flight Mode

 - Camera twitches strangely if player turns while airborne

#### Normal Mode

 - Camera clips through terrain when moving forward quickly and jumping onto a ledge
 - Camera freaks out when player jumps off the edge of the level
 - Camera can collide jankily with terrain if player drops off it and doesn't move far forwards
 - Strange camera behaviour when touching the key to end the level
   - Probably just needs to centre on the player at the moment of completion

## Unity

 - Sound effects
 - Music
 - Configure launcher
 - Custom cursor
 - Add distant islands / clouds within scene

## Asset Enhancements

 - House (add wooden beams?)
 - Chimney (doesn't fit properly with the house)
 - Village terrain piece (some janky geometry at edges)
 - Terrain pieces need bottoms!
 - Mountains (more jaggedy snow-line)
 - Mountain bottoms should not be visible
    - Fade to transparent
    - OR have a textured floor plane that cuts through them

## New Assets

### Player

 - Grabber (if this is first manipulator module)

### Large/Complex

 - Keep
   - Foundation/floor
   - Entrance hall staircase
   - Balcony
   - Chandelier
   - Walls/exterior
 - Portcullis
 - Dungeon
   - Cell bars and door
   - Dungeon staircase
 - Castle walls
   - Dormer / gable / addon (to attach to towers)

### Terrain

 - Underside to island
 - Staircase of platforms from valley to village

### Standalone

 - Tunnel segments
 - Tunnel arches
 - Torches (with light source)
 - Lanterns/standing torches
 - Crop furrows
 - Crops
 - Button
 - Flags
 - Power shards

### Mobs

 - Cat
 - Sheep
 - Cows
 - Chickens
 - Turrets
