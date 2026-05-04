# Statistical View

This folder contains the scripts that power the statistical review page. The scene presents summary metrics for a completed session, lets the user switch between hands and movement components, and draws the deviation graphs for the selected data.

## Files In This Folder

- `StatisticalViewManager.cs`: Main controller for the statistical review page. It loads processed analysis data from `AnalysisResultsStore`, reacts to dropdown changes, refreshes the numeric summary panels, and redraws the graphs.
- `StatisticsManager.cs`: Small helper that writes a `Statistics` object into four text labels for average, max, min, and standard deviation.
- `StatisticalView.md`: This documentation file.

## Scene Connection

The attached `StatisticalView.unity` scene is the visual review screen for the statistical tab. It contains the chart objects, dropdowns, export button, and the summary text panels that `StatisticalViewManager` wires together.

## How The Page Works

1. When the scene starts, `StatisticalViewManager` connects the UI callbacks for the hand/path dropdown, deviation dropdown, component dropdown, export button, and end-analysis button.
2. The manager pulls the processed analysis data from `AnalysisResultsStore`.
3. The selected hand controls which side of the session is shown.
4. The selected component controls which movement zone is used for the numeric target statistics.
5. The selected deviation type controls which `Statistics` object is shown in the summary panel.
6. The chart data is rebuilt from the stored analysis arrays and grouped by analysis mode so the line colors change when the movement mode changes.
7. Export writes the analysis results back out through `AnalysisResultsStore`.
8. End Analysis returns to the `ReviewPastSession` scene.

## What The Manager Displays

The page is split into two kinds of output:

- Summary numbers: `StatisticsManager` fills the average, maximum, minimum, and standard deviation labels for the currently selected deviation.
- Graphs: The manager updates the X-axis, Y-axis, Z-axis, and magnitude deviation charts so the user can inspect how the trial changed over time.

The graphs always use the overall movement data for the selected hand, while the dropdowns control the summary panels. That separation makes the page useful for both high-level summary reading and detailed movement inspection.

## Data Flow

The statistical page depends on the analysis pipeline that runs after a session is reviewed. `AnalysisResultsStore` holds the processed data, and `StatisticalViewManager.LoadFromStore()` reads it when the scene becomes active or when the data changes. The stored values include the analyzed datasets, target analysis results, point-type sequences, and the full time series used by the graphs.

If the store does not yet have data, the page does nothing until the review pipeline finishes loading the results.

## Adding A New Statistic Or Graph

If you want to add another metric to this page, follow the same pattern already used here:

1. Add the value to the analysis data model or processing step.
2. Add a text field or chart object to `StatisticalView.unity`.
3. Add a serialized field to `StatisticalViewManager.cs`.
4. Update `SetResults()`, `RefreshData()`, or `UpdateGraphs()` so the new UI element receives the correct data.
5. If the new metric depends on a dropdown choice, hook it into `Start()` the same way the existing controls are wired.
6. Update `StatisticsManager.cs` only if the new output is another text-based statistics panel.

The important rule is that the scene, the processed analysis data, and the manager logic all need to stay aligned. If one of those pieces changes without the others, the page will show stale or incomplete statistics.