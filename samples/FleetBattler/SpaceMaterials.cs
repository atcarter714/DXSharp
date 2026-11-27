namespace FleetBattler;

// Shared procedural material functions. Coordinates and directions are body-local.
internal static class SpaceMaterials {
    internal const string Hlsl = """
        struct BodyMaterial {
            float3 albedo;
            float3 normal;
            float height;
            float metalness;
            float roughness;
        };

        float TerrainHeight(float2 p) {
            return .53 * Noise(p * 6) + .28 * Noise(p * 17) + .13 * Noise(p * 47) + .06 * Noise(p * 123);
        }

        BodyMaterial RockMaterial(float3 n, float spin) {
            float longitude = atan2(n.x,n.z) + spin;
            float2 p = float2(cos(longitude),sin(longitude))*sqrt(saturate(1-n.y*n.y)) + n.y*float2(1.6,2.4);
            BodyMaterial m;
            m.height = TerrainHeight(p);
            float dx = TerrainHeight(p + float2(.008,0)) - m.height;
            float dy = TerrainHeight(p + float2(0,.008)) - m.height;
            m.normal = normalize(n - float3(dx,dy,0)*1.2);
            m.albedo = lerp(float3(.22,.18,.17),float3(.66,.51,.39),smoothstep(.23,.79,m.height));
            m.roughness = lerp(.97,.62,m.height);
            m.metalness = .055 * smoothstep(.68,.79,m.height);
            return m;
        }

        float3 Regolith(float3 n, float3 light, float spin) {
            BodyMaterial m = RockMaterial(n,spin);
            float incidence = saturate(dot(m.normal,light));
            float highlight = pow(saturate(dot(m.normal,normalize(light + float3(.001,0,1)))),lerp(48,5,m.roughness));
            return m.albedo*(.075 + .925*incidence)*(1-m.metalness) + highlight*incidence*lerp(.018,m.albedo,m.metalness);
        }

        float3 StellarSurface(float3 sky, float2 p, float radius, float time, float kelvin) {
            float a = atan2(p.y,p.x);
            float limb = max(radius - 1,0);
            float loops = 0;
            const float3 activeRegions = float3(-2.3,-.77,.40);
            float plasma = 0;
            [unroll] for(int i = 0; i < 3; ++i) {
                float center = activeRegions[i] + .18*sin(time*.04+i);
                float angle = atan2(sin(a-center),cos(a-center));
                float pulse = .22+.78*smoothstep(.23,.74,.5+.5*sin(time*.115+i*2.03));
                float shear = angle + limb*(.17 + .11*sin(i*2.1));
                float stream = exp(-pow(shear/(.055 + limb*.36),2)) * exp(-limb/(.10 + .25*pulse));
                float strands = .25 + .75*pow(.5+.5*sin(shear*113 + Noise(p*8 + time*.02)*8),3);
                plasma += stream*strands*pulse;
                [unroll] for(int strand = 0; strand < 4; ++strand) {
                    float shift = (strand-1.5)*.023;
                    float across = (angle-shift)/(.13+strand*.016);
                    float arch = (.09 + strand*.042 + .08*pulse)*sqrt(saturate(1-across*across));
                    loops += exp(-abs(limb-arch)*(63+strand*19)) *
                        (1-smoothstep(.89,1.06,abs(across))) * pulse * .25;
                }
            }
            float magnetic = pow(.5 + .5*sin(a*17 + Noise(p*5)*4),3);
            float corona = exp(-limb*(3.4+magnetic*2)) * (.12+.21*magnetic);
            float field = (1-smoothstep(2.3,3.55,radius));
            sky += (float3(1,.32,.08)*(corona + .69*loops) + float3(1,.52,.12)*plasma*.85)*field;
            sky += float3(.96,.46,.10)*exp(-limb*7.8)*.09*field;
            if (radius > 1.02) return sky;
            float view = sqrt(saturate(1-radius*radius));
            float2 surface = float2(atan2(p.x,view) + time*.033, asin(clamp(p.y,-1,1)));
            float2 shear = float2(Noise(surface*9 + time*.013),Noise(surface*12-5.2));
            float2 flow = surface + shear*.14;
            float plasmaLanes = Noise(flow*24)*.57 + Noise(flow*53)*.31 + Noise(flow*99)*.12;
            float granules = Noise(flow*154)*.07;
            float active = Noise(surface*4.4 + float2(9.7,1.8));
            float heat = smoothstep(.20,.83,plasmaLanes + granules + .14*active);
            float3 warm = lerp(float3(.59,.19,.033),float3(.82,.34,.075),saturate((kelvin-3400)/3100));
            float3 hot = lerp(float3(1,.60,.22),float3(1,.93,.68),saturate((kelvin-3400)/3100));
            float magneticRim = pow(1-view,4.4)*(.20+.80*Noise(flow*38));
            float3 disk = lerp(warm,hot,heat)*(.71+.29*view) + float3(1,.55,.14)*magneticRim*.26;
            return lerp(sky,disk,1-smoothstep(.984,1.018,radius));
        }

        float3 StellarFlare(float2 pixel, float2 sun, float radius, float2 size) {
            if (any(sun < -radius*.45) || any(sun > size + radius*.45)) return 0;
            float2 center = size*.5;
            float2 uv = (pixel-sun)/max(radius,4);
            float streak = exp(-abs(uv.x)*.6 - abs(uv.y)*28) * .095;
            float2 axis = center-sun;
            float ghost = exp(-length(pixel-(center+.48*axis))/max(size.y*.024,1)*2.8);
            float halo = abs(length(pixel-(center+.83*axis))/max(size.y*.056,1)-1);
            return float3(1,.44,.19)*streak + float3(.15,.41,.47)*(ghost*.16 + exp(-halo*32)*.05);
        }

        """;
}
