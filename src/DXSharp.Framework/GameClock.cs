namespace DXSharp.Framework;

/// <summary>Simulation time and interpolation for one rendered frame.</summary>
/// <param name="Total">Total simulated time, excluding pauses and discarded catch-up time.</param>
/// <param name="Elapsed">Duration of a simulation update.</param>
/// <param name="Interpolation">Fraction between the previous and current fixed simulation states.</param>
public readonly record struct GameTime(TimeSpan Total, TimeSpan Elapsed, double Interpolation);

/// <summary>An instance-owned fixed-step accumulator driven by a host's monotonic elapsed time.</summary>
/// <remarks>Advance on one thread. Simulation owns its previous/current states; render interpolates them. Pause does not accumulate catch-up work.</remarks>
public sealed class GameClock {
    private TimeSpan _remainder, _total;
    /// <summary>The immutable simulation step.</summary>
    public TimeSpan Step { get; }
    /// <summary>Maximum simulation updates accepted per rendered frame.</summary>
    public int MaximumUpdates { get; }
    /// <summary>Whether simulation updates are suspended.</summary>
    public bool Paused { get; set; }
    /// <summary>Elapsed host time deliberately discarded to bound simulation catch-up.</summary>
    public TimeSpan DroppedTime { get; private set; }
    /// <summary>Creates a clock; the default step is approximately 1/60 second and at most eight updates per frame.</summary>
    public GameClock(TimeSpan? step = null, int maximumUpdates = 8) {
        Step = step ?? TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        if (Step <= TimeSpan.Zero || maximumUpdates <= 0) throw new ArgumentOutOfRangeException(nameof(step));
        MaximumUpdates = maximumUpdates;
    }
    /// <summary>Consumes host elapsed time, performs bounded updates and returns render interpolation.</summary>
    public GameTime Advance(TimeSpan elapsed, Action<GameTime> update) {
        ArgumentNullException.ThrowIfNull(update);
        if (elapsed < TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(elapsed));
        if (Paused) return new(_total, TimeSpan.Zero, _remainder.TotalSeconds / Step.TotalSeconds);
        long capacity = checked(Step.Ticks * MaximumUpdates);
        if (elapsed.Ticks > capacity) { DroppedTime += TimeSpan.FromTicks(elapsed.Ticks - capacity); elapsed = TimeSpan.FromTicks(capacity); }
        _remainder += elapsed;
        while (_remainder >= Step) { _remainder -= Step; _total += Step; update(new(_total, Step, 0)); }
        return new(_total, elapsed, _remainder.TotalSeconds / Step.TotalSeconds);
    }
    /// <summary>Clears accumulated simulation, interpolation and dropped-time counters.</summary>
    public void Reset() { _remainder = _total = DroppedTime = TimeSpan.Zero; }
}
