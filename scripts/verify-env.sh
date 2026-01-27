#!/usr/bin/env bash
set -euo pipefail

repo_root="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
cd "$repo_root"

fail() {
  echo "KO: $*" >&2
  exit 1
}

warn() {
  echo "WARN: $*" >&2
}

reasons=()
next_steps=()

add_reason() {
  reasons+=("$1")
}

add_next() {
  next_steps+=("$1")
}

echo "Plugin.Firebase - environment verification (macOS)"
echo

uname_s="$(uname -s)"
if [[ "$uname_s" != "Darwin" ]]; then
  warn "This script is intended for macOS. Detected: $uname_s"
fi

echo "== OS =="
sw_vers || true
echo "arch: $(uname -m)"
echo

if ! command -v dotnet >/dev/null; then
  add_reason "dotnet not found on PATH"
  add_next "Install the .NET SDK required by global.json (see BUILDING.md)"
else
  echo "== .NET =="
  sdk_version="$(dotnet --version 2>/dev/null || true)"

  pinned_sdk=""
  if [[ -f global.json ]]; then
    if command -v python3 >/dev/null; then
      pinned_sdk="$(python3 -c 'import json; print(json.load(open("global.json"))["sdk"]["version"])' 2>/dev/null || true)"
    else
      pinned_sdk="$(sed -n 's/.*\"version\"[[:space:]]*:[[:space:]]*\"\\([^\"]*\\)\".*/\\1/p' global.json 2>/dev/null || true)"
      pinned_sdk="${pinned_sdk%%$'\n'*}"
    fi
  fi

  if [[ -z "$sdk_version" ]]; then
    if [[ -n "$pinned_sdk" ]]; then
      add_reason "dotnet failed to run with this repo's global.json pinned SDK (${pinned_sdk})"
      add_next "Install .NET SDK ${pinned_sdk} (or temporarily move global.json out of the repo)"
    else
      add_reason "dotnet failed to run"
      add_next "Reinstall .NET SDK and retry"
    fi
  else
    echo "dotnet --version: ${sdk_version}"
    if [[ -n "$pinned_sdk" && "$sdk_version" != "$pinned_sdk" ]]; then
      warn "global.json pins SDK: ${pinned_sdk} (dotnet resolved: ${sdk_version})"
      add_reason "SDK mismatch: global.json pins ${pinned_sdk} but dotnet resolved ${sdk_version}"
      add_next "Install .NET SDK ${pinned_sdk} (recommended), then rerun this script"
    fi
  fi
  echo
fi

if command -v dotnet >/dev/null; then
  echo "dotnet --info (abridged):"
  dotnet --info | sed -n '1,90p' || true
  echo

  echo "dotnet workload --version:"
  dotnet workload --version || true
  echo

  echo "dotnet workload list:"
  dotnet workload list || true
  echo

  echo "dotnet workload --info (abridged):"
  dotnet workload --info | sed -n '1,160p' || true
  echo
fi

echo "== Xcode / CLT =="
if command -v xcodebuild >/dev/null; then
  xcodebuild -version || fail "xcodebuild exists but failed; ensure Xcode is installed and licensed"
else
  add_reason "xcodebuild not found (Xcode / Command Line Tools missing)"
  add_next "Install Xcode, then install Command Line Tools and re-run: xcodebuild -version"
fi

if command -v xcode-select >/dev/null; then
  xcode_path="$(xcode-select -p 2>/dev/null || true)"
  if [[ -n "$xcode_path" ]]; then
    echo "xcode-select -p: $xcode_path"
  else
    add_reason "xcode-select -p returned empty (CLT not installed or not configured)"
    add_next "Run: xcode-select -p (then configure with sudo xcode-select -s ... if needed)"
  fi
else
  add_reason "xcode-select not found (Command Line Tools missing)"
  add_next "Install Xcode Command Line Tools (CLT) and re-run: xcode-select -p"
fi
echo

echo "== iOS packs (Apple workloads) =="
dotnet_root=""
packs_dir=""

if command -v dotnet >/dev/null; then
  if [[ -n "${DOTNET_ROOT:-}" ]]; then
    dotnet_root="$DOTNET_ROOT"
  else
    # Prefer the SDK Base Path from dotnet --info. This is reliable even when `dotnet` is a shim on PATH.
    base_path="$(dotnet --info 2>/dev/null | sed -n 's/^ Base Path:[[:space:]]*//p' || true)"
    base_path="${base_path%%$'\n'*}"
    if [[ -n "$base_path" && -d "$base_path" ]]; then
      dotnet_root="$(cd "$base_path/../.." && pwd)"
    else
      dotnet_path="$(command -v dotnet)"
      dotnet_root="$(cd "$(dirname "$dotnet_path")/.." && pwd)"
    fi
  fi
  packs_dir="${dotnet_root}/packs"
fi

if [[ ! -d "$packs_dir" ]]; then
  add_reason "Could not find dotnet packs directory (expected <DOTNET_ROOT>/packs)"
  add_next "Check DOTNET_ROOT or install .NET SDK via the official installer/Homebrew, then re-run this script"
else
  echo "packs dir: $packs_dir"

  ios_packs=()
  while IFS= read -r line; do
    [[ -n "$line" ]] && ios_packs+=("$line")
  done < <(ls -1 "$packs_dir" 2>/dev/null | grep -E '^Microsoft\.iOS\.' || true)

  ref_packs=()
  while IFS= read -r line; do
    [[ -n "$line" ]] && ref_packs+=("$line")
  done < <(printf "%s\n" "${ios_packs[@]}" | grep -E '^Microsoft\.iOS\.Ref\.' || true)

  sdk_packs=()
  while IFS= read -r line; do
    [[ -n "$line" ]] && sdk_packs+=("$line")
  done < <(printf "%s\n" "${ios_packs[@]}" | grep -E '^Microsoft\.iOS\.Sdk\.' || true)

  if [[ "${#ios_packs[@]}" -eq 0 ]]; then
    add_reason "No Microsoft.iOS.* packs found under <DOTNET_ROOT>/packs"
    add_next "Install Apple workloads/packs (see BUILDING.md) then re-run this script"
  else
    echo "found Microsoft.iOS packs (sample):"
    printf "  - %s\n" "${ios_packs[@]:0:5}"
    echo
  fi

  if [[ "${#ref_packs[@]}" -eq 0 ]]; then
    add_reason "No Microsoft.iOS.Ref.* packs found (missing reference assemblies for compilation)"
    add_next "Repair workloads: dotnet workload install ios maccatalyst maui --version \"$(dotnet --version)\""
  fi

  if [[ "${#sdk_packs[@]}" -eq 0 ]]; then
    add_reason "No Microsoft.iOS.Sdk.* packs found (missing Apple SDK targets/toolchain pack)"
    add_next "Repair workloads: dotnet workload install ios maccatalyst maui --version \"$(dotnet --version)\""
  fi

  # Repo-specific expectation: target iOS TFM from src/Core/Core.csproj (net9.0-ios today).
  ios_tfm=""
  if [[ -f src/Core/Core.csproj ]]; then
    ios_tfm="$(grep -Eo 'net[0-9]+\.[0-9]+-ios([0-9]+\.[0-9]+)?' src/Core/Core.csproj | sed -n '1p' || true)"
    ios_tfm="${ios_tfm%%$'\n'*}"
  fi

  if [[ -n "$ios_tfm" ]]; then
    echo "repo iOS TFM (from src/Core/Core.csproj): $ios_tfm"

    net_part="${ios_tfm%%-*}" # net9.0
    if [[ "$net_part" =~ ^net([0-9]+)\.([0-9]+)$ ]]; then
      net_prefix="net${BASH_REMATCH[1]}.${BASH_REMATCH[2]}"
      expected_ref_re="^Microsoft\\.iOS\\.Ref\\.${net_prefix//./\\.}_"
      expected_sdk_re="^Microsoft\\.iOS\\.Sdk\\.${net_prefix//./\\.}_"

      if ! printf "%s\n" "${ref_packs[@]}" | grep -Eq "$expected_ref_re"; then
        add_reason "Missing Microsoft.iOS.Ref.${net_prefix}_* packs required to compile ${ios_tfm}"
        add_next "Install/repair Apple workloads for ${net_prefix}: dotnet workload install ios maccatalyst maui --version \"$(dotnet --version)\""
      fi

      if ! printf "%s\n" "${sdk_packs[@]}" | grep -Eq "$expected_sdk_re"; then
        add_reason "Missing Microsoft.iOS.Sdk.${net_prefix}_* packs required to compile ${ios_tfm}"
        add_next "Install/repair Apple workloads for ${net_prefix}: dotnet workload install ios maccatalyst maui --version \"$(dotnet --version)\""
      fi
    fi
    echo
  fi
fi
echo

status="HEALTHY"
if [[ "${#reasons[@]}" -gt 0 ]]; then
  status="UNHEALTHY"
fi

echo "== Summary =="
echo "STATUS=${status}"
if [[ -f global.json ]]; then
  if command -v python3 >/dev/null; then
    pinned_sdk="$(python3 -c 'import json; print(json.load(open("global.json"))["sdk"]["version"])' 2>/dev/null || true)"
  else
    pinned_sdk="$(sed -n 's/.*\"version\"[[:space:]]*:[[:space:]]*\"\\([^\"]*\\)\".*/\\1/p' global.json 2>/dev/null || true)"
    pinned_sdk="${pinned_sdk%%$'\n'*}"
  fi
  if [[ -n "$pinned_sdk" ]]; then
    echo "PINNED_SDK=${pinned_sdk}"
  fi
fi
if command -v dotnet >/dev/null; then
  echo "SDK_VERSION=$(dotnet --version 2>/dev/null || true)"
  echo "WORKLOAD_VERSION=$(dotnet workload --version 2>/dev/null || true)"
fi
if command -v xcodebuild >/dev/null; then
  echo "XCODE=$(xcodebuild -version 2>/dev/null | head -n 1 || true)"
fi
if [[ -n "${dotnet_root:-}" ]]; then
  echo "DOTNET_ROOT=${dotnet_root}"
fi
if [[ -n "${packs_dir:-}" ]]; then
  echo "PACKS_DIR=${packs_dir}"
fi
if [[ -n "${ios_tfm:-}" ]]; then
  echo "IOS_TFM=${ios_tfm}"
fi

if [[ "$status" == "UNHEALTHY" ]]; then
  for i in "${!reasons[@]}"; do
    n=$((i + 1))
    echo "REASON_${n}=${reasons[$i]}"
  done
  for i in "${!next_steps[@]}"; do
    n=$((i + 1))
    echo "NEXT_${n}=${next_steps[$i]}"
  done
  exit 1
fi

echo "OK: environment looks ready for iOS builds."
