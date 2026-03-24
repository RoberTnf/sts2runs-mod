# STS2RunsMod

Lightweight Slay the Spire 2 mod that exports completed runs and uploads them to an STS2Runs-compatible server.

## Quickstart

Build:

```bash
dotnet build -c Release
```

Install (Linux default path):

```bash
./install.sh
```

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
