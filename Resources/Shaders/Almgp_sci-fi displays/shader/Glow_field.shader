Shader "Almgp/glow_field" {
    Properties {
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _tile ("tile", Float ) = 2
        _blurfactor ("blur factor", Range(0, 1)) = 0
        _power ("power", Float ) = 1
        _Texture ("Texture", 2D) = "white" {}
        _Texture_blured ("Texture_blured", 2D) = "white" {}
        _speedU ("speed U", Float ) = 0
        _detail_emmis ("detail_emmis", 2D) = "white" {}
        _detail_size ("detail_size", Float ) = 2
        _detailcolor ("detail color", Color) = (0.5,0.5,0.5,1)
        _detailpower ("detail power", Float ) = 1
        _speedV ("speed V", Float ) = 0
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend One One
            Cull Off
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Texture_blured; uniform float4 _Texture_blured_ST;
            uniform float _tile;
            uniform float _blurfactor;
            uniform float _power;
            uniform float _speedU;
            uniform sampler2D _detail_emmis; uniform float4 _detail_emmis_ST;
            uniform float _detail_size;
            uniform float4 _detailcolor;
            uniform float _detailpower;
            uniform float _speedV;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                UNITY_FOG_COORDS(1)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i, float facing : VFACE) : COLOR {
                float isFrontFace = ( facing >= 0 ? 1 : 0 );
                float faceSign = ( facing >= 0 ? 1 : -1 );
////// Lighting:
////// Emissive:
                float node_1854 = 0.0;
                float4 node_2070 = _Time + _TimeEditor;
                float2 node_4277 = (float2((i.uv0.r+(node_2070.r*_speedU)),(i.uv0.g+(node_2070.r*_speedV)))*_detail_size);
                float4 _detail_emmis_var = tex2D(_detail_emmis,TRANSFORM_TEX(node_4277, _detail_emmis));
                float2 node_9494 = (i.uv0*_tile);
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9494, _Texture));
                float4 _Texture_blured_var = tex2D(_Texture_blured,TRANSFORM_TEX(node_9494, _Texture_blured));
                float3 node_5747 = lerp(_Texture_var.rgb,((_Texture_blured_var.rgb+_Texture_blured_var.rgb)+_Texture_var.rgb),_blurfactor);
                float3 emissive = (lerp(float3(node_1854,node_1854,node_1854),(_detail_emmis_var.rgb*_detailcolor.rgb*_detailpower),saturate(node_5747))+(_TintColor.rgb*node_5747*_power));
                float3 finalColor = emissive;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG_COLOR(i.fogCoord, finalRGBA, fixed4(0,0,0,1));
                return finalRGBA;
            }
            ENDCG
        }
    }
}
