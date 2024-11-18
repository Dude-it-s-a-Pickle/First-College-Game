Shader "Unlit/WallShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WallColor ("Wall Color", Color) = (0.22, 0.22, 0.341, 1)
        _BackColor ("Background Color", Color) = (0.16, 0.16, 0.28, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
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
            float4 _WallColor;
            float4 _BackColor;
            
            uniform float2[] _wallPos = float2[]((0.0, 0.0), (1.0, 1.0), (2.0, 2.0), (3.0, 3.0));
            uniform float2 _levelSize = (8.0);

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
                // Drawing Walls
                bool walls = false;

                for (int i = 0; i < _wallPos.length(); i++)
                {
                    bool possibleX = ((i.uv.x > (_wallPos[i].x/levelSize.x)) && (i.uv.x < ((_wallPos[i].x+1.0)/levelSize.x)));
                    bool possibleY = ((i.uv.y > (_wallPos[i].y/levelSize.y)) && (i.uv.y < ((_wallPos[i].y+1.0)/levelSize.y)));
                    walls = (walls || (possibleX && possibleY));
                }

                float4 color = mix(_BackColor, _WallColor, float(walls));

                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) + color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
