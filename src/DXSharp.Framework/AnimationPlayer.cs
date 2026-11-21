using System.Numerics;
using DXSharp.Assets;

namespace DXSharp.Framework;

/// <summary>Single-threaded clip playback with allocation-free sampling, interruptible cross-fades and explicit root motion.</summary>
public sealed class AnimationPlayer {
    private readonly SkinnedModel _model;
    private readonly SkeletonPose _current, _source;
    private readonly int _root;
    private int _clip;
    private double _time, _blendElapsed, _blendDuration;
    private float _speed = 1;
    private bool _loop = true;
    /// <summary>Whether playback time is suspended.</summary>
    public bool Paused { get; set; }
    /// <summary>Playback speed; must be finite and nonnegative.</summary>
    public float Speed { get => _speed; set { if (!float.IsFinite(value) || value < 0) throw new ArgumentOutOfRangeException(nameof(value)); _speed = value; } }
    /// <summary>Whether root translation is removed from the pose and reported through RootMotionDelta.</summary>
    public bool ExtractRootMotion { get; set; }
    /// <summary>Axes removed from the skeleton and optionally extracted; defaults to horizontal XZ motion so jumps retain vertical movement.</summary>
    public Vector3 RootMotionAxes { get; set; } = new(1,0,1);
    /// <summary>Translation produced by the most recent update, including displacement across clip loop boundaries.</summary>
    /// <remarks>The delta follows the current clip; root-velocity blending and root rotation are outside this first playback contract.</remarks>
    public Vector3 RootMotionDelta { get; private set; }
    /// <summary>Name of the current clip.</summary>
    public string CurrentClipName => _model.Animations[_clip].Name;
    /// <summary>Whether a non-looping clip has reached its final key time.</summary>
    public bool Completed => !_loop && _time >= _model.Animations[_clip].Duration;
    /// <summary>Creates playback for a model with at least one clip and an optional root-translation node.</summary>
    public AnimationPlayer(SkinnedModel model, int rootMotionNode = -1) {
        ArgumentNullException.ThrowIfNull(model);
        if (model.Animations.Length == 0) throw new ArgumentException("The model has no animation clips.", nameof(model));
        if (rootMotionNode < -1 || rootMotionNode >= model.Nodes.Length) throw new ArgumentOutOfRangeException(nameof(rootMotionNode));
        _model = model; _root = rootMotionNode;
        _current = new(model.Nodes); _source = new(model.Nodes); _clip = model.FindAnimation("Idle");
    }
    /// <summary>Starts a clip. Interrupted fades begin at the currently visible pose; non-looping clips hold their final pose.</summary>
    public void Play(int clip, double blendSeconds = 0.25, bool loop = true) {
        if ((uint)clip >= _model.Animations.Length) throw new ArgumentOutOfRangeException(nameof(clip));
        if (!double.IsFinite(blendSeconds) || blendSeconds < 0) throw new ArgumentOutOfRangeException(nameof(blendSeconds));
        Sample(_source);
        _clip = clip; _time = 0; _blendElapsed = 0; _blendDuration = blendSeconds; _loop = loop; RootMotionDelta = default;
    }
    /// <summary>Advances playback in seconds. Sample independently at each render time; no deformation-rate cap is imposed.</summary>
    public void Update(double elapsedSeconds) {
        if (!double.IsFinite(elapsedSeconds) || elapsedSeconds < 0) throw new ArgumentOutOfRangeException(nameof(elapsedSeconds));
        RootMotionDelta = default;
        if (Paused) return;
        double next = _time + elapsedSeconds * _speed;
        if (!double.IsFinite(next)) throw new ArgumentOutOfRangeException(nameof(elapsedSeconds));
        if (!_loop) next = Math.Min(next, _model.Animations[_clip].Duration);
        if (ExtractRootMotion) RootMotionDelta = RootPosition(next) - RootPosition(_time);
        _time = next; _blendElapsed = Math.Min(_blendDuration, _blendElapsed + elapsedSeconds);
    }
    /// <summary>Samples local TRS values into a matching caller-owned pose without allocating.</summary>
    public void Sample(SkeletonPose destination) {
        ArgumentNullException.ThrowIfNull(destination);
        if (destination.Translation.Length != _model.Nodes.Length) throw new ArgumentException("Pose does not match the skeleton.", nameof(destination));
        SampleClip(_model.Animations[_clip], ClipTime(_time), _current);
        if (_root >= 0 && _model.Animations[_clip].Tracks.TryGetValue(_root, out var rootTrack) && rootTrack.Translation is { } rootTranslation) {
            var start = rootTranslation.Sample(0);
            _current.Translation[_root] -= (_current.Translation[_root] - new Vector3(start.X,start.Y,start.Z)) * RootMotionAxes;
        }
        float amount = _blendDuration <= 0 ? 1 : (float)Math.Clamp(_blendElapsed / _blendDuration, 0, 1);
        for (int i = 0; i < _model.Nodes.Length; ++i) {
            destination.Translation[i] = Vector3.Lerp(_source.Translation[i], _current.Translation[i], amount);
            destination.Scale[i] = Vector3.Lerp(_source.Scale[i], _current.Scale[i], amount);
            destination.Rotation[i] = Quaternion.Normalize(Quaternion.Slerp(_source.Rotation[i], _current.Rotation[i], amount));
        }
    }
    private float ClipTime(double time) {
        float duration = _model.Animations[_clip].Duration;
        return duration <= 0 ? 0 : (float)(_loop ? time % duration : Math.Min(time, duration));
    }
    private Vector3 RootPosition(double time) {
        var clip = _model.Animations[_clip];
        if (_root < 0 || !clip.Tracks.TryGetValue(_root, out var track) || track.Translation is null) return default;
        Vector4 p = track.Translation.Sample(ClipTime(time));
        Vector4 start = track.Translation.Sample(0), end = track.Translation.Sample(clip.Duration);
        double loops = _loop && clip.Duration > 0 ? Math.Floor(time / clip.Duration) : 0;
        return (new Vector3(p.X,p.Y,p.Z) + (float)loops * new Vector3(end.X-start.X,end.Y-start.Y,end.Z-start.Z)) * RootMotionAxes;
    }
    private void SampleClip(AnimationClip clip, float time, SkeletonPose pose) {
        for (int i = 0; i < _model.Nodes.Length; ++i) {
            pose.Translation[i] = _model.Nodes[i].Translation; pose.Rotation[i] = _model.Nodes[i].Rotation; pose.Scale[i] = _model.Nodes[i].Scale;
        }
        foreach (var (node, track) in clip.Tracks) {
            if (track.Translation is { } t) { var p=t.Sample(time); pose.Translation[node]=new(p.X,p.Y,p.Z); }
            if (track.Scale is { } s) { var p=s.Sample(time); pose.Scale[node]=new(p.X,p.Y,p.Z); }
            if (track.Rotation is { } r) { var p=r.Sample(time); pose.Rotation[node]=Quaternion.Normalize(new(p.X,p.Y,p.Z,p.W)); }
        }
    }
}
