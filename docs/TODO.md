# To Do

## Code

 - For jumping, check if ANY point under the player is grounded, instead of using the average?
 - Player model should bob up and down slightly when grounded
 - Separate PlayerController into multiple controllers (single responsibility!)
 - Change the "start lifetime" property of the Player exhaust based on acceleration; keep emission rate constant
 - Add unit tests
 - Get the player's collider bounds in a rotation-agnostic manner on init
 - Handle player's velocity vector using local coords, not world
   - i.e. convert at start of FixedUpdate, do calculations, and convert back at end
 - Particle effects when picking up a Power Shard
 - NPE in LevelCompleteMenuController.Start()
 - NPE when collecting a Power Shard

## Unity

 - Sound effects
 - Music
 - Configure launcher
 - Custom cursor
 - Add mountains to Skybox
 - Add distant islands / clouds within scene

## Asset Enhancements

 - Castle plateau (jagged at edges)
 - House (add wooden beams?)
 - Chimney (doesn't fit properly with the house)
 - Village terrain piece (some janky geometry at edges)

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
