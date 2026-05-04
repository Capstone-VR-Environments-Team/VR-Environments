# Customize Session

This folder contains the code and data structures that power the Customize Session screen in Unity. The scene lets the user configure a trial before saving it as a session settings file.

## Files In This Folder

- `CustomizeSessionManager.cs`: Main controller for the customization UI. It reads values from the input fields, shows or hides related controls, validates numeric and color inputs, loads target locations from a file, and saves the final settings into a `TrialSettingsData` object.
- `CustomizeSessionSubpageController.cs`: Handles the subpage navigation inside the Customize Session screen. It keeps only one settings section visible at a time, such as visibility, offset, target, background, or color settings.
- `TrialSettingsData.cs`: Defines the data model used to store a customized session. It includes the full settings payload, such as visibility, offset, background, target, and color settings, plus the `TargetImportData` helper used when importing target coordinates from CSV or JSON.
- `Direction.cs`: Simple enum that lists the allowed movement directions for the background objects.

## Scene Connection

The attached `CustomizeSession.unity` scene is the UI that uses these scripts. In that scene, the controller and manager components are wired to the buttons, dropdowns, toggles, and text fields that appear on the Customize Session screen.

## How The Pieces Fit Together

1. The user opens the Customize Session scene.
2. `CustomizeSessionSubpageController` shows the selected subpage and hides the others.
3. `CustomizeSessionManager` watches the UI state, enables or disables actions as needed, and collects all settings when Save is clicked.
4. `TrialSettingsData` stores the final values in a serializable format.
5. The session is saved through `SessionManager` for later use.

## Adding A New Setting

If you need to add another setting to this folder, the usual workflow is:

1. Add the new value to the data model in `TrialSettingsData.cs` so it can be saved and loaded.
2. Add the matching UI control to `CustomizeSession.unity` and wire it to `CustomizeSessionManager` in the Inspector.
3. Add a serialized field in `CustomizeSessionManager.cs` for the new control.
4. Read, validate, and assign the value in `OnSaveButtonClicked()`.
5. If the setting belongs to a separate panel, update `CustomizeSessionSubpageController.cs` so the new panel can be shown and hidden like the others.
6. Update this documentation so the new setting is listed here.

Keeping the data model, scene wiring, and save logic in sync is the important part. If one of those pieces is missing, the setting will not persist correctly.