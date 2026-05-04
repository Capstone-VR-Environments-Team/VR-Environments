# Interactive View

This folder contains the scripts that power the review/analysis view for a past trial. The scene lets the user inspect hand paths, target locations, and summary statistics while moving around the 3D environment.

## Files In This Folder

- `InteractiveViewManager.cs`: Main controller for the review scene. It loads the stored trial data, builds the left/right/target path lines, spawns target spheres, applies saved colors and target size, and connects the UI toggles and dropdown to the displayed data.
- `CameraController.cs`: Gives the review camera keyboard, mouse, and scroll-wheel controls so the user can move around the scene, look around, and zoom.
- `HandTrialVisualizer.cs`: Standalone trail visualizer that can load one or more CSV files and render them as line trails with toggle controls. It is useful for comparing raw trials outside the main review flow.
- `SphereLabel.cs`: Keeps a target sphere's label centered and facing the camera, and appends target numbers when multiple hits land on the same location.

## Scene Connection

The attached `InteractiveView.unity` scene wires these scripts together. The main camera uses `CameraController`, the canvas hosts the `InteractiveViewManager` controls, and the scene is set up to display the loaded trial data in 3D.

## How The Pieces Fit Together

1. The scene loads with the review camera and UI ready.
2. `InteractiveViewManager` reads the active analysis data from the store when the scene becomes available.
3. The manager applies saved colors and target sizes, then creates line renderers for the left hand, right hand, and optimal target path.
4. Target hits are turned into spheres, and `SphereLabel` keeps their numbering readable in 3D.
5. The user can toggle paths and targets on or off, change the selected statistics view, move the camera, or return to `ReviewPastSession`.

The main thing to keep in sync is the data store, the scene objects, and the UI callbacks. If one of those pieces is missing, the review view will either show incomplete data or stop responding to the new control.