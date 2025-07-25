Shader "HighlightPlus/Geometry/InnerGlow" {
Properties {
    _MainTex ("Texture", Any) = "white" {}
    _Color ("Color", Color) = (1,1,1) // not used; dummy property to avoid inspector warning "material has no _Color property"
    _InnerGlowColor ("Inner Glow Color", Color) = (1,1,1,1)
    _InnerGlowData ("Data", Vector) = (1,1,1,0)
    _CutOff("CutOff", Float ) = 0.5
    _Cull ("Cull Mode", Int) = 2
    _InnerGlowZTest ("ZTest", Int) = 4
    _InnerGlowBlendMode("Blend Mode", Int) = 1
}
    SubShader
    {
        Tags { "Queue"="Transparent+122" "RenderType"="Transparent" }
    
        // Inner Glow
        Pass
        {
        	Stencil {
                Ref 4
                ReadMask 4
                Comp NotEqual
                Pass keep
            }
            Blend SrcAlpha [_InnerGlowBlendMode]
            ZWrite Off
            ZTest [_InnerGlowZTest]
            Cull [_Cull]

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ HP_ALPHACLIP

            #include "UnityCG.cginc"
            #include "CustomVertexTransform.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
                float3 norm   : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos    : SV_POSITION;
                float2 uv     : TEXCOORD0;
                float3 wpos   : TEXCOORD1;
                float3 norm   : NORMAL;
				UNITY_VERTEX_OUTPUT_STEREO
            };
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed _CutOff;
      		fixed4 _InnerGlowColor;
      		fixed4 _InnerGlowData;
            #define INNER_GLOW_WIDTH _InnerGlowData.x
            #define INNER_GLOW_POWER _InnerGlowData.y

            v2f vert (appdata v)
            {
                v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.pos = ComputeVertexPosition(v.vertex);
                o.wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.norm = UnityObjectToWorldNormal(v.norm);
                o.uv = TRANSFORM_TEX (v.uv, _MainTex);

                #if UNITY_REVERSED_Z
                    o.pos.z += 0.0001;
                #else
                    o.pos.z -= 0.0001;
                #endif
                                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                #if HP_ALPHACLIP
                    fixed4 color = tex2D(_MainTex, i.uv);
                    clip(color.a - _CutOff);
                #endif
            
            	float3 viewDir = normalize(i.wpos - _WorldSpaceCameraPos.xyz);
                float hn = abs(dot(viewDir, normalize(i.norm)));
            	fixed dx = saturate((INNER_GLOW_WIDTH - hn) / INNER_GLOW_WIDTH);
                dx = 1.0 - pow( 1.0 - dx, INNER_GLOW_POWER);
                fixed4 col = _InnerGlowColor;
                col.a *= dx;
                col.a = saturate(col.a);
				return col;
            }
            ENDCG
        }

    }
}