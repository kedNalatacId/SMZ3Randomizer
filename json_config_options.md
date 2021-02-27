
## Some relevant SMZ3 and SMR options

You can copy the sample_config.json file and change the parameters in order to get started.
There are a LOT of options. And more are coming.


### Metroid-Specific Parameters

These options will be inside a "Metroid" JSON block. See the sample_config.json
as an example.

- smFile
    - Required for any seed
    - Location of Super Metroid ROM on disk (doesn't have to be local dir)
- SMLogic
    - Optional
    - Ignored when mystery mode is set
    - Values are NO LONGER "Casual" and "Tournament"
    - Possible values are the same in both SMZ3 and SMR:
        - Normal
            - Mostly the same as stock SMZ3 (https://samus.link)
            - Has been slightly tuned down, removing some logic that's slightly
                harder for newer players.
            - "CanFly" is usually replaced with SpaceJump only (removing IBJ)
        - Medium
            - A step between Normal and Hard, makes it possible to tune down Normal
            - Introduces "Heck Run" which is 7 E-tanks instead of 5
            - IBJ used more as well as various other harder options
        - Hard
            - The exact same as stock SMZ3 hard mode
- Keycards
    - Optional
    - Ignored when mystery mode is set
    - Any option other than "None" or "Keysanity" require AutoIPS to be on
    - Possible values in both SMZ3 and SMR:
        - None
            - Keycards are off
        - Exist
            - Keycards are in a relative "vanilla" location and will generally not impede your progress too much
        - Keysanity
            - Keycards can be anywhere
        - Randomized
            - Randomly Chooses different parameters for each of keycards 1, 2, and boss
            - Valid Locations are:
                - Home -- valid in SMZ3 and SMR; keycards appear in their "vanilla" location
                - Local -- only valid in SMZ3; keycards are only in Metroid
                - Away -- only valid in SMZ3; keycards are only in Zelda
                - Anywhere -- valid in SMZ3 and SMR; keycards appear anywhere
            - Example SMR values:
                - `{ one => home, two => anywhere, boss => anywhere }`
            - Example SMZ3 values:
                - `{ one => away, two => local, boss => home }`
        - Mystery
             - The option chosen is any of the above
    - Possible values in SMZ3 only:
        - WithinWorld
            - Keycards will only be in Metroid
        - Outsiders
            - Keycards will only be in Zelda
        - AlienOverlords
            - Keycards 1 and 2 will only be in Metroid (anywhere), boss keys will only be in Zelda
        - Mystery
            - The option chosen will be any of the SMZ3 or SMR options
- MorphLocation
    - Optional
    - Ignored when mystery mode is set
    - Possible values:
        - Original -- Morph Ball in original location; works in both SMZ3 and SMR
        - Early -- Only works in SMZ3, Morph Ball will be in "sphere one"
        - Randomized -- Morph Ball can be anywhere
- Placement
    - Optional
    - Ignored when mystery mode is set
    - (Currently) only works in SMR (not in SMZ3)
    - TODO: add to SMZ3
    - Possible values:
        - Split -- Major items can only be in major locations, minor items can be anywhere
        - Full -- both major and minor items can be anywhere


### Zelda-Specific Parameters

These options will be inside a "Zelda" JSON block. See the sample_config.json
as an example.

- z3File
    - Required for any seed
    - Location of Zelda ROM on disk (doesn't have to be local dir)
- Z3Logic
    - Optional
    - Ignored when mystery mode is set
    - Possible values:
        - Normal
            - Mostly the same as stock SMZ3 (https://samus.link)
        - Medium
            - This is basically "Advanced Item Placement" from ALTTPR
            - dark rooms, fake flippers, etc
        - Hard
            - Not much here, a few harder dark rooms, a couple other minor things
- KeyShuffle
    - Optional
    - Ignored when mystery mode is set
    - Any option other than "None" or "Keysanity" require AutoIPS to be on
    - Possible values:
        - None
            - all dungeon items are in their respecitve dungeons
        - Keysanity
            - dungeon items can be anywhere
        - Randomized
            - Randomly Chooses different parameters for each dungeon item (maps, compasses, small and big keys)
            - Valid Locations are:
                - Home -- dungeon items appear in their "vanilla" location
                - Local -- dungeon items are only in Zelda
                - Away -- dungeon items are only in Metroid
                - Anywhere -- dungeon items  appear anywhere
            - Example SMZ3 values:
                - `{ maps => anywhere, compasses => local, small => away, big => home }
        - WithinWorld
            - Dungeon Items will only be in Zelda
        - Outsiders
            - maps/compasses will be anywhere, but small/big keys will only be in Metroid
        - AlienOverlords
            - maps, compasses, and Small Keys will only be in Zelda (anywhere), big keys will only be in Metroid
        - Mystery
             - The option chosen is any of the above
- BottleContents
    - Optional
    - Ignored when mystery mode is set
    - Possible values:
        - Empty
        - Randomized
- BossDrops
    - Optional
    - Ignored when mystery mode is set
    - Possible values:
        - NonDungeon -- Bosses can only drop non-dungeon items
        - Randomized -- Bosses can drop anything
- RandomFlyingTles
    - Optional
    - Ignored when mystery mode is set
    - Boolean
    - When true, sets the number of flying tiles in flying tile rooms
        to a random number between 1 and 21.
    - Will typically shorten flying tile rooms
- Z3HeartColor
    - Optional
    - Ignored when SurpriseMe mode is set
    - A way to set Z3HeartColor from command line (since it's usually only available from web)
    - Possible values:
        - blue, green, red, yellow
- SwordLocation
    - Optional
    - Ignored when mystery mode is set
    - Possible values:
        - Uncle -- First Sword will be on Uncle
        - Early -- First Sword will be in Sphere one
        - Randomzied -- First Sword can be anywhere
- ProgressiveBow
    - Optional
    - Ignored when mystery mode is set
    - Boolean


### Generic Parameters (can be used in both games)

Only inside the generic JSON block.

- PlayerName
    - Optional
    - Well... this is really only used in SMZ3
    - Sets the save file name and "Link's House" name to PlayerName
- SMControls
    - Optional
    - Sets the Super Metroid controls before starting the game
    - stringified JSON dictionary (see sample)
        - The sample in the file is ridiculous on purpose
    - Allowed controls:
        - Dash, Shot, Jump, ItemCancel, ItemSelect, AngleUp, AngleDown
    - Allowed Buttons:
        - A, B, X, Y, L, R, Select, None
- Race
    - Optional
    - Boolean
    - Default is false
    - Race seeds cannot have playthroughs/spoilers generated
- Players
    - Optional
    - Default is 1
    - Untested for any number other than 1 at this time (eventually)
- Multi
    - Optional
    - Boolean
    - Whether it's a multi-world seed or not (more than one player)
- MysterySeed
    - Optional
    - Sets various options "at random"
    - Uses the global seed for any important choices, so they should be the same
        choices for multi-world (in theory)
- OutputFile
    - Optional
    - By default the output file will be in the current directory
    - If this is set to a directory (with or without a trailing slash) it
        will set the filename to default inside that directory
    - If this includes a partial filename after the directory, it will use the given portion plus the seed number as the filename
- Playthrough
    - Optional
    - Whether to generate a playthrough (shows where progression items are)
    - Default is false
- Spoiler
    - Optional
    - Mutually exclusive with Playthrough (includes playthrough as part of output)
    - Whether to generate a spoiler (shows the playthrough, the given optiopns, the parsed config, and where /all/ items are)
    - Default is false
- SurpriseMe
    - Turns on and off various cosmetic items for fun
    - Requires AutoIPS to be on
    - Warning: currently includes a couple of race-affecting items
        - such as quickswap or whether the G4 statue cutscene plays or not
    - Randomized sprites, heart color, etc
- GoFast
    - Optional
    - Ignored when Mystery mode is on
    - Boolean
    - In SMZ3: Puts boots and speed booster in sphere one
    - In SMR: as yet undone, on the todo list (maybe will force starting with speed booster?)
- LiveDangerously
    - Optional
    - Ignored when Mystery mode is on
    - Boolean
    - In SMZ3:
        - removes the forced small key in Skull Woods Pinball Room
            - This is a possible soft-lock, but you can save/quit out, so it only costs time
        - removes the forced super/power drops in sphere one
            - You may have to go through Metroid with only 5 PBs
            - The same softlocks are available as with 10 or 15 PBs
    - In SMR:
        - removes a forced super/power drop in sphere one


### IPS options

Currently inside the generic JSON block.

- IPS
    - Optional
    - A list of IPS files to apply
    - One of either IPS or AutoIPS must be specified to generate a seed
        - this is enforced

The below options are used with AutoIPS.
It is recommended to use AutoIPS with this codebase (it relies on it).

- AsarBin
    - Location of asar executable
    - no need to compile by hand, just have it available
- PythonBin
    - Location of python3 executable
    - On windows this tends to just be the default "python" (no need to set it)
    - On mac this tends to be "python3" ($PATH will expand for you)
- AutoIPSPath
    - Location of alttp_sm_combo_randomizer_rom checked out on disk
- AutoIPSCOnfig
    - JSON config file used with alttp_sm_combo_randomizer_rom
    - If using AsarBin above, this is no longer necessary
    - more useful when NOT using the SurpriseMe mode (to set quickswap, heartcolor, etc)


### Sprite options

Currently inside the generic JSON block.

- Sprites
    - Optional
    - List of Sprites to apply (full path) or list of search terms to match (unique string)
    - Ignored if "SurpriseMe" mode is set

The below options are used with SurpriseMe.
Some of them may become used as part of specifying sprites above in the future.

- SpriteURL
    - Location of JSON sprite inventory list of SMZ3 approved sprites
    - Fetched once per day, automatically, then cached
- SpriteCachePath
    - Where to store both the cached sprite inventory list as well as any sprites that are downloaded and converted
- SpriteSomethingBin
    - As part of downloading the sprites, they're not in a game-friendly format and require conversion. SpriteSomething converts the sprites into a game-friendly form.
- AvoidSprites
    - In case there are sprites you don't want to use, you can list them here
    - No sprites are ignored by default, however you may want to ignore some sprites for your purposes. In that case, peruse what's available at the SpriteURL and add anything to this list you wish to avoid.
    - Regular expressions are used to make this easier, and this list applies to both Zelda and Metroid. An example list is included in sample_config.json

