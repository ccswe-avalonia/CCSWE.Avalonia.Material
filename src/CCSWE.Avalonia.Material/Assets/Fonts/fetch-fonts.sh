#!/usr/bin/env bash
# =============================================================================
# Font acquisition for CCSWE.Avalonia.Material  (run from this Assets/Fonts/ folder)
# =============================================================================
# Downloads the OFL TTFs for every family Fonts.axaml references, plus each
# OFL.txt, from google/fonts: the variable font where Google publishes one,
# otherwise the static cuts. Upright only. Re-run to refresh.
set -euo pipefail

BASE="https://github.com/google/fonts/raw/main/ofl"

echo "Fetching Plus Jakarta Sans..."
curl -fL "$BASE/plusjakartasans/PlusJakartaSans%5Bwght%5D.ttf" -o "PlusJakartaSans[wght].ttf"
curl -fL "$BASE/plusjakartasans/OFL.txt"                       -o "PlusJakartaSans-OFL.txt"

echo "Fetching DM Sans..."
curl -fL "$BASE/dmsans/DMSans%5Bopsz,wght%5D.ttf" -o "DMSans[opsz,wght].ttf"
curl -fL "$BASE/dmsans/OFL.txt"                   -o "DMSans-OFL.txt"

echo "Fetching DM Mono..."
curl -fL "$BASE/dmmono/DMMono-Regular.ttf" -o "DMMono-Regular.ttf"
curl -fL "$BASE/dmmono/DMMono-Medium.ttf"  -o "DMMono-Medium.ttf"
curl -fL "$BASE/dmmono/OFL.txt"            -o "DMMono-OFL.txt"

echo "Fetching JetBrains Mono..."
curl -fL "$BASE/jetbrainsmono/JetBrainsMono%5Bwght%5D.ttf" -o "JetBrainsMono[wght].ttf"
curl -fL "$BASE/jetbrainsmono/OFL.txt"                     -o "JetBrainsMono-OFL.txt"

echo "Done. Keep the *-OFL.txt files in the repo to satisfy the OFL redistribution clause."
