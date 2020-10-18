
# Setting up a development environment

This document goes over how to set up the development
environment to start building ROMs. It does not go
into finer details. If you require help installing
any of the various tools the respective websites should
have help available.

---

## Table of Contents

- [On Shells](#on-shells)
- [Web Gui](#build-roms-from-the-web-gui)
  - [Setting up the GUI](#setting-up-the-gui)
- [Command Line](#build-roms-from-the-command-line)
  - [Setting up the Command Line](#setting-up-the-cli)
- [Testing](#testing)

---

## On Shells

For the below commands that are intended to be run on command line
it is assumed that PowerShell will be used on Windows and bash on
either mac or linux.

In many cases (such as git) there are alternatives to running
from the command line. This is not intended to be an exhaustive
listing of all possible methods to get through this. Any
functionally equivalent command is fine.

## Build ROMs from the Web GUI

This is the canonical form of the application and how it's most often used.
When doing development, it's necessary that any changes be tested via
the web interface.

### Setting up the GUI

1. Clone the SMZ3 Repository
    - If you plan on contributing, make a fork and clone the fork instead
    ```bash
    # git clone https://github.com/tewtal/SMZ3Randomizer
    # git submodule init
    # git submodule update
    ```

2. Install [dotnet 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

3. Install the [Entity Framework](https://docs.microsoft.com/en-us/ef/core/miscellaneous/cli/dotnet)
    ```bash
    # dotnet tool install --global dotnet-ef
    # dotnet ef
    ```
    - If `dotnet ef` doesn't work, you may need to run `dotnet tool restore`

3. Install [VSCode](https://code.visualstudio.com/download)

4. Install [node.js](https://nodejs.org)
    - Depending on version you may have to run:
        - `npm audit fix`

5. Have VSCode install dependencies for you
    - open the WebRandomizer solution (`WebRandomizer.sln` file)
    - say "yes" when VSCode asks if you want to install / restore

6. Install [Postgres](https://www.postgresql.org/download)
    - On Windows, after the installation the "Stack Builder" will open
        - install npgsql for dotnet
        - this may not be required

7. Initialize postgres database
    - You can find the database/users to create
      at [SMZ3Randomizer/WebRandomizer/appsettings.json](https://github.com/tewtal/SMZ3Randomizer/blob/master/WebRandomizer/appsettings.json)
        - Feel free to change these, just don't check in the changes!
    - `psql -U postgres`
        - opens up postgres to do some adminitration
        - you may have to ``createdb `whoami` `` depending on how you installed postgres
    - In the psql command line you just opened
    ```bash
    create database webrandomizer11;
    create user webrandomizer with password '<redacted>';
    grant all privileges on database webrandomizer11 to webrandomizer;
    ```
    - In one instance it was necessary to also:
        - `alter user webrandomizer createdb`
        - unclear why this wasn't available from "grant all"
    - Warning: it's currrently unclear based on checked in code, but WebGameService uses the randomizer database
      and WebRandomizer uses the randomizer11 database.
        - In theory they should both use the same database.

8. Use Entity Framework tools to initialize the database
    ```bash
    cd WebRandomizer
    dotnet ef database update
    ```

9. Open the WebRandomizer Project in VSCode (as in step 5 above) and select Run->Start Debugging (or hit F5)
    - It will automatically open a web browser to your localhost:5001
    - Click on "Generate randomized game" and follow instructions
    - Enjoy!


---


## Build ROMs from the Command Line

Building ROMs from the command line can aid in development, but do not conflate
command line builds with production builds. The production code runs via web interface.

Make sure to test your changes in the gui!

### Setting up the CLI

1. Clone the SMZ3 Repository
    ```bash
    # git clone https://github.com/tewtal/SMZ3Randomizer
    # git submodule init
    # git submodule update
    ```

2. Install [dotnet 3.1](https://dotnet.microsoft.com/download/dotnet-core/3.1)

3. Install [python3](https://www.python.org/downloads/)

4. Download [asar](https://github.com/RPGHacker/asar/releases)
    - Make sure the binary is in your path somewhere

5. Build the IPS file
    - `cd alttp_sm_combo_randomizer_rom`
    - on Windows:
    `build.bat`
    - on mac or linux:
    `sh build.sh`

    - copy the resultant resources/zsm.ips file somewhere useful

6. Get a copy of the SM and Z3 ROMs
    - Sorry... can't help you here
    - Use your favorite search engine for help

7. Prepare the project and do a test build
    ```bash
    cd Randomizer.CLI
    dotnet restore
    dotnet build
    ```

8. Build the ROM
    - `dotnet run smz3 --rom --ips <ips file>`
        - You can run `dotnet run help smz3` by itself to see all cli options
    - This will create the rom in the local directory
    - Enjoy!


---


## Testing

For now testing is manual. Create seeds and check the resultant files
for validity.
