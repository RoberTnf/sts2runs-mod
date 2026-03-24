#!/usr/bin/env bash
set -euo pipefail

GAME_DIR=~/.local/share/Steam/steamapps/common/Slay\ the\ Spire\ 2
MOD_DIR="$GAME_DIR/mods/STS2RunsMod"

nix develop --command bash -c 'dotnet build -c Release'

mkdir -p "$MOD_DIR"

# Remove stale config so it regenerates with current defaults
rm -f "$MOD_DIR/config.cfg"

cp mod_manifest.json "$MOD_DIR/"
cp .godot/mono/temp/bin/Release/STS2RunsMod.dll "$MOD_DIR/"

echo "Installed to $MOD_DIR"
