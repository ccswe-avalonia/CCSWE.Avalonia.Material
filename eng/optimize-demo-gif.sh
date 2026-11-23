#!/usr/bin/env bash
#
# Optimize a raw capture of the demo gallery tour into the README GIF. On Linux,
# eng/record-demo-gif.sh records the tour and calls this script; elsewhere:
#
# Capture the tour with:  dotnet run --project src/CCSWE.Avalonia.Material.Demo -- --record --exit
# then run:               eng/optimize-demo-gif.sh <capture.gif> [output.gif]
#
# Output defaults to docs/images/demo.gif (the path the root README links). Tune size/quality with
# the SCALE / COLORS / LOSSY environment variables.
#
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

IN="${1:?usage: optimize-demo-gif.sh <capture.gif> [output.gif]}"
OUT="${2:-$REPO_ROOT/docs/images/demo.gif}"

SCALE="${SCALE:-0.58}"    # width factor: 0.58 suits a 1377px-wide capture (~800px); 0.7265625 takes 1024px to 744px
COLORS="${COLORS:-80}"    # palette size
LOSSY="${LOSSY:-120}"     # higher = smaller + lossier

if ! command -v gifsicle >/dev/null 2>&1; then
    echo "error: gifsicle not found. Install it with: sudo apt-get install -y gifsicle" >&2
    exit 1
fi

mkdir -p "$(dirname "$OUT")"
gifsicle -O3 --lossy="$LOSSY" --colors "$COLORS" --scale "$SCALE" "$IN" -o "$OUT"

echo "in:  $(du -h "$IN" | cut -f1)	$IN"
echo "out: $(du -h "$OUT" | cut -f1)	$OUT"
