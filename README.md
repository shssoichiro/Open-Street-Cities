# Open Cities Map

_**Caveats:**_ _This project is in an early stage. The imported streets are more or less totally broken. You will need some time to fix everything. I will try to improve the import in future, but please, feel free to fork this project and fix stuff on your own. I would be happy to get pull requests!_

### Import real roads to Cities Skylines from Open Street Map.

1. Go to [skydark's map generator](https://heightmap.skydark.pl/), find the location you want.
2. Open the Info panel (top left corner, "i" icon). Click "Auto" to auto-populate the size and map scale.
3. Adjust to your preferences. Map size should be 17.28m to match the 81 tiles in Cities Skylines. Height Scale may be below 100% if you are in a mountainous area, due to in-game limitations--if it is, keep it that way. In flat areas it defaults to 250% to give greater emphasis to terrain, but I prefer 100% for better realism. I also like Water Depth = 8m.
4. Download either the RAW or the PNG height map using the icon on the sidebar.
5. Also download the OSM map using the sidebar.
6. From the unzipped folder, move the PNGs to height maps folder:
   - macOS: ~/Library/Application Support/Colossal Order/Cities_Skylines/Addons/MapEditor/Heightmaps
   - Linux: ~/.local/share/Colossal Order/Cities_Skylines/Addons/MapEditor/Heightmaps
   - Windows: C:\Users\USERNAME\AppData\Local\Colossal Order\Cities_Skylines\Addons\MapEditor\Heightmaps
7. Now start the game and make sure the Mod is active.
8. Open the map editor and start a new map.
9. In the map editor, open the height map import and select the height map that was exported. ![Height Map import](/pics/heightmap.png)
10. After the height map is imported, find the sea level tool and set it to whatever you set Water Depth to in the export.
11. Now click on the road button far right to bring up the OSM import dialog.
12. Provide the path to the downloaded OSM map file and click on `Load OSM From File` (this will take a while, be patient). ![Import File](/pics/load.png)
13. After the import is done, select the types of roads you want and click `Make Roads`. You can import one or a few types of roads at a time and import more afterwards if you like. This may take a long time for finer roads like residental. I recommend to wait additional 10-20 seconds, after the import is done.
14. Now, try to fulfill the requirements based on the real region to be able to start the map in a game. This is also the point where you should place all trees and make forests and so on to be as close to real life as possible.
15. After you are done, save the map and start a new game with the unlimited money mod enabled (or Game Anarchy with the money anarchy mode).
16. Finally, before you start the game, fix the broken roads.

##### Congrats. You are done.
