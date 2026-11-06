# Desktop Focus Timer

Run focus and break sessions with pause/resume, adjustable durations, completion counts, and a Windows sound cue.

[![Build and verify](https://github.com/netcodermike/desktop-focus-timer/actions/workflows/verify.yml/badge.svg)](https://github.com/netcodermike/desktop-focus-timer/actions/workflows/verify.yml)

**Language:** C# · **Platform:** Windows desktop · **App type:** Native Windows productivity app

First-release learning implementation, with explicit scope below. This is one independently runnable app, not a placeholder in a combined repository.

## Features

- 25-minute focus and 5-minute break defaults.
- Adjustable 1–180 minute focus and 1–60 minute break periods.
- Pause/resume, reset, and persistent completed-session count.
- Phase completion sound and window activation.

## Requirements

Windows 10/11 with the .NET 10 SDK and Windows Desktop runtime. WPF is Windows-only. No NuGet packages.

## Quick start

```sh
git clone https://github.com/netcodermike/desktop-focus-timer.git
cd desktop-focus-timer
dotnet run
# Build a release executable:
dotnet publish -c Release --self-contained false -o publish
```

Commands run from the repository root. Lines marked for a particular operating system apply only there. Language tools are prerequisites, not installed by the app. Close desktop windows normally; terminal/server apps stop with Ctrl+C unless their usage notes say otherwise.

## Example workflow

Set focus and break to one minute, apply durations/reset, and start. Pause partway through, wait, and resume; paused time should not consume the countdown. At completion verify the break phase appears and completed count increments. Start the break explicitly.

## Architecture, configuration, and data

`Program.cs` separates the Session state model from a code-built WPF window. A DispatcherTimer refreshes at 200 ms, but remaining time derives from a UTC deadline rather than tick counts. Session completion changes phase and stops; press Start to begin the next phase. Settings and completion counts are written under `%LOCALAPPDATA%/NetCoderMike/FocusTimer/settings.json` via same-directory replacement. Active countdown state is not restored on relaunch. --self-test executes the model without opening a window.

## Verification

Build with the quick-start command where needed, then run:

```sh
dotnet run -- --self-test
```

Manual acceptance: Exercise start, pause, resume, duration validation, phase completion, close/reopen, and a one-minute break. Verify the completion counter persists.

See [validation record](docs/VALIDATION.md) for observed checks and their limits. CI results verify only what the workflow runs; they do not establish native GUI correctness or production readiness.

## Limitations and next steps

No tray icon, task list, schedule automation, notifications API, session history, or automatic next-phase start. Closing stops the current countdown. Clock changes affect deadlines; after sleep a crossed deadline completes on the next UI tick. Sound depends on Windows audio settings. Only one app instance should edit the settings store.

## Troubleshooting

Build/run on Windows with the .NET 10 Windows Desktop components. A malformed saved settings file stops launch with an error; preserve it before resetting. Start/resume does not apply edited duration fields; use the reset button to apply them. If sound is absent check the system sound scheme.

## Source map

- `FocusTimer.csproj`
- `Program.cs`

## Development and license

Read [contributing](CONTRIBUTING.md) before changing the app, and [security/privacy](SECURITY.md) before supplying private data. Use synthetic fixtures for tests. No telemetry, paid API key, or account is required by the app itself. Dependencies, when present, are stated in Requirements.

[MIT license](LICENSE), copyright 2026 netcodermike.
