Shader "Unlit/test"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
         _Color1 ("Color 1", color) = (1,0,0,1)
        _Color2 ("Color 2", color) = (0,0,0,1)
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color1;
            float4 _Color2;
            float _Mouseposx;
            float _Mouseposy;
            float _Mouseposz;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //float3 
                float4 pos = mul(unity_ObjectToWorld, i.vertex).xyzw;
                //i think the problem is that i don't understand what that line is doing
                //float distance = (pos.z*0.0001) -_Mouseposz;
                //distance += ((pos.x*0.01)-_Mouseposx);
                //distance += ((pos.y*0.01) -_Mouseposy);
                float distance = (pos.x*0.001) - _Mouseposx;
                //(pos.x*0.01)
                fixed4 col = lerp(_Color1, _Color2, distance);
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
