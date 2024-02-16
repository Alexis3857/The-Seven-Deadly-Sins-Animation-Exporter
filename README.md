# The Seven Deadly Sins Animation Exporter

Program to export animated models of The Seven Deadly Sins Grand Cross


## Usage

Install and update the game and write the path to the **Game\7dsgc_Data\master\bm\m\\** folder in **directory.txt**.

Use [7dsgcDatamine](https://github.com/Alexis3857/The-Seven-Deadly-Sins-Datamine) for your version of the game and replace **BundleData.bytes** with **game\version\Bmdata\m_BundleData.bytes**.

Run the program and it will ask you to enter a character name body number and head number, example :
```
Enter the name of the character you're looking for : merlin_delver
Enter the body number : 1
Enter the head number : 1
```
The character name list can be created with the function WriteAllCharacterName in the Program class.

The animations will be available in the output folder.


## Build

Get a copy of [AssetStudio](https://github.com/Perfare/AssetStudio) source code, add the projects AssetStudio, AssetStudio.PInvoke, AssetStudioFBXNative, AssetStudioFBXWrapper, AssetStudioUtility, Texture2DDecoderNative, Texture2DDecoderWrapper to the solution and compile.

Alternatively you can download the [compiled version of AssetStudio](https://github.com/Perfare/AssetStudio/releases/tag/v0.16.47) and add the dlls as reference.