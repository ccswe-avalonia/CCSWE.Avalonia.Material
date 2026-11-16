# =============================================================================
# Font acquisition for CCSWE.Avalonia.Material  (run from this Assets/Fonts/ folder)
# PowerShell equivalent of fetch-fonts.sh.
# =============================================================================
# Downloads the OFL TTFs for every family Fonts.axaml references, plus each
# OFL.txt, from google/fonts: the variable font where Google publishes one,
# otherwise the static cuts. Upright only. Re-run to refresh.
$ErrorActionPreference = "Stop"
$base = "https://github.com/google/fonts/raw/main/ofl"

Write-Host "Fetching Plus Jakarta Sans..."
Invoke-WebRequest "$base/plusjakartasans/PlusJakartaSans%5Bwght%5D.ttf" -OutFile "PlusJakartaSans[wght].ttf"
Invoke-WebRequest "$base/plusjakartasans/OFL.txt"                       -OutFile "PlusJakartaSans-OFL.txt"

Write-Host "Fetching DM Sans..."
Invoke-WebRequest "$base/dmsans/DMSans%5Bopsz,wght%5D.ttf" -OutFile "DMSans[opsz,wght].ttf"
Invoke-WebRequest "$base/dmsans/OFL.txt"                   -OutFile "DMSans-OFL.txt"

Write-Host "Fetching DM Mono..."
Invoke-WebRequest "$base/dmmono/DMMono-Regular.ttf" -OutFile "DMMono-Regular.ttf"
Invoke-WebRequest "$base/dmmono/DMMono-Medium.ttf"  -OutFile "DMMono-Medium.ttf"
Invoke-WebRequest "$base/dmmono/OFL.txt"            -OutFile "DMMono-OFL.txt"

Write-Host "Fetching JetBrains Mono..."
Invoke-WebRequest "$base/jetbrainsmono/JetBrainsMono%5Bwght%5D.ttf" -OutFile "JetBrainsMono[wght].ttf"
Invoke-WebRequest "$base/jetbrainsmono/OFL.txt"                     -OutFile "JetBrainsMono-OFL.txt"

Write-Host "Done. Keep the *-OFL.txt files in the repo to satisfy the OFL redistribution clause."
