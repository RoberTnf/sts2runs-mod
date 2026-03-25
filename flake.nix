{
  inputs.nixpkgs.url = "github:NixOS/nixpkgs/nixpkgs-unstable";
  outputs = { nixpkgs, ... }:
    let pkgs = nixpkgs.legacyPackages.x86_64-linux;
    in {
      devShells.x86_64-linux.default = pkgs.mkShell {
        packages = [ pkgs.dotnet-sdk_9 pkgs.gh ];
        shellHook = ''
          export STS2_GAME_DIR="$HOME/.local/share/Steam/steamapps/common/Slay the Spire 2/data_sts2_linuxbsd_x86_64"
        '';
      };
    };
}
