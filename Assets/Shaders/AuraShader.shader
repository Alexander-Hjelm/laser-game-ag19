Shader "Unlit/AuraShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AspectRatio ("Screen Aspect Ratio", Float) = 1
       }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent"  }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        
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
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _ObjectsLength;
			float4 _Objects[10];
			float _ObjectAngles[10];
            float _AspectRatio;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f x) : SV_Target
            {
                for(int i = 0; i < _ObjectsLength; i++) {
                    float4 obj = _Objects[i];
					float angle = _ObjectAngles[i];
                    float2 distVec = x.uv - obj.xy;
                    distVec.x *= _AspectRatio;
					// Ellipse
					float numerator = distVec.x * cos(angle) - distVec.y * sin(angle);
					float numerator2 = distVec.x * sin(angle) + distVec.y * cos(angle);
					float dist = (numerator * numerator) / (obj.z * obj.z) + (numerator2 * numerator2) / (obj.w * obj.w);
                    if (dist < 1)
                        return fixed4(1,0,0,1);
                }
                return fixed4(0,0,0,0);
            }
            ENDCG
        }
    }
}
