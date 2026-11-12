# Add your music to Fleet Battler

1. Copy a track such as `theme.ogg` into `samples/FleetBattler/Music/`. For an already built copy, use the `Music` folder beside `FleetBattler.exe`.
2. Open `jukebox.json` in that same folder. Change the `system` list from `["@survey"]` to `["theme.ogg"]`. Keep the other entries as shipped.
3. Rebuild if you edited the source folder, then launch the game. Fleet Battler reads the settings at startup.

The theme keeps playing when you enter combat, win, or open the galaxy map. Add more filenames to the list to play them in order. Paths are relative to `Music`, so `["Ambient/theme.ogg", "patrol.wav"]` works if those files are present.

| Playlist | When it takes over if you give it its own track |
| --- | --- |
| `system` | Routine system operations |
| `galaxy` | Galaxy map |
| `contested` | A contested relay or active combat |
| `victory` | The system is won |

Tracks beginning with `@` are the bundled procedural cues. With a custom system playlist, untouched contextual lists inherit it. Set `"useProceduralTransitions": true` to retain the stock cue changes. If an explicit playlist fails to play, Fleet Battler logs the failure and falls back to a procedural cue. Use Ogg Vorbis, MP3, or PCM WAV. Write `"theme.ogg"` without a leading `@`. The sample decodes mono or stereo Vorbis on a background thread into a local PCM cache on first use. Subsequent plays of the same file reuse that cache. Export Vorbis when encoding an Ogg container. The Ogg playback path supports Vorbis audio.

`musicVolume` and `effectsVolume` run from 0 to 1. Press **M** to toggle audio or start with `--mute`.
