#!/usr/bin/env bash
#
# Record the README demo GIF in one step (Linux): build the Demo, run its hands-free tour
# (`--record --exit`) on a private Xvfb display, record the window with ffmpeg, cut the recording
# at the tour's start/end markers, and optimize it with eng/optimize-demo-gif.sh.
#
# Usage:     eng/record-demo-gif.sh [output.gif]     (default: docs/images/demo.gif)
# Requires:  Xvfb, xdotool, ffmpeg (ffprobe), gifsicle, dotnet, GNU coreutils
#            (sudo apt-get install -y xvfb xdotool ffmpeg gifsicle)
#
# Elsewhere, capture the `--record --exit` tour with a screen recorder (e.g. ShareX) and run
# eng/optimize-demo-gif.sh on it.
#
set -euo pipefail

REPO_ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
OUT="${1:-}"
WIDTH_OUT=744   # the README's GIF width

missing=()
for tool in Xvfb xdotool ffmpeg ffprobe gifsicle dotnet timeout awk; do
  command -v "$tool" > /dev/null 2>&1 || missing+=("$tool")
done
if [ ${#missing[@]} -gt 0 ]; then
  echo "error: missing ${missing[*]}. Install with: sudo apt-get install -y xvfb xdotool ffmpeg gifsicle" >&2
  exit 1
fi

tmp="$(mktemp -d)"
xvfb_pid="" demo_pid="" ff_pid=""
cleanup() {
  for pid in "$ff_pid" "$demo_pid" "$xvfb_pid"; do
    if [ -n "$pid" ] && kill -0 "$pid" 2> /dev/null; then
      kill "$pid" 2> /dev/null || true
      wait "$pid" 2> /dev/null || true
    fi
  done
  rm -rf "$tmp"
}
trap cleanup EXIT

fail() {
  echo "error: $1" >&2
  for log in demo ffmpeg xvfb; do
    [ -s "$tmp/$log.log" ] && { echo "--- $log.log (tail)" >&2; tail -n 20 "$tmp/$log.log" >&2; }
  done
  exit 1
}

echo "Building the Demo..."
dotnet build "$REPO_ROOT/src/CCSWE.Avalonia.Material.Demo" -c Release -v q --nologo > "$tmp/build.log" 2>&1 ||
  { cat "$tmp/build.log" >&2; exit 1; }
dll="$(find "$REPO_ROOT/src/CCSWE.Avalonia.Material.Demo/bin/Release" -name CCSWE.Avalonia.Material.Demo.dll | head -n 1)"
[ -n "$dll" ] || fail "Demo build output not found"

# A private display: Xvfb picks a free number and writes it to fd 3 once it accepts connections.
Xvfb -displayfd 3 -screen 0 1280x900x24 -nolisten tcp 3> "$tmp/display" 2> "$tmp/xvfb.log" &
xvfb_pid=$!
for _ in $(seq 1 100); do
  [ -s "$tmp/display" ] && break
  kill -0 "$xvfb_pid" 2> /dev/null || fail "Xvfb exited"
  sleep 0.1
done
[ -s "$tmp/display" ] || fail "Xvfb didn't start"
DISPLAY=":$(tr -d '[:space:]' < "$tmp/display")"
export DISPLAY

# Keep the pointer off the window (hover states); the capture also hides the cursor.
xdotool mousemove 1270 890

echo "Running the tour on $DISPLAY..."
AVALONIA_GLOBAL_SCALE_FACTOR=1 timeout 5m dotnet "$dll" --record --exit --start-delay 5 > "$tmp/demo.log" 2>&1 &
demo_pid=$!

wid="$(timeout 60 xdotool search --sync --onlyvisible --name 'CCSWE\.Avalonia\.Material\.Demo' 2> /dev/null | head -n 1)" ||
  true
[ -n "$wid" ] || fail "the Demo window didn't appear"
eval "$(xdotool getwindowgeometry --shell "$wid")"

# Wall-clock timestamps let the tour's markers be mapped onto the recording.
ffmpeg -nostdin -loglevel info -use_wallclock_as_timestamps 1 -f x11grab -draw_mouse 0 -framerate 15 \
  -video_size "${WIDTH}x${HEIGHT}" -i "$DISPLAY.0+$X,$Y" -c:v libx264 -preset ultrafast -qp 0 \
  "$tmp/tour.mkv" 2> "$tmp/ffmpeg.log" &
ff_pid=$!

status=0
wait "$demo_pid" || status=$?
demo_pid=""
[ "$status" -ne 124 ] || fail "the tour didn't finish within 5 minutes"
[ "$status" -eq 0 ] || fail "the Demo exited with $status"

kill -INT "$ff_pid" 2> /dev/null || true
wait "$ff_pid" || true   # 255 after a signal
ff_pid=""
if [ ! -s "$tmp/tour.mkv" ] || ! ffprobe -v error "$tmp/tour.mkv"; then
  fail "no usable recording"
fi

t0="$(grep -m 1 -oE 'start: [0-9.]+' "$tmp/ffmpeg.log" | cut -d' ' -f2)" || true
ts="$(grep -m 1 -oE '^tour: start [0-9]+' "$tmp/demo.log" | cut -d' ' -f3)" || true
te="$(grep -m 1 -oE '^tour: end [0-9]+' "$tmp/demo.log" | cut -d' ' -f3)" || true
[ -n "$t0" ] && [ -n "$ts" ] && [ -n "$te" ] || fail "missing recording start or tour markers"
read -r start end < <(awk -v t0="$t0" -v ts="$ts" -v te="$te" 'BEGIN { printf "%.3f %.3f\n", ts / 1000 - t0, te / 1000 - t0 }')
awk -v s="$start" 'BEGIN { exit !(s >= 0) }' || fail "the tour started before the recording (start $start s)"
echo "Tour: ${start}s to ${end}s of the recording"

ffmpeg -nostdin -loglevel error -y -ss "$start" -to "$end" -i "$tmp/tour.mkv" \
  -vf "fps=5,split[a][b];[a]palettegen=max_colors=128:stats_mode=diff[p];[b][p]paletteuse=dither=none:diff_mode=rectangle" \
  "$tmp/raw.gif"

scale="$(awk -v w="$WIDTH" -v o="$WIDTH_OUT" 'BEGIN { printf "%.7f", o / w }')"
SCALE="$scale" bash "$REPO_ROOT/eng/optimize-demo-gif.sh" "$tmp/raw.gif" ${OUT:+"$OUT"}

if command -v identify > /dev/null 2>&1; then
  result="${OUT:-$REPO_ROOT/docs/images/demo.gif}"
  identify -format "%w x %h, %n frames\n" "$result" | head -n 1
fi
