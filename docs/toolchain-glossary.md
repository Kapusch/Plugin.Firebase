# Toolchain glossary (SDK vs workloads vs manifests vs packs vs TFM)

This document explains why “mobile builds” can break when .NET SDK versions, workloads/manifests, packs, and Xcode get out of sync.

## Glossary

### .NET SDK version (ex: `9.0.306`, `10.0.100`)
The SDK is the `dotnet` toolchain that runs MSBuild and selects workloads/packs.

Check:
```bash
dotnet --version
```

### Workload version (ex: `9.0.306`, `10.0.102`)
The workload “version” is the bundle of workload components used for a given SDK.

Check:
```bash
dotnet workload --version
dotnet workload --info
```

### Workload manifests (ex: `maui 9.0.111/9.0.100` or `10.0.1/10.0.100`)
A manifest is the metadata that defines what a workload installs (packs, templates, etc.).

You’ll see manifest versions in:
```bash
dotnet workload --info
```

### Packs (Apple packs: `Microsoft.iOS.*` under `<DOTNET_ROOT>/packs`)
Packs are the reference assemblies + runtimes + SDK bits used by the Apple toolchain.

Common location (Homebrew install):
```
/usr/local/share/dotnet/packs
```

Key Apple packs:
- `Microsoft.iOS.Sdk.*` (targets/toolchain pack)
- `Microsoft.iOS.Ref.*` (reference assemblies used at compile time)
- `Microsoft.iOS.Runtime.*` (runtime bits)

### TFM (Target Framework Moniker)
This repo targets (examples):
- `net9.0` (no mobile toolchain required)
- `net9.0-ios` (iOS toolchain + packs + Xcode required)
- `net9.0-android` (Android toolchain required)

TFM with an explicit platform version (example): `net9.0-ios18.0`
- Same base .NET version (`net9.0`) but **pins the iOS target platform version** (API surface / analyzers).
- It does **not** install workloads for you. It only changes compile-time targeting.

## Relationship (text diagram)
```
.NET SDK (dotnet)        -> selects/uses -> workloads + manifests
workloads + manifests    -> install      -> packs under <DOTNET_ROOT>/packs
TFM (net9.0-ios...)      -> consumes     -> packs + Xcode toolchain to compile/link
```

