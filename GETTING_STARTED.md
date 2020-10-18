
## Getting Started with the Web GUI:

1. Clone the SMZ3 Repository
  - git clone https://github.com/kedNalatacId/SMZ3Randomizer
  - git submodule init
  - git submodule update
    - Make sure to pull in the alttp_sm_combo_randomizer_rom submodule as it will be needed later

2. Install dotnet 3.1
  - https://dotnet.microsoft.com/download/dotnet-core/3.1
  - From command line:
    - dotnet tool install --global dotnet-ef
    - run "dotnet ef"
      - If it doesn't work, you may need to run "dotnet tool restore"

3. Install VSCode
  - https://code.visualstudio.com/download

4. Install node.js
  - https://nodejs.org
  - Depending on version you may have to run:
    - npm audit
    - npm audit fix

5. Have VSCode install dependencies for you
  - open the WebRandomizer csproj
  - say "yes" when VSCode asks if you want to install / restore

6. Install Postgres
  - On a mac:
    - brew install postgres
      - brew services start postgresql
    - or the postgress.app

7. Initialize postgres database (from command line)
  - createdb \`whoami\`
    - this is needed to access psql to begin with
  - psql
    - open postgres to do some adminitration
    - You can find the database/users to create
      at SMZ3Randomizer/WebRandomizer/appsettings.json
    - Feel free to change these, just don't check
      in the changes!
  - create database webrandomizer11;
  - create user webrandomizer with password <redacted>;
  - grant all privileges on database webrandomizer11 to webrandomizer;

8. Use dotnet ef tool to initialize database instance
  - cd into the WebRandomizer directory
  - dotnet ef database update InitializeDatabase

9. Open the WebRandomizer.csproj in VSCode and select Run->Start Debugging (or hit F5)
  - It will automatically open a web browser to your localhost:5001
  - Click on "Generate randomized game" and follow instructions
  - Enjoy!


## Getting Started with CLI:

1. Clone the SMZ3 Repository
  - git clone https://github.com/kedNalatacId/SMZ3Randomizer
  - git submodule init
  - git submodule update
    - Make sure to pull in the alttp_sm_combo_randomizer_rom submodule as it will be needed later

2. Install dotnet 3.1 for mac
  - https://dotnet.microsoft.com/download/dotnet-core/3.1

3. Install python3
  - brew install python3 (is one option)

4. Build asar
  - git clone https://github.com/RPGHacker/asar
  - cd asar
  - cmake src && make
    - you may have to "brew install cmake" or install XCode CLI tools
  - either make a symlink from the asar binary into the alttp_sm_combo_randomizer_rom
    directory, or edit the build.sh and remove the "./" from "./asar"

5. Build the IPS file
  - cd into alttp_sm_combo_randomizer_rom
  - sh build.sh
  - copy the resultant resources/zsm.ips file somewhere useful (or use it where it is)

6. Get a copy of the SM and Z3 ROMs
  - Sorry... can't help you here
  - Use your favorite search engine for help

7. cd into Randomizer.CLI
  - run "dotnet restore" to restore the package files
    - runs NuGet under the hood (which is what the error message will mention)
  - run "dotnet build" to build the project
    - as a test, not necessary

8. dotnet run smz3 --rom <options>
  - will create the rom in the local directory
  - Enjoy!
