// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33586,y:32012,varname:node_4795,prsc:2|emission-3953-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32209,y:32317,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:_Texture,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9494-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:33313,y:32115,varname:node_2393,prsc:2|A-797-RGB,B-5747-OUT,C-7920-OUT;n:type:ShaderForge.SFN_Color,id:797,x:32529,y:32849,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32308,y:33104,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:5664,x:32200,y:32567,ptovrint:False,ptlb:Texture_blured,ptin:_Texture_blured,varname:_Texture_blured,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9494-OUT;n:type:ShaderForge.SFN_TexCoord,id:9074,x:31563,y:32402,varname:node_9074,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:6400,x:31648,y:32594,ptovrint:False,ptlb:tile,ptin:_tile,varname:_tile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:9494,x:31835,y:32372,varname:node_9494,prsc:2|A-9074-UVOUT,B-6400-OUT;n:type:ShaderForge.SFN_Add,id:645,x:32358,y:32517,varname:node_645,prsc:2|A-5664-RGB,B-5664-RGB;n:type:ShaderForge.SFN_Add,id:2381,x:32639,y:32410,varname:node_2381,prsc:2|A-645-OUT,B-6074-RGB;n:type:ShaderForge.SFN_Lerp,id:5747,x:32928,y:32425,varname:node_5747,prsc:2|A-6074-RGB,B-2381-OUT,T-1576-OUT;n:type:ShaderForge.SFN_Slider,id:1576,x:32344,y:32710,ptovrint:False,ptlb:blur factor,ptin:_blurfactor,varname:_blurfactor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ValueProperty,id:7920,x:32975,y:32887,ptovrint:False,ptlb:power,ptin:_power,varname:_power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Time,id:2070,x:31890,y:32047,varname:node_2070,prsc:2;n:type:ShaderForge.SFN_ValueProperty,id:9613,x:31825,y:32226,ptovrint:False,ptlb:speed U,ptin:_speedU,varname:_speedU,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Tex2d,id:3424,x:32702,y:31889,ptovrint:False,ptlb:detail_emmis,ptin:_detail_emmis,varname:_detail_emmis,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:529239097d02f9f42b0ddd436c6fcbb0,ntxv:0,isnm:False|UVIN-4277-OUT;n:type:ShaderForge.SFN_TexCoord,id:8345,x:31791,y:31871,varname:node_8345,prsc:2,uv:0;n:type:ShaderForge.SFN_Multiply,id:4277,x:32276,y:31751,varname:node_4277,prsc:2|A-217-OUT,B-192-OUT;n:type:ShaderForge.SFN_ValueProperty,id:192,x:31986,y:31683,ptovrint:False,ptlb:detail_size,ptin:_detail_size,varname:_detail_size,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Clamp01,id:3906,x:33080,y:32032,varname:node_3906,prsc:2|IN-5747-OUT;n:type:ShaderForge.SFN_Lerp,id:3108,x:33279,y:31886,varname:node_3108,prsc:2|A-1854-OUT,B-4946-OUT,T-3906-OUT;n:type:ShaderForge.SFN_Vector1,id:1854,x:33123,y:31790,varname:node_1854,prsc:2,v1:0;n:type:ShaderForge.SFN_Color,id:6878,x:32805,y:31996,ptovrint:False,ptlb:detail color,ptin:_detailcolor,varname:_detailcolor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:4946,x:33019,y:31917,varname:node_4946,prsc:2|A-3424-RGB,B-6878-RGB,C-415-OUT;n:type:ShaderForge.SFN_ValueProperty,id:415,x:32859,y:31791,ptovrint:False,ptlb:detail power,ptin:_detailpower,varname:_detailpower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Add,id:3953,x:33481,y:31924,varname:node_3953,prsc:2|A-3108-OUT,B-2393-OUT;n:type:ShaderForge.SFN_Multiply,id:6166,x:32156,y:32056,varname:node_6166,prsc:2|A-2070-TSL,B-9613-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3984,x:32006,y:32264,ptovrint:False,ptlb:speed V,ptin:_speedV,varname:_speedV,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:7971,x:32170,y:32193,varname:node_7971,prsc:2|A-2070-TSL,B-3984-OUT;n:type:ShaderForge.SFN_Add,id:6717,x:32114,y:31797,varname:node_6717,prsc:2|A-8345-U,B-6166-OUT;n:type:ShaderForge.SFN_Add,id:3694,x:32114,y:31914,varname:node_3694,prsc:2|A-8345-V,B-7971-OUT;n:type:ShaderForge.SFN_Append,id:217,x:32296,y:31867,varname:node_217,prsc:2|A-6717-OUT,B-3694-OUT;proporder:797-6400-1576-7920-6074-5664-9613-3424-192-6878-415-3984;pass:END;sub:END;*/

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
    CustomEditor "ShaderForgeMaterialInspector"
}
