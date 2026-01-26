# Building Plugin.Firebase (reproducible setup)

Goal: avoid “works on my machine” issues around .NET SDKs, workloads/packs, and Xcode.

## Supported toolchain matrix
This repo targets `net9.0` + mobile TFMs (`net9.0-ios`, `net9.0-android`).

**Officially supported (reproducible):**
| .NET SDK | Workloads | Notes |
|---:|---|---|
| 9.0.306 | Workload-set mode + workloads aligned to 9.0.306 | Pinned by `global.json`. Used for CI guidance and docs. |

**Best-effort / observed OK (not CI-pinned):**
| .NET SDK | Workloads | Notes |
|---:|---|---|
| 10.0.100 | Matching workloads for that SDK | Works locally when `global.json` is not used; we don’t guarantee it stays working. |

If you want to officially support a new toolchain combination (ex: .NET 10), propose it in a PR with CI updates and a documented matrix change.

## Quick start (diagnose first, then fix)

### Step 0 — Run the verifier (no installs)
Run:
```bash
bash scripts/verify-env.sh
```

If `scripts/verify-env.sh` ends with `STATUS=HEALTHY`, you can build iOS right away:
```bash
dotnet build src/Core/Core.csproj -c Release -f net9.0-ios
```

If it prints `STATUS=UNHEALTHY`, it will also print `REASON_1..N` and `NEXT_1..N` with suggested fix commands.

### Step 1 — Fix only if unhealthy (workloads / packs)
When SDK and workloads are out of sync, the most reliable repair path is:
```bash
dotnet workload config --update-mode workload-set
dotnet workload update --from-previous-sdk --version "$(dotnet --version)"
dotnet workload install ios maccatalyst maui --version "$(dotnet --version)"
```

Notes:
- You do **not** need to run `dotnet workload update` on every machine every time. Use it when you changed SDK feature bands, when packs are missing, or when you hit workload-related errors.
- Minimal iOS requirement to build `src/Core/Core.csproj` is Apple workloads/packs (`ios`). `maui` is needed for `sample/Playground`.

Then clean and rebuild:
```bash
dotnet clean Plugin.Firebase.sln -c Release
dotnet build src/Core/Core.csproj -c Release -f net9.0-ios
```

## Restore & build
Restore:
```bash
dotnet restore Plugin.Firebase.sln
```

Build the full solution (includes sample + integration tests):
```bash
dotnet build Plugin.Firebase.sln -c Release
```

If you don’t have local Firebase configs, use these builds to validate packages:

Build without mobile workloads:
```bash
dotnet build src/Auth/Auth.csproj -c Release -f net9.0
```

Build iOS only:
```bash
dotnet build src/Core/Core.csproj -c Release -f net9.0-ios
```

## Tests

### Unit tests (run anywhere)
```bash
dotnet test tests/Plugin.Firebase.UnitTests/Plugin.Firebase.UnitTests.csproj -c Release
```

### Integration tests (device-only)
Integration tests require a real device and local Firebase config files (not committed):
- `GoogleService-Info.plist` (iOS)
- `google-services.json` (Android)

Run:
```bash
dotnet test tests/Plugin.Firebase.IntegrationTests/Plugin.Firebase.IntegrationTests.csproj --no-build
```

## Formatting
```bash
dotnet format Plugin.Firebase.sln
```

Formatting is required before every PR.

## Troubleshooting

### Workloads missing (`NETSDK1147`)
Symptoms:
- “To build this project, the following workloads must be installed: ios/android”

Fix:
```bash
dotnet workload install ios maccatalyst maui --version "$(dotnet --version)"
```

### Packs missing / mismatched (often `CS1705`)
Symptoms:
- `CS1705` mentioning `Microsoft.iOS, Version=...` mismatches

Fix (repair path):
```bash
dotnet workload config --update-mode workload-set
dotnet workload update --from-previous-sdk --version "$(dotnet --version)"
dotnet workload install ios maccatalyst maui --version "$(dotnet --version)"
```

### Xcode / CLT / xcode-select
Symptoms:
- `xcodebuild` not found
- `xcode-select -p` points to an unexpected path

Fix:
```bash
xcodebuild -version
xcode-select -p
sudo xcode-select -s /Applications/Xcode.app/Contents/Developer
```

## Glossary
If you need the deeper explanation of “SDK vs workloads vs manifests vs packs vs TFM”, see `docs/toolchain-glossary.md`.
