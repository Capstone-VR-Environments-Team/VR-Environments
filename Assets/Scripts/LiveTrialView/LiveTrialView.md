# Live Trial View

This folder contains the scripts that run the main live experiment scene. It is the core of the project: it initializes XR, shows the trial UI, records hand and eye data, spawns and manages targets, logs notes and events, and saves the trial when it ends.

## Files In This Folder

- `LiveTrialViewManager.cs`: Main UI controller for the live trial scene. It handles the begin/end trial buttons, home navigation, the on-screen timer, the participant experiment name/ID display, and the notes log UI.
- `ExperimentController.cs`: Bridges the UI and the event system. It primes the trial, starts recording once the trial begins, and stops the experiment when the end button is pressed.
- `SessionManager.cs`: Singleton that stores the active trial settings, keeps track of trial time, saves the trial JSON summary and target-event CSV, and exposes helper getters used throughout the live scene.
- `LoggingManager.cs`: Collects live tracking data and experiment events through the event bus, buffers them during the trial, and writes the CSV log when the trial ends.
- `CSVLogger.cs`: Concrete logger used by `LoggingManager` to serialize tracking samples into a CSV file.
- `HandDataRecorder.cs`: Samples the left and right controller transforms during the trial and publishes them as tracking events.
- `EyeDataRecorder.cs`: Samples gaze and pupil data from the headset during the trial, handles Varjo gaze calibration, and publishes eye-tracking events.
- `SphereManager.cs`: Builds the ordered target spheres for the trial, applies visibility and offset settings, controls flicker behavior, and advances the participant through the target sequence.
- `SphereContact.cs`: Handles trigger interactions on each target sphere, including the start sphere, target hits, re-entry, and exit events.
- `ProximityAlertTrigger.cs`: Adds the proximity trigger around each target so the system can detect when a hand gets close enough to reveal hands or raise proximity events.
- `BackgroundManager.cs`: Applies the configured skybox or video background, restores the default background when the trial is complete, and reveals the participant canvas when the live portion ends.
- `ObjectSpawner.cs`: Spawns moving background objects during the trial when the moving-background setting is enabled.
- `SpawnedObjectMover.cs`: Moves spawned background objects through the scene and destroys them after they travel far enough.
- `XRManager.cs`: Starts and stops XR subsystems when entering or leaving the live VR scene.
- `TrackingData.cs`: Serializable data model for one tracking sample, including hand transforms and eye-tracking values.
- `ILogger.cs`: Logging abstraction used by the CSV logger implementation.

## Scene Connection

The attached `LiveTrialView.unity` scene wires these scripts together. The scene contains the XR rig, the live camera, the participant and instructor canvases, the trial UI, the target controllers, the logging objects, and the background spawner. The `GameManager` object in the scene owns the main runtime pieces that drive the trial.

## How The Trial Works

1. The scene loads and `XRManager` turns XR on.
2. `SessionManager` provides the current trial settings that were chosen earlier.
3. `BackgroundManager` applies the configured skybox, photo, or video background.
4. `SphereManager` creates the ordered target spheres and applies the configured hand visibility, target visibility, offsets, flicker, colors, and start delay.
5. `HandDataRecorder`, `EyeDataRecorder`, and `LoggingManager` begin collecting movement, gaze, notes, and target events when the trial starts.
6. `LiveTrialViewManager` updates the UI, timer, and notes panel while the participant works through the targets.
7. When the final target is collected, or when the trial ends from the UI, the collected data is saved and the scene transitions back home.

## Data Flow

- Hand and eye tracking events are published on the event bus and captured by `LoggingManager`.
- `LoggingManager` stores the live samples in `TrackingData` and the event history in `CollectedTimingData`.
- `SessionManager` saves the final session JSON and the target-event CSV into the trial output folder.
- `CSVLogger` writes the per-sample tracking CSV for the live recording.
- `LiveTrialViewManager` also writes user notes into the on-screen log so the participant and instructor can see them during the run.

## Adding New Live-Trial Behavior

If you need to add a new live-trial feature, the usual pattern is:

1. Decide whether the new behavior belongs in the UI, the tracking pipeline, target handling, or the background/system setup.
2. Add any new state to the data model first if it needs to be saved.
3. Publish or consume events through the event bus instead of wiring objects together directly where possible.
4. Update `LoggingManager.cs` and `SessionManager.cs` if the feature changes what should be recorded or saved.
5. Update the relevant scene objects in `LiveTrialView.unity` so the new component is actually connected.
6. Document the new behavior here so the folder stays understandable for future changes.

The most important thing to keep in sync is the trial settings, the runtime scene objects, and the save/logging path. If one of those pieces is missing, the trial may appear to work but the data will not be saved correctly.