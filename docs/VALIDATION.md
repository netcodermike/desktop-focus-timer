# Validation

Authoring date: November 6, 2026. Host: Windows. Target: Windows desktop.

## Local result

Local build/model verification is recorded after completion. WPF interaction is a manual acceptance check.

## Reproducible checks

The GitHub Actions workflow builds/runs the following on `windows-latest`:

```sh
dotnet build
dotnet run --no-build -- --self-test
```

## Manual acceptance

Exercise start, pause, resume, duration validation, phase completion, close/reopen, and a one-minute break. Verify the completion counter persists.

A workflow file is not evidence of a passing run. Observed CI run links and source commits are recorded after publication. Native GUI, sound, sleep/resume, and platform permissions require direct target-platform acceptance even when model tests pass.
