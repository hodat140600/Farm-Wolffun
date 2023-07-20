# Farm Wolffun
## Scripts

### Manager Scripts

All managers can be accessed with Get() example TheGame.Get()

- [TheGame.cs](Assets/Scripts/TheGame.cs)

  *Core manager of the gameplay, contain function for loading and save the game. Will spawn saved 
objects when the scene starts, and will make the time progress and manage time speed.*
- [TheControls.cs](Assets/Scripts/TheControls.cs)

  *Related to keyboard controls, or mouse controls are in this script.*
- [TheData.cs](Assets/Scripts/TheData.cs)

  *Loads all data contained in the Resources folder. Also spawn the UI and the Audio manager.*
- [TheAudio.cs](Assets/Scripts/TheAudio.cs)

  *Use this to play SFX or music. It works by channel so 2 sfx in different channel will play at the same 
time, but sfx in the same channel will replace each other. This prevent sounds accumulation if many 
actions happen at the same time, it also allow to control volume globally.*
- [TheAudio.cs](Assets/Scripts/TheAudio.cs)

  *Use this to play SFX or music. It works by channel so 2 sfx in different channel will play at the same 
time, but sfx in the same channel will replace each other. This prevent sounds accumulation if many 
actions happen at the same time, it also allow to control volume globally.*
- [WorkerManager.cs](Assets/Scripts/WorkerManager.cs)

  *Manage the schedule of colonists, will send them to work automatically to the highest priority work 
location that is unassigned. And will make the worker start/stop working when a condition is met.*
- [EventManager.cs](Assets/Scripts/EventManager.cs)

  *Manages events and triggers them. Events are things that can happen during the game and affect 
the gameplay.*

### Gameplay Scripts

- [Selectable.cs](Assets/Scripts/Selectable.cs)

  *Basic components added to almost all interactable objects. Allow the object to be 
selected. This script is used by almost all other gameplay scripts.*
