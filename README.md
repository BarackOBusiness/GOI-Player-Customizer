# Player Customizer
A player cosmetic customization mod for Getting Over It with Bennett Foddy

## Installation
Firstly you need to install [BepInEx](https://github.com/BepInEx/BepInEx/releases), follow [these instructions](https://docs.bepinex.dev/articles/user_guide/installation/index.html) to install BepInEx for Getting Over It.

Once that is complete, head to the [releases](https://github.com/The-head-obamid/GOI-Player-Customizer/releases) and download the latest version of the plugin, and place this in the  `Getting Over It/BepInEx/plugins` folder, once you've done that, that's the installation done.

This plugin does build with some features for ingame customization, so if you'd like to utilize that, you can download [ConfigurationManager](https://github.com/BepInEx/BepInEx.ConfigurationManager) and click the releases and install it the same way you did this plugin, you can read the readme there for more information on how to make use of this

## Configuration
Running the game with the plugin installed will generate the necessary config files and folders for you to make use of the mod in `BepInEx/config`.

You can edit the `GOI.plugins.playerCustomizer.cfg` file in that folder as per your liking, there are 3 materials of choice for each component of the player, and you can select any custom path of choice for your images

## Building from source & development
All necessary libs should be fetched during a compilation of the project, to do so, use `dotnet build` with the -c parameter specifying either `B5` or `B6` for a BepInEx 5 or 6 build respectively.
