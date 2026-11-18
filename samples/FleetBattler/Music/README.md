# Add your music to Fleet Battler

1. Put your tracks in the `Music` folder beside `FleetBattler.exe`. For a source build, put them in `samples/FleetBattler/Music/` and rebuild. Subfolders are copied with the sample.
2. Edit `jukebox.json` in that folder. Use paths relative to `Music`, including each file's extension. Replace these example filenames with your own. Use forward slashes in JSON paths.
3. Restart the game. It reads the file at startup.

```json
{
  "musicVolume": 0.35,
  "effectsVolume": 0.55,
  "shuffle": true,
  "useProceduralTransitions": false,
  "playlists": {
    "system": ["Exploration/Sol.ogg", "Exploration/Quiet Orbit.ogg", "Exploration/Blue Hours.ogg"],
    "galaxy": ["Galaxy/Deep Sky.ogg", "Galaxy/Parallax.ogg"],
    "contested": ["Combat/Intercept.ogg", "Combat/Long Burn.ogg"],
    "victory": ["Victory/Homeward.ogg", "Victory/Safe Harbor.ogg"]
  }
}
```

| Playlist | When it plays |
| --- | --- |
| `system` | Routine system operations |
| `galaxy` | Galaxy map |
| `contested` | A contested relay or active combat in the current system |
| `victory` | One celebration track the first time the player is in a newly secured system outside combat. Playback then returns to `system` |

With `"shuffle": true`, each list shuffles its distinct tracks and cycles through them before reshuffling. A different playlist starts its next track when the situation changes. Victory uses one track even if the list contains several. Revisiting a secured system or reopening the map does not replay its celebration. If the victory list inherits the system playlist, music continues uninterrupted. Returning to a list picks up at its next track, and contexts with the same track list keep the current song playing. Set `"shuffle": false` to play the lists in written order.

To use one large library everywhere, define only `system` and leave `"useProceduralTransitions": false`. Unspecified contexts inherit that library. Tracks beginning with `@` are bundled procedural cues. Enable `"useProceduralTransitions": true` to retain default cues in contexts without a custom list. If every track in a regular playlist fails, the game logs the errors and uses a bundled cue. If every victory track fails, the game returns to the system playlist.

Use Ogg Vorbis, MP3, or PCM WAV. Export Vorbis when encoding an Ogg container. The sample decodes mono or stereo Vorbis on a background thread into a local PCM cache on first use and reuses that cache. `musicVolume` and `effectsVolume` run from 0 to 1. Press **M** to toggle audio or start with `--mute`.
