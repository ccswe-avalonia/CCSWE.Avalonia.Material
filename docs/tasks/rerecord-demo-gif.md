# Re-record the Demo GIF

`docs/images/demo.gif` predates 12.2: it shows the old filled-button color and no contrast toggle.

1. Run the tour with `dotnet run --project src/CCSWE.Avalonia.Material.Demo -- --record` and capture it with ShareX on Windows.
2. Run `eng/optimize-demo-gif.sh <capture.gif>` (needs `gifsicle`); it writes `docs/images/demo.gif`.
3. Delete this task.
