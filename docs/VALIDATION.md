# Validation

Authoring date: November 6, 2026. Host: Windows. Target: Windows desktop.

## Local result

Windows local dotnet build passed with zero warnings and zero errors. The deadline/pause/phase/persistence model self-test exited successfully. WPF interaction remains a manual acceptance check.

## Reproducible checks

The GitHub Actions workflow builds/runs the following on `windows-latest`:

```sh
dotnet build
dotnet run --no-build -- --self-test
```

## Manual acceptance

Exercise start, pause, resume, duration validation, phase completion, close/reopen, and a one-minute break. Verify the completion counter persists.

A workflow file is not evidence of a passing run. Observed CI run links and source commits are recorded after publication. Native GUI, sound, sleep/resume, and platform permissions require direct target-platform acceptance even when model tests pass.

## Observed CI result

Passed on November 6, 2026: [target-platform build/check](https://github.com/netcodermike/desktop-focus-timer/actions/runs/42198869227).

Verified source commit: `33f51d06e05793ca5d7ebe8b3fad88801f42a047`. This evidence identifies the exact tested source; a later documentation-only commit does not change that source.

The commands listed above passed. Native GUI/manual acceptance is not claimed by this CI result.
