
BE WARNED, this is a "pet" project... it's probably not what you want.

You still have time to turn back, you're probably looking for:
https://github.com/tewtal/SMZ3Randomizer

-----

The canonical method of running this version is from command line.
cd into Randomizer.CLI and run:

`dotnet run smz3 --config <config file>`  
`dotnet run sm --config <config file>`

You can look at json_config_options.md for a break of relevant options.
You can look at sample_config.json for a reasonable default config that you
will need to modify in order to put in your own details.

You can use the same config for both SMR and SMZ3!

In order to make a Super Metroid seed you WILL NEED the Zelda ROM available. The
Super Metroid Randomizer "double stacks" the ROM image, the same way SMZ3 does
(and uses the same alttp_sm_combo_randomizer_rom assembly on the backend).


If you want to use AutoIPS option (required for SMR with keycards and 
some SMZ3 KeyShuffle/Keycard variants) then you'll need:
* alttp_sm_combo_randomizer_rom checked out, and both repos will need to have the "personal" branches checked out.
* python3
* asar


If you want to use the SurpriseMe option (which is fun) then you'll need:
* python3
* SpriteSomething checked out locally
* A local directory to cache the sprite inventory and downloaded sprites


-----

A number of changes have occurred between Tewtal's original version and here.
These are presented in no particular order.

- "Medium" Metroid logic
    - easier than "Hard" Metroid logic
    - some logic from "Normal" Metroid has been removed
        - such as speed booster to break the blocks out of initial area
    - introduces "Heck Run" which is 8 E-tanks instead of 5
        - for those of us who aren't very efficient at movement
- "Medium" Zelda logic
    - more akin to "advanced item placement" from ALTTPR
    - includes dark rooms and fake flippers (+ a little)
        - also includes reductions in TR laser bridge items (no safety, lamp not required, etc)
        - ice breaker
        - etc
    - likely to be removed or changed when zelda logic comes in
- "Hard" Zelda logic
    - more or less unimpleneted; likely to be removed
- MysterySeed mode
    - randomizes difficulty of logic
    - randomizes morph and sword location (original, early, or random)
        - 5% chance each of original or early; 90% chance of random
    - randomizes bottle content (random or empty; usually random)
        - 20% chance of "Empty"; 80% chance of "Randomized"
    - randomizes gofast or not (low chance, see below)
        - 10% chance
    - randomizes to live dangerously or not (high chance; see below)
        - 70% chance
    - randomizes which level of keyshuffle/keycard to use (see below)
    - randomizes number of flying tiles (see below)
        - 80% chance
    - randomizes boss drops between "non-dungeon" and "random"
        - 80% chance of "Randomized"
    - randomized chance of progressive bow
        - currently disabled; hopefully soon
        - not sure what percentages will be yet
    - reserved for future:
        - randomizes goal
        - randomizes ganon invincible
- BossDrops can be either random or non-dungeon
- Random Bottle content is implemented
    - also: bottles are moved behind the first progressive bottle
        - less chance of alternate pool items showing up early
- "GoFast" mode
    - puts boots and speed booster early (sphere zero)
    - for when you want a shorter seed
    - ~10% chance in mystery mode (rarely on)
- "LiveDangerously" option
    - removes forced skull wood small key in pinball room
    - removes forced super missile / power bomb drops in sphere zero
- randomized number of flying tiles is available
    - anywhere between 1 and 22
- KeyShuffle changes
    - new keyshuffle modes
        - WithinWorld (keys only spawn within Zelda)
        - Outsiders (keys only spawn outside Zelda)
        - AlienOverlords (small keys are anywhere in Zelda, Big Keys are in Metroid)
        - Randomized:
            - each dungeon item (map, compass, small, big) gets its own random chance between:
                - "home" (item in near-vanilla location)
                - "local" (item in same game)
                - "away" (item in opposite game)
                - "anywhere" (anywhere)
    - mystery mode chooses randomized about half of the time
- Keycard changes
    - new Keycard modes
        - Exist (keycards are in near-vanilla locations)
        - WithinWorld (keycards only spawn within Metroid)
        - Outsiders (keycards only spawn in Zelda)
        - AlienOverloads (keycards 1 & 2 spawn in Metroid; boss keycards spawn in zelda)
        - randomized:
            - each keycard type (1, 2, and boss) gets its own random chance between:
                - "home" (item in near-vanilla location)
                - "local" (item in same game)
                - "away" (item in opposite game)
                - "anywhere" (anywhere)
    - mystery mode chooses randomized about half of the time
        - WIP: "None" will be an option for randomized mode at some point
- progressive bow is... sort of working
    - when progressive bow is chosen, it'll only show up in zelda
    - WIP; will work on getting it working in SM soon... i hope?
- Add AutoIPS mode, which is necessary for mystery mode
    - compiles the IPS on the fly
    - WIP: doesn't currently work with --loop
    - Must also set AutoIPSPath when setting AutoIPS
        - the location of alttp_sm_combo_randomizer_rom as checked out
    - recommend using alttp_sm_combo_randomizer_rom repo with build.py functionality
    - can set AutoIPSConfig to tell the IPS generator which config file to use
- Changed --loop to take an integer argument
    - zero or negative is "infinite"
- Can now use a JSON file for configuration, makes creating seeds easier/faster
    - see example.json for a list of available options
- Surprise Me mode
    - chooses random sprites
    - sends --surpriseme when using --autoips
- Added Spoiler to CLI
    - Added Spoiler as First Class object (part of Randomizer.SMZ3)
    - includes options as passed and config as parsed
        - that way the randomization for mystery can be seen
    - includes the Playthrough
    - shows all items in the randomizer (progression or not)
        - useful for debugging
- Added some argument validation
    - Choosing IPS or RDC files that don't exist will throw errors
    - Trying to run without either IPS or AutoIPS will fail
    - etc
- Changed option "Console" to "ToConsole" that allows "-t" or "--terminal" as shortcuts
    - "Console" is also a C# Library
    - while it didn't overlap, didn't like the conflation and easy to change
- Input and Output files as options
    - smFile and z3File can be set to anywhere
    - OutputFile places the file anywhere
    - rather than forcing local dir
- some items moved out of progression
    - shields, half magic, bugnet, and spazer
    - may need revisiting, but generally "progression" items should open up new locations
    - anything that doesn't open up a new location has been removed from progression
- changed out two reserve tanks for two e-tanks in progression items
    - generally i think newer players would rather have an e-tank than a reserve tank
- removed all but one missile, super, and two power bombs from progression
- removed G4 requirement from opening Ganon's Tower
    - this means some Metroid bosses might be behind Agahnim 3
    - but Agahnim 3 is (currently) required anyway
- Added PlayerName option which gets used by the Zelda File Select screen
    - WIP: currently only first 4 characters
- Added custom "CannotFillWorldException" that gets thrown when the world cannot be filled
    - this happens when the logic gets particularly bent, like "Outsiders" that pushes all zelda
        keys to metroid...
    - gets triggered when the number of attempts to place an item goes over the number of available locations
        - technically if the number of items is co-prime to the number of locations this would be items * locations
            however this works as a first-order approximation.
    - For now when this gets triggered we just retry without counting. Tends to work within 2-3 retries.
- Stole various PRs from others
    - including ComposeFilename, which gets used when OutputFile isn't set
    - more to come (various logic fixes, etc)


-----

Further changes that have been added:

- Super Metroid Randomizer with keycards available
    - This uses the SMZ3 codebase and assembly
    - The ROM is "double-stacked", just like SMZ3 (most have both ROM images available
        to create an SMR seed)
    - This means that certain blocks in red tower and cathedral have NOT been replaced
        and soft-locking in those places IS an issue
    - For NOW (hopefully to be fixed) Zebes starts awake, not sleeping. Sorry :(
    - The logic here is still very much under review and not yet nailed down;
        seeds will occasionally fail to create. Hopefully this will be more stable
        in the future.
- Sprites are downloaded and applied "just in time"
    - SpriteSomething is used to convert .jpg and .zspr files to .rdc's and cached
    - the cached files are then applied
    - future work (TODO): allowing specifying a sprite by name and it will get pulled from
        online, converted, cached, and then applied
    - sprite authors are injected into the credits when using "SurpriseMe"
    - sprites in "SurpriseMe" mode are only allowed to be "approved for SMZ3" sprites
        - Please don't use sprites without the author's permission
