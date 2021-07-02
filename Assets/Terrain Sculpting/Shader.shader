Shader "Unlit/Shader"
{
    Properties
    {
        _Color1 ("Color 1", color) = (1,0,0,1)
        _Color2 ("Color 2", color) = (0,0,0,1)
        _SpecularColor("Specular color", color) = (1,1,1,1)
        _Size ("Size", float) = 15.0
        _Definition ("Definition?", float) = 2.0
        _AmbientColor ("Ambient Color", color) = (1,1,1,1)
        _AmbientIntensity ("Ambient Intensity", range(0.0,5.0)) = 1.0
        _Bias ("Bias?", float) = 0.5
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags {"LightMode"="ForwardBase"}
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityLightingCommon.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                UNITY_SHADOW_COORDS(2)
                float4 pos : SV_POSITION;
                float4 diffuse : COLOR0;
                float4 ambient : COLOR1;
                float4 specular : COLOR2;
            };

            float4 _Color1;
            float4 _Color2;
            float4 _SpecularColor;
            float _Size;
            float _Definition;
            float _Bias;
            float4 _AmbientColor;
            float _AmbientIntensity;
            float _Mouseposx;
            float _Mouseposy;
            float _Mouseposz;


            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_SHADOW(o);
                 float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 lightVector = _WorldSpaceLightPos0.xyz;
                fixed howMuchLight = max(0, dot(worldNormal, lightVector));
                o.diffuse = howMuchLight * fixed4(1,1,1,1);
                float4 worldNormal4 = float4(worldNormal.x, worldNormal.y, worldNormal.z, 1);
                o.ambient.rgb =ShadeSH9(worldNormal4);
                float3 projection = dot(worldNormal, lightVector) *worldNormal;
                float3 reflection = 2*projection - lightVector;

                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float3 eye = normalize(UnityWorldSpaceViewDir(worldPos));

                float howMuchSpecular = max(0, pow(dot(reflection,eye), 2));
                o.specular = howMuchSpecular* _SpecularColor;
                return o;
            }

            fixed random01 (fixed x){
                return frac(sin(x*240290));
            }

            fixed srandom(fixed x){
                return random01(x) * 2 - 1;
            }

            float randomCombo(fixed x, fixed y){
                return srandom(x * 0.123 + x * x * 0.0345 + y *-1.342);
            }
                
            fixed2 randomUnitVector(fixed x, fixed y){
               fixed theta = randomCombo(x,y) *3.1415;
               return fixed2( cos(theta),sin(theta) );
            }
                
            fixed2 dotWithCorner(fixed column, fixed row, fixed2 coords){
                fixed2 randomVector = randomUnitVector(column, row);
                fixed2 vectorToPoint = coords - fixed2(column, row);
                return dot(randomVector, vectorToPoint);
            }
                
            fixed4 perlinNoise(fixed2 coords){
                fixed column = floor(coords.x);
                fixed row = floor(coords.y);

                fixed dot_ul = dotWithCorner(column, row, coords);
                fixed dot_ur = dotWithCorner(column+1,row,coords);
                fixed dot_ll = dotWithCorner(column, row+1, coords);
                fixed dot_lr = dotWithCorner(column+1, row+1, coords);

                fixed upper = lerp(dot_ul, dot_ur, frac(coords.x));
                fixed lower = lerp(dot_ll, dot_lr, frac (coords.x));

                return lerp(upper, lower, frac(coords.y));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 lighting = (i.ambient*(_AmbientColor*_AmbientIntensity)) + (i.diffuse*SHADOW_ATTENUATION(i));
                fixed val = perlinNoise(i.uv*_Size) * _Definition + _Bias;
                fixed4 coll= lerp(_Color1, _Color2, clamp(val,0,1)) * lighting +i.specular;
                UNITY_APPLY_FOG(i.fogCoord, coll);
                return coll;
            }
            ENDCG
        }
        UsePass "Legacy Shaders/VertexLit/SHADOWCASTER"
    }
}
