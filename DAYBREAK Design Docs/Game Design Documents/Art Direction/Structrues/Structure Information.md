The goal is to create small encampments with one or two buildings, some planters, antennas, and wire fences. These can be arranged in a circular area to fit with the terrain generator's capabilities. A great example of this are the Points of Interest in helldivers 2. Small areas that contain 2 - 3 structures and a core focus like a collectable or interactable element. 

Fences will act as barriers between the player and the enemies for a time while also acting as a sort of hinderance as well. Players can either hide behind them, get trapped behind them, or destroy them to by pass them. Enemies over time will also be able to destroy the fences.

It will be easier to make simple props that you can arrange to make structures. Make a sci-fi looking building/house or two. Make some metal crates. One section of wired fence. Maybe some planters that can be arranged in rows. 

- [ ] 1-2 Small Sci-fi Buildings
- [ ] 2-3 Variations of metal crates
- [ ] One Section of Wired Fence
- [ ] Optional Planter

Then once you're done. You can import these to Unity. Create an empty object in the scene that will act as the prefab for a structure. Arrange the props you created in different ways to create variations. Once you're done making a prefab, save it in the assets menu in the prefabs folder. Then go to Assets/ScriptableObjects. Go to Structures. Either copy one the existing structures or right click, Create, 3DTerrainGenerator/StructureDataObject and fill out the properties. The prefab is the structure you just created. Give the radius or size of the structure. And you should be done.

If you are up to it. You can try adding it to the game scene and generating a level. Either in Assets/ScriptableObject, open Biomes. Select Biome 1 or 2 and open the drop down for structures. There, simply remove the debug structures and add your own. Once that is finished. Open the Game Scene, find the terrain generator object, and press generate level.

If you have any questions or need any help. Let me know. - Dan