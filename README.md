# Farm Wolffun
## Scripts

### Manager Scripts

All managers can be accessed with Get() example TheGame.Get()

- [TheGame.cs](Assets/Scripts/TheGame.cs)

  *Core manager of the gameplay, contain function for loading and save the game. Will spawn saved objects when the scene starts, and will make the time progress and manage time speed.*
- [TheControls.cs](Assets/Scripts/TheControls.cs)

  *Related to keyboard controls, or mouse controls are in this script. Check the component to see controls setting*
- [TheData.cs](Assets/Scripts/TheData.cs)

  *Loads all data contained in the Resources folder. Also spawn the UI and the Audio manager.*
- [TheAudio.cs](Assets/Scripts/TheAudio.cs)

  *Use this to play SFX or music. It works by channel so 2 sfx in different channel will play at the same time, but sfx in the same channel will replace each other. This prevent sounds accumulation if many 
actions happen at the same time, it also allow to control volume globally.*
- [TheAudio.cs](Assets/Scripts/TheAudio.cs)

  *Use this to play SFX or music. It works by channel so 2 sfx in different channel will play at the same time, but sfx in the same channel will replace each other. This prevent sounds accumulation if many 
actions happen at the same time, it also allow to control volume globally.*
- [WorkerManager.cs](Assets/Scripts/WorkerManager.cs)

  *Manage the schedule of colonists, will send them to work automatically to the highest priority work location that is unassigned. And will make the worker start/stop working when a condition is met.*
- [EventManager.cs](Assets/Scripts/EventManager.cs)

  *Manages events and triggers them. Events are things that can happen during the game and affect the gameplay.*

### Gameplay Scripts

- [Selectable.cs](Assets/Scripts/Selectable.cs)

  *Basic components added to almost all interactable objects. Allow the object to be selected. This script is used by almost all other gameplay scripts.*
- [Interactable.cs](Assets/Scripts/Interactable.cs)

  *Allow characters to interact with it (with the right click). Can assign actions on this component, and worker will execute the first possible action in the list when right click*
- [UniqueID.cs](Assets/Scripts/UniqueID.cs)

  *generate a unique ID linked to the object. This ID is used by the save system. This script is used by almost all other gameplay scripts. Generate a new random ID by clicking the button, or generate all IDS in the scene by going to Farm Wolfun -> Generate IDS.*
- [Character.cs](Assets/Scripts/Character.cs)

  *Anything that can move and execute actions, including Worker.*
- [Selectable.cs](Assets/Scripts/Selectable.cs)

  *Basic components added to almost all interactable objects. Allow the object to be selected. This script is used by almost all other gameplay scripts.*
- [Worker.cs](Assets/Scripts/Worker.cs)

  *Workers are characters controlled by the player, and by the WorkerManager, they can be assigned to actions automatically or manually.*
- [WorkerAttribute.cs](Assets/Scripts/WorkerAttribute.cs)

  *Allow a worker to have attributes such as health, energy, hunger.*
- [NPC.cs](Assets/Scripts/NPC.cs)

  *NPC will save the current action of the character, and also spawning new characters.*
- [Buildable.cs](Assets/Scripts/Buildable.cs)

  *An object that can be placed on the map manually by the player, selecting the position with the mouse. It will check for valid terrain and obstacles before allowing to be placed.*
- [Construction.cs](Assets/Scripts/Construction.cs)

  *Constructions are objects that can be built. Once placed with the Buildable script, Worker need to go to the construction and build it with resources. It has a build progress and will swap mesh depending off the progress (under construction or completed).*
- [House.cs](Assets/Scripts/House.cs)

  *A building that increases the population limit.*
- [Factory.cs](Assets/Scripts/Factory.cs)

  *A building that produces things (items, characters). Can be required worker to be assigned to the building to work*
- [Storage.cs](Assets/Scripts/Storage.cs)

  *A building were items can be dropped, to be placed into the global inventory.*
- [Inventory.cs](Assets/Scripts/Inventory.cs)

  *Allow the object to hold items. If the global toggle is checked, will link to the global inventory (which is the one displayed in the UI and accessible from anywhere). For example, a storage should have the global set to on. But a character would have it to off, because their inventory is not sharedwith the global one.*
- [Trader.cs](Assets/Scripts/Trader.cs)

  *A NPC you can trade with. Add items as “starting items” in their inventory to add items to their list.*

### Save Data Scripts
- [SaveData.cs](Assets/Scripts/SaveData.cs)

  *This is the main data script that contains all the saved data. There is only one instance of this and it will be serialized when saving. It contains many useful functions to save, load, create a new game, or edit or access data.*

### Data Scripts
Data script are scriptable objects that define the structure of the data. These data file should be placed in the Resources folder so that TheData.cs load them automatically at the start of scene. Most data files are linked to a prefab that will be spawned when this object is created. To create data file: right click in Project files tab -> Create -> Farm Wolffun.

- [AssetData.cs](Assets/Scripts/AssetData.cs) [GameData.cs](Assets/Scripts/GameData.cs)

  *These contain generic settings for the whole game. There should be only 1 of each.*
- [CSData.cs](Assets/Scripts/CSData.cs)

  *Parent class for all Worker data.*
- [CraftData.cs](Assets/Scripts/CraftData.cs)

  *Anything that can be crafted, parent class. Inherited by WorkerData, ItemData, and ConstructionData.*
- [SpawnData.cs](Assets/Scripts/SpawnData.cs)

  *This is the main data script that contains all the saved data. There is only one instance of this and it will be serialized when saving. It contains many useful functions to save, load, create a new game, or edit or access data.*
- [ItemData.cs](Assets/Scripts/ItemData.cs)

  *Data for Items*
- [WorkerData.cs](Assets/Scripts/WorkerData.cs)

  *Data for workers. Multiple Workers can share the same prefab. Should have one data file of this type for each worker class in game.*
- [WorkerSkinData.cs](Assets/Scripts/WorkerSkinData.cs)

  *Constains the visuals (prefab) and the possible names of a worker. A WorkerData can have more than one possible skin, it will be selected at random if more than 1.*
- [ConstructionData.cs](Assets/Scripts/ConstructionData.cs)

  *Data for constructions.*
- [GroupData.cs](Assets/Scripts/SaveData.cs)

  *can be used to classify Selectables, or CraftData. And some actions will use groups to filter objects.*

### Action Scripts
Actions allow to define character interactions with objects in the game. Actions are also scriptable objects, but they contain a code logic for different types of actions that can be performed. There are Actions and Works which are a bit different. There are many different types of actions but they all inherit from the same two scripts.
Create a custom Action, inherit a new script from one of these two basic classes, and override the main functions.

- [ActionBasic.cs](Assets/Scripts/ActionBasic.cs)

  *Actions are specific commands that will be executed by a character. For example, eating, harvesting, building, sleeping. They usually target only 1 object and the action is performed by the character on 
that object (Selectable) when the character is within the use_range.*
- [WorkBasic.cs](Assets/Scripts/WorkBasic.cs)

  *Works can only be performed by worker, and can contain a series of automated actions (for example: move to house, harvest plant, bring it back to base, etc). Works are the things that are used by the WorkerManager to auto assign work to Workers.*
  
