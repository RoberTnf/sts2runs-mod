# STS2RunsMod

Lightweight Slay the Spire 2 mod that exports completed runs and uploads them to an STS2Runs-compatible server.

## Quickstart

Build and publish in two steps:

```bash
task build-release
task upload-release
```

Output:

- `dist/STS2RunsMod-v<version>.zip`
- Zip contents: `STS2RunsMod/mod_manifest.json`, `STS2RunsMod/STS2RunsMod.dll`, and `STS2RunsMod/installation_instructions.txt`

Upload behavior:

- GitHub release tag: `v<version>` (created automatically if missing)
- `task upload-release` uploads `dist/STS2RunsMod-v<version>.zip`

Prerequisite for upload:

```bash
gh auth login
```

For a local Linux install shortcut (build + copy into your default Steam path):

```bash
./install.sh
```

If you are coming from My Runs, install or update from the [GitHub releases page](https://github.com/RoberTnf/sts2runs-mod/releases).

## Install

1. Build the release zip with `task build-release`.
2. Extract it so you have a folder named `STS2RunsMod`.
3. Copy that folder to `<Slay the Spire 2 install dir>/mods/`.

Resulting path should be:

- `<Slay the Spire 2 install dir>/mods/STS2RunsMod/mod_manifest.json`
- `<Slay the Spire 2 install dir>/mods/STS2RunsMod/STS2RunsMod.dll`

Common Steam install locations:

- Linux: `~/.local/share/Steam/steamapps/common/Slay the Spire 2`
- macOS: `~/Library/Application Support/Steam/steamapps/common/Slay the Spire 2`
- Windows: `C:\Program Files (x86)\Steam\steamapps\common\Slay the Spire 2`

If your library is on another drive, open Steam -> Slay the Spire 2 -> Properties -> Installed Files -> Browse to get the exact install directory.

The mod writes `config.cfg` next to the installed DLL on first run.

## Config

`config.cfg` fields:

- `ServerUrl`: Base URL for the API (example: `https://your-server.example`). Use HTTPS for any non-localhost endpoint.
- `Enabled`: `true` to enable upload/auth behavior, `false` to disable all network activity.

## Privacy / Data Sent

- Auth: Steam ID + Steamworks auth session ticket (`/api/auth/mod`).
- Upload: run JSON payload (base64), SHA-256 hash, and profile number (`/api/runs/upload`).
- No upload is attempted when gameplay-affecting mods are detected.

## Troubleshooting

- `Auth unavailable` in logs: Steamworks ticket could not be acquired (Steam not running, no session, or Steamworks unavailable).
- `Auth request rejected`: check `ServerUrl`, server availability, and endpoint compatibility.
- Upload failures after auth: verify server API routes and that the server accepts mod-issued bearer tokens.

## License

MIT. See `LICENSE`.
