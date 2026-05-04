# Home Page

This folder contains the scripts that power the application's main home screen. The scene acts as the navigation hub for the rest of the app.

## Files In This Folder

- `HomeManager.cs`: Binds the Home screen buttons to their actions. It loads the Start New Session, Review Past Session, and Customize Session scenes, and it quits the application when the Quit button is pressed.

## Scene Connection

The attached `HomeScreen.unity` scene is the UI that uses this script. In that scene, the HomeManager component is connected to the four main buttons on the canvas so each button can trigger its corresponding action.

## How The Pieces Fit Together

1. The Home screen scene loads.
2. `HomeManager` registers click handlers in `Awake()`.
3. The user selects one of the navigation buttons.
4. The matching scene is loaded, or the application exits if Quit is pressed.

## Adding A New Button Or Action

If you need to add another option to the Home screen, follow the same pattern:

1. Add the new button to the `HomeScreen.unity` scene.
2. Add a serialized `Button` field to `HomeManager.cs`.
3. Hook the field up in the Inspector.
4. Register the click handler in `Awake()`.
5. Load the desired scene or call the desired action when the button is pressed.

Keep the script and scene wiring in sync so the button does not remain unresponsive in Play mode.