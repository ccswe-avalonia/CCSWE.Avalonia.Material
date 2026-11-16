# PipsPager: distinguish the selected pip by shape

Resting pips (`Controls/PipsPager.axaml`, `OutlineVariant`) miss 3:1 non-text contrast on `Surface` in Dark and Light. `Outline` passes, but against the `Primary` selected pip it differs by hue only, and hover already uses `Outline`.

1. Make the selected pip a pill (wider than the resting dot), with a width transition on `MotionDurationShort4` / `MotionEasingStandard`.
2. Move resting pips to `Outline` and hover to `OnSurfaceVariant`.
3. Check every variant in the Demo (Navigation page) and update the `Base/MIGRATION.md` PipsPager row.
