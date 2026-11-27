using DXSharp.Assets;
using DXSharp.Graphics;
using DXSharp.Samples;
using System.Numerics;

namespace FleetBattler;

internal readonly record struct HullModel( string File, float Scale, bool Flatten, Vector3 Nozzles ) {
    internal static HullModel[] Read( string[] args ) {
        using var data = FleetContent.Read( "models", "models" );
        var roles = Enum.GetValues<HullRole>( );
        var result = new HullModel[roles.Length];
        foreach ( var row in data.RootElement.GetProperty( "models" ).EnumerateArray( ) ) {
            var role = CatalogJson.EnumId<HullRole>( row.GetProperty( "id" ) );
            string file = row.GetProperty( "file" ).GetString( )!;
            float scale = CatalogJson.Number( row, "scale", 0.0001f, 1000, 1 );
            bool flatten = !row.TryGetProperty( "flatten", out var mode ) || mode.GetBoolean( );
            string option = role switch {
                HullRole.Scout => "--scout-model=", HullRole.Frigate => "--frigate-model=",
                _ when FleetSimulation.UsesHaulerModel( role ) => "--miner-model=", _ => "--ship-model="
            };
            string? custom = args.FirstOrDefault( arg => arg.StartsWith( option, StringComparison.Ordinal ) )?[option.Length..];
            if ( custom is null && FleetSimulation.UsesHaulerModel( role ) )
                custom = args.FirstOrDefault( arg => arg.StartsWith( "--freighter-model=", StringComparison.Ordinal ) )?["--freighter-model=".Length..];
            result[(int)role] = new( custom ?? file, custom is null ? scale : 1, custom is not null || flatten,
                new( CatalogJson.Number( row, "nozzleOffset", 0, 10, 0.91f ), CatalogJson.Number( row, "nozzleAft", 0, 10, 1.12f ),
                    CatalogJson.Number( row, "plumeWidth", 0.01f, 4, 0.12f ) ) );
        }
        foreach ( var role in roles )
            if ( !System.IO.File.Exists( result[(int)role].File ) ) throw new InvalidDataException( $"Missing {role} model: {result[(int)role].File}" );
        return result;
    }
}

internal sealed class HullMeshes : IDisposable {
    private readonly Dictionary<(string, float, bool), StaticMeshSet> _loaded = [];
    private readonly StaticMeshSet[] _roles;
    private readonly Dictionary<StaticMeshSet, (Vector3 Center, float Radius)> _bounds = [];
    internal (Vector3 Center, float Radius) Bounds( int role ) => _bounds[_roles[role]];
    private readonly ThrusterEmitter[][] _emitters;
    internal ReadOnlySpan<ThrusterEmitter> Emitters( int role ) => _emitters[role];
    internal int RoleCount => _roles.Length;
    internal StaticMeshSet this[int role] => _roles[role];

    internal HullMeshes( GraphicsDevice device, GraphicsCommandList commands, HullModel[] models ) {
        var markers = new Dictionary<StaticMeshSet, ReadOnlyMemory<StaticModelMarker>>();
        _emitters = new ThrusterEmitter[models.Length][];
        _roles = new StaticMeshSet[models.Length];
        try {
            for ( int i = 0; i < models.Length; ++i ) {
                var model = models[i];
                var key = (model.File, model.Scale, model.Flatten);
                if ( !_loaded.TryGetValue( key, out var mesh ) ) {
                    var scene = GltfModel.Load( model.File );
                    mesh = new( device, commands, scene, model.Scale, model.Flatten );
                    var min = scene.BoundsMin;
                    var max = scene.BoundsMax;
                    if ( !model.Flatten ) {
                        min = new Vector3( float.PositiveInfinity );
                        max = new Vector3( float.NegativeInfinity );
                        foreach ( var primitive in scene.Primitives )
                            foreach ( var vertex in primitive.Vertices ) {
                                min = Vector3.Min( min, vertex.Position );
                                max = Vector3.Max( max, vertex.Position );
                            }
                    }
                    _bounds.Add( mesh, ((max + min) * (model.Scale * 0.5f), Math.Max( 0.001f, (max - min).Length() * model.Scale * 0.5f )) );
                    _loaded.Add( key, mesh );
                    markers.Add( mesh, scene.Markers );
                }
                _roles[i] = mesh;
                _emitters[i] = ReadEmitters( markers[mesh].Span, model );
            }
        } catch { Dispose( ); throw; }
    }
    private static ThrusterEmitter[] ReadEmitters( ReadOnlySpan<StaticModelMarker> markers, HullModel model ) {
        var result = new List<ThrusterEmitter>();
        foreach ( var marker in markers ) {
            if ( !marker.Name.StartsWith( "exhaust_", StringComparison.OrdinalIgnoreCase ) ) continue;
            var position = marker.Transform.Translation * model.Scale;
            var axis = Vector3.TransformNormal( Vector3.UnitZ, marker.Transform );
            float length = axis.Length();
            if ( !float.IsFinite( length ) || length < 1e-6f || !float.IsFinite( position.LengthSquared() ) )
                throw new InvalidDataException( $"Invalid emitter {marker.Name} in {model.File}." );
            result.Add( new( position, axis / length, model.Nozzles.Z ) );
        }
        if ( result.Count > 0 ) return result.ToArray();
        float offset = model.Nozzles.X, aft = model.Nozzles.Y, width = model.Nozzles.Z;
        return offset == 0 ? [new( new( 0, 0, aft ), Vector3.UnitZ, width )] :
            [new( new( -offset, 0, aft ), Vector3.UnitZ, width ), new( new( offset, 0, aft ), Vector3.UnitZ, width )];
    }

    public void Dispose( ) {
        foreach ( var mesh in _loaded.Values ) mesh.Dispose( );
        _loaded.Clear( );
    }
}
