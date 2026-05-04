# Start New Session

This folder contains the scripts that create a new trial session before the live experiment starts. The page collects the session metadata, loads a saved trial configuration, and hands the completed session setup to the live trial scene.

## Files In This Folder

- `StartNewSessionManager.cs`: Main controller for the Start New Session page. It reads the session name, participant ID, and notes, loads a saved trial configuration file, enables the Begin button only when the required data is present, and starts the live trial scene.
- `SessionInfo.cs`: Small data container for session metadata. It stores the session name, participant ID, and notes in a simple structure.
- `StartNewSession.md`: This documentation file.

## Scene Connection

The attached `StartNewSession.unity` scene is the UI that uses this script. In that scene, the manager is connected to the text fields, upload button, Begin button, and Cancel button that make up the form.

## How The Page Works

1. The user enters a session name and participant ID.
2. The user optionally adds notes for the trial.
3. The user uploads a saved trial settings JSON file.
4. `StartNewSessionManager` stores the loaded settings and enables the Begin button once the required fields are filled in.
5. When Begin is clicked, the page creates a `TrialSessionInformation` object, stores it in `SessionManager`, clears the form, and loads `LiveTrialView`.
6. Cancel clears the form, destroys the session manager singleton, and returns to the home screen.

## Why The Configuration File Matters

The Start New Session page is the handoff point between setup and execution. The configuration file provides the full `TrialSettingsData` object that the live trial scene needs, including visibility, offset, background, target, and color settings. Without that file, the live trial cannot start because it would not know how the trial should behave.

## Adding A New Session Field

If you need to add another field to the Start New Session page, use the same pattern:

1. Add the new value to the session data model if it needs to be saved with the trial.
2. Add a matching UI control to `StartNewSession.unity`.
3. Add a serialized field to `StartNewSessionManager.cs`.
4. Include the value when building `TrialSessionInformation` in `OnBeginTrialButtonClicked()`.
5. Update any validation logic so the Begin button only becomes available when the page is ready.
6. Update this document so the new field is easy to understand later.

The key rule is that the form, the saved session data, and the live trial startup all need to agree. If one of those pieces is missing, the session will not transfer cleanly into the experiment.