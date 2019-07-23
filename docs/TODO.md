# To Do

## Code

 - Camera doesn't lower if player drops onto the key from above
 - Camera clips through walls (camera_collision branch)
 - Player model should bob up and down slightly when grounded
 - Separate PlayerController into multiple controllers (single responsibility!)
 - Change the "start lifetime" property of the Player exhaust based on acceleration; keep emission rate constant
 - Add unit tests
 - Get the player's collider bounds in a rotation-agnostic manner on init
 - Handle player's velocity vector using local coords, not world
   - i.e. convert at start of FixedUpdate, do calculations, and convert back at end
 - Stop player being able to turn or accelerate once they've fallen a certain distance

## Unity

 - Sound effects
 - Music
 - Skybox
 - Configure launcher

## Asset Enhancements

 - House (add wooden beams?)
 - Chimney (doesn't fit properly with the house)
 - Village terrain piece (some janky geometry at edges)
 - Well (can't fall down easily!)

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
 - Gatehouse
   - Portcullis
 - Dungeon
   - Cell bars and door
   - Dungeon staircase
 - Castle walls
   - Dormer / gable / addon (to attach to towers)

### Terrain

 - Farm plateau
 - Hill for windmill
 - Lower section for cabin
 - Plateau for castle
 - Underside to island

### Standalone

 - Tunnel segments
 - Tunnel arches
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
