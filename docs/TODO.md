# To Do

## Code

 - Camera doesn't lower if player drops onto the key from above
 - Player model should bob up and down slightly when grounded
 - Separate PlayerController into multiple controllers (single responsibility!)
 - Change the "start lifetime" property of the Player exhaust based on acceleration; keep emission rate constant
 - Play a sound when collecting a Power Shard
 - Add unit tests
 - Get the player's collider bounds in a rotation-agnostic manner on init
 - Handle player's velocity vector using local coords, not world
   - i.e. convert at start of FixedUpdate, do calculations, and convert back at end
 - Strange inward slide behaviour if contacting inside of wing

## Unity

 - Sound effects
 - Music
 - Skybox
 - Configure launcher
 - Use third-party text effects
 - Display Power Shards in the HUD
    - Update counter as Power Shards are collected
    - Fix position (currently dependent on camera size)
        - Use a script to set the camera viewport dynamically?
    - Ensure UiController has access to this icon
    - Re-use (don't duplicate!) Power Shard components for Power Shard Icon
 - Fix crazy object positions within Scene

## Asset Enhancements

 - House (add wooden beams?)
 - Chimney (doesn't fit properly with the house)
 - Player (add more colours)
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
   - Including tunnel
 - Middle valley section containing road
   - Including tunnel
 - Underside to island

### Standalone

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
