# Fleet Battler jukebox

Put your own tracks in this folder and list them in `jukebox.json`. Paths are relative to this folder and can include subdirectories. Each playlist plays in order and repeats. The Windows media player handles file decoding.

| Playlist | Plays when |
| --- | --- |
| `system` | Viewing a system during routine operations |
| `galaxy` | Viewing the galaxy map |
| `contested` | Both factions have a presence at the mining beacon |
| `victory` | Both reinforcement wings have launched |

The `@` entries name original procedural tracks generated as PCM WAV files on first use. For example, change the system list to `["survey.mp3", "patrol.wav", "@survey"]` and copy the named files here. Set music and effects volume independently from 0 to 1. The sample copies this directory to its build and publish outputs.

Press **M** to toggle audio in the game. Run with `--mute` to start silent.
