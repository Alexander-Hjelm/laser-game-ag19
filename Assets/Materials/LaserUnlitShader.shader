Shader "Unlit/LaserUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MaskTex ("TextureMask", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _MainScrollSpeed ("Main Scroll Speed", Range(0, 30)) = 2
        _NoiseScaleX ("Noise Scale X", Range(0.01, 10)) = 1
        _NoiseScaleY ("Noise Scale Y", Range(0.01, 10)) = 1
        _NoiseAmount ("Noise Amount", Range(0.01, 1)) = 1
    }
    SubShader
    {
        //Tags { "RenderType"="Opaque" }
        Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
        LOD 100

		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass {
			CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				struct appdata_t {
					float4 vertex : POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				struct v2f {
					float4 vertex : SV_POSITION;
					float2 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					UNITY_VERTEX_OUTPUT_STEREO
				};

				sampler2D _MainTex;
				sampler2D _MaskTex;
				float4 _MainTex_ST;
				fixed _MainScrollSpeed;
                fixed _NoiseScaleX;
                fixed _NoiseScaleY;
                fixed _NoiseAmount;
                fixed4 _Color;

                float hash( float n )
                {
                    return frac(sin(n)*43758.5453);
                }
 
                float noise( float3 x )
                {
                    // The noise function returns a value in the range -1.0f -> 1.0f
                 
                    float3 p = floor(x);
                    float3 f = frac(x);
                 
                    f = f*f*(3.0-2.0*f);
                    float n = p.x + p.y*57.0 + 113.0*p.z;
         
                    return lerp(lerp(lerp( hash(n+0.0), hash(n+1.0),f.x),
                                   lerp( hash(n+57.0), hash(n+58.0),f.x),f.y),
                               lerp(lerp( hash(n+113.0), hash(n+114.0),f.x),
                                   lerp( hash(n+170.0), hash(n+171.0),f.x),f.y),f.z);
                }


				v2f vert (appdata_t v)
				{
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag (v2f i) : SV_Target
				{
                    //Scrolling UVs
                    fixed2 uv = i.texcoord;
                    fixed xScrollValue = _MainScrollSpeed * _Time;
                    uv -= fixed2(xScrollValue, 0.0);
                    
					fixed4 col = tex2D(_MainTex, uv) * _Color;
                    col *= tex2D(_MaskTex, i.texcoord);
                    col *= lerp(1, noise(float3(uv.x*_NoiseScaleX, uv.y*_NoiseScaleY, 0.0)), _NoiseAmount);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
			ENDCG
		}
    }
}
