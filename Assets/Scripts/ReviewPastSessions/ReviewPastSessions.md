# Review Past Sessions

This folder contains the scripts that let the team load a completed trial, run the analysis pipeline, and review the results in either a statistical or interactive format. It is the bridge between the saved live-trial data and the post-trial review experience.

## Files In This Folder

- `ReviewPastSessionsManager.cs`: Main controller for the review landing page. It shows the loaded session metadata, enables the review buttons only when a session is available, and exposes button hooks for canceling, selecting a session, or opening a review mode.
- `ReviewUIManager.cs`: Handles the landing-page navigation. It opens the review page, switches to the statistical or interactive view scenes, and clears the stored analysis when the user returns home.
- `FullAnalysisManager.cs`: Starts the review workflow by letting the user browse for a saved trial folder, loading the JSON and CSV files, and running the full analysis pipeline.
- `AnalysisProcessingService.cs`: Core analysis entry point. It slices the raw movement data into segments, runs geometry analysis, and combines the results into the processed analysis data used by the review views.
- `AnalysisResultsStore.cs`: Singleton cache for the selected session, raw tracking data, and processed analysis results. It also supports exporting the analyzed data back out to CSV files.
- `CollectedTimingData.cs`: Serializable structure for the recorded timing and event data that came from the live trial.
- `DataAnalyzer.cs`: Calculates summary statistics such as average, min, max, median, standard deviation, and duration from a list of values and times.
- `DataSlicer.cs`: Breaks the raw movement timeline into meaningful segments based on target hits, proximity events, exits, and re-entries.
- `GeometryAnalyzer.cs`: Turns a sliced movement segment into geometry metrics, such as deviation from a line or a point, so the review tools can measure performance.
- `GeometryInputData.cs`: Input model for the geometry analyzer.
- `GeometryResults.cs`: Output model for the geometry analyzer, including the total, search, approach, and previous-sphere result sets.
- `SegmentDataStructures.cs`: Supporting data structures for the slicing and segment-analysis pipeline.
- `Statistics.cs`: Basic statistics container used throughout the review and export pipeline.
- `TargetAnalyzer.cs`: Computes target timing metrics such as total target-to-target time, search time, and pre-search time.
- `TargetHitSequenceBuilder.cs`: Normalizes target-hit order and ensures the first configured target is represented when the recorded sequence is incomplete.

## Scene Connection

The attached `ReviewPastSession.unity` scene wires these scripts together. The scene contains the review canvas, the session metadata panel, the analysis manager, and the UI manager that routes the user into the statistical or interactive review scenes.

## How The Review Flow Works

1. The user opens the review page from the home screen.
2. `FullAnalysisManager` lets the user browse to a saved trial folder.
3. The manager loads the trial JSON, the raw tracking CSV, and any target-event CSV that belongs to the run.
4. `AnalysisProcessingService` slices the raw hand data into segments and turns those segments into geometry results and statistics.
5. `AnalysisResultsStore` keeps the loaded session and processed data available for the review scenes.
6. `ReviewPastSessionsManager` shows the loaded file name, participant ID, session name, and notes.
7. The user can open the statistical view, the interactive view, or cancel back to the home screen.

## Data Pipeline

- The live trial saves session JSON, tracking CSV, and target-event CSV files.
- `FullAnalysisManager` finds those files inside the selected trial folder and loads them into memory.
- `TargetHitSequenceBuilder` and `TargetAnalyzer` organize the target events so the timing metrics are consistent even if the original record is incomplete.
- `DataSlicer`, `GeometryAnalyzer`, and `DataAnalyzer` convert the raw positions into segment-level and summary-level measurements.
- `AnalysisResultsStore` keeps the final processed results ready for the statistical and interactive review scenes.

## Exporting And Reviewing Results

`AnalysisResultsStore` can export the processed analysis into CSV files for external inspection. This keeps the review workflow useful both inside Unity and outside the project if someone wants to inspect the numbers in a spreadsheet.

## Adding A New Review Metric Or Screen

If you need to extend the review pipeline, follow the same pattern:

1. Decide whether the feature belongs in the loader, the processing pipeline, the store, or the UI.
2. Add new data fields to the relevant analysis or timing structure first.
3. Update the processing service so the new metric is computed from the raw data.
4. Store the result in `AnalysisResultsStore` if the UI needs to use it later.
5. Wire any new buttons or fields into `ReviewPastSession.unity` and the relevant manager script.
6. Update this document so the new review behavior is easy to find.

The most important thing to keep in sync is the saved trial format, the analysis code, and the UI that displays the result. If one of those pieces changes without the others, the review screens will become inconsistent or stop loading data correctly.