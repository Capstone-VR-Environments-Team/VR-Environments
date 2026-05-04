# VR-Environments

VR-Environments is a Unity-based VR application for running, logging, and reviewing trial sessions. It supports a simple end-to-end workflow: set up a session, customize the trial, run the live experiment in VR, then review the recorded results in both interactive and statistical views.

## What The Project Includes

- A home screen for navigating between the main parts of the app.
- A start-new-session flow for entering participant/session metadata and loading saved trial settings.
- A customize-session page for editing trial configuration before a run.
- A live-trial scene that records experiment data in real time.
- Review pages for analyzing completed sessions.
- Dedicated docs under `Assets/Scripts/*/*.md` that explain each major feature folder in more detail.

## Core Flow

1. Open the project in Unity.
2. Start from the home screen.
3. Create a new session or customize the trial settings.
4. Launch the live trial and record data.
5. Review the saved session through the statistical or interactive review pages.

## Project Requirements

- Unity `6000.2.7f2`
- XR hardware or an XR-capable development setup
- The packages listed in `Packages/manifest.json`

Notable dependencies include XR Interaction Toolkit, OpenXR, Varjo XR, the Input System, URP, and XCharts.

## Main Scenes

- `HomeScreen`: Entry point for navigating the app.
- `StartNewSession`: Collects session metadata and loads trial settings.
- `CustomizeSession`: Edits trial configuration.
- `LiveTrialView`: Runs the live VR experiment and records data.
- `ReviewPastSession`: Loads completed sessions for review.
- `StatisticalView`: Shows summary statistics and deviation graphs.
- `InteractiveView`: Presents the interactive review experience.

## Scripts And Documentation

The `Assets/Scripts` folder contains the main runtime code. Each major feature folder includes its own markdown guide:

- `Assets/Scripts/HomePage/HomePage.md`
- `Assets/Scripts/StartNewSession/StartNewSession.md`
- `Assets/Scripts/CustomizeSession/CustomizeSession.md`
- `Assets/Scripts/LiveTrialView/LiveTrialView.md`
- `Assets/Scripts/ReviewPastSessions/ReviewPastSessions.md`
- `Assets/Scripts/StatisticalView/StatisticalView.md`
- `Assets/Scripts/InteractiveView/InteractiveView.md`

The shared utility scripts in `Assets/Scripts` provide supporting behavior such as file selection, JSON handling, singleton management, full-screen toggling, and the event bus used by the runtime scenes.

## How To Open And Run

1. Open the repository in Unity `6000.2.7f2`.
2. Let Unity finish importing packages and compiling scripts.
3. Open the desired scene from `Assets/Scenes`.
4. Enter play mode in the editor, or build the project for the target VR device.

If you are working on the experiment flow, start with `HomeScreen` and move through the session setup screens before launching `LiveTrialView`.

## Notes For Development

- The project uses a scene-based workflow, so most features are wired through Unity scene objects and serialized references.
- Trial settings are saved and loaded as JSON.
- Live trial data is recorded for later review.
- Review pages depend on the analysis data produced from completed sessions, so they should be opened after a session has been run or loaded.

## Additional Documentation

If you want implementation details for a specific feature, start with the markdown file in that feature's script folder. Those pages describe the main scripts, the scene wiring, and how to extend the feature without breaking the rest of the workflow.