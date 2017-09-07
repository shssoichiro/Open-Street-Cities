# Open Cities Map

*__Caveats:__* *This project is in an early stage. The imported streets are more ore less totally broken. You will need some time to fix everything. I will try to improve the import in future, but please, feel free to fork this project and fix stuff on your own. I would be happy to get pull requests!*

### Import real roads to Cities Skylines from Open Street Map.

1. Go to [terrain.party](http://terrain.party), find you location and click on the download button. **Do not touch the scaling buttons!**
2. Unzip the downloaded file.
3. From the unzipped folder, move the PNGs to height maps folder:
    * macOS: ~/Library/Application Support/Colossal Order/Cities_Skylines/Addons/MapEditor/Heightmaps
    * Linux: ~/.local/share/Colossal Order/Cities_Skylines/Addons/MapEditor/Heightmaps
    * Windows: C:\Users\USERNAME\AppData\Local\Colossal Order\Cities_Skylines\Addons\MapEditor\Heightmaps
4. Open the README.txt and locate the following line: `http://terrain.party/api/export?name`. This line ends with the latitude and longitude of your specified area.
5. Copy those numbers to any location, reverse latitude and longitude and trim all four numbers to four decimal places. Here is an example: `8.894940,50.882951,8.638647,50.721255` would be `8.6386,50.7212,8.8949,50.8829`
6. Now open the following URL: `http://overpass-api.de/api/map?bbox=YOURNUMBERSHERE`, but replace `YOURNUMBERSHERE` with the reversed latitude and longitude (the above example will be `http://overpass-api.de/api/map?bbox=8.6386,50.7212,8.8949,50.8829`). This will download the Open Street Map file called map.
7. Now start the game and make sure the Mod is active.
8. Open the map editor and start a new map.
9. In the map editor, open the heightd map import and chose one of the four PNGs. Mostly, merged works best. ![Height Map import](/pics/heightmap.png)
10. After the height map is imported, click on the road button far right.
11. Provide the path to the downloaded map file and click on `Load OSM From File` (this will take a while, be patient). ![Import File](/pics/load.png)
12. After the import is done, click on `Make Roads`. This will take like forever. I recommend to wait additional 10-20 seconds, after the import is done. *You can play around with the Tolerance buttons, sometimes the results will be better.* ![Make Roads](/pics/make.png)
13. Now, try to fulfill the requirements based on the real region to be able to start the map in a game. This is also the point where you should place all trees and make forests and so on to be as close to real life as possible.d
14. After you are done, save the map and start a new game with the unlimited money mod enabled.
15. Finally, before you start the game, fix the broken roads.

##### Congrats. You are done.
