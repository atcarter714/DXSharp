"""Regenerate original DXSharp test media with FFmpeg; no external footage is used."""
from pathlib import Path
import hashlib, json, subprocess
root = Path(__file__).resolve().parent
video = "testsrc2=size=320x180:rate=24:duration=6,drawbox=x=280:y=8:w=32:h=32:color=white:t=fill:enable='lt(mod(t,1),0.1)'"
audio = "sine=frequency=880:sample_rate=48000:duration=6"
subprocess.run(['ffmpeg','-hide_banner','-loglevel','error','-y','-f','lavfi','-i',video,'-f','lavfi','-i',audio,
 '-vf','format=yuv420p','-af',"volume='0.3*lt(mod(t,1),0.1)':eval=frame",'-c:v','libx264','-preset','slow','-crf','23',
 '-c:a','aac','-b:a','64k','-movflags','+faststart','-map_metadata','-1',str(root/'test-av.mp4')],check=True)
subprocess.run(['ffmpeg','-hide_banner','-loglevel','error','-y','-i',str(root/'test-av.mp4'),'-an','-c:v','copy',
 '-map_metadata','-1',str(root/'test-video.mp4')],check=True)
(root/'manifest.json').write_text(json.dumps({'origin':'Original FFmpeg-generated DXSharp test pattern; MIT licensed with this repository.',
 'width':320,'height':180,'framesPerSecond':24,'durationSeconds':6,'audio':'880 Hz pulses synchronized to the white flash; 48 kHz AAC stereo/mono as encoded',
 'files':{p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in root.glob('*.mp4')}},indent=2)+'\n')
