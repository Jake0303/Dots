// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:2,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:33195,y:32174,varname:node_4795,prsc:2|emission-2393-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32209,y:32317,ptovrint:False,ptlb:Texture,ptin:_Texture,varname:_Texture,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9494-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32968,y:32391,varname:node_2393,prsc:2|A-797-RGB,B-5747-OUT,C-7920-OUT;n:type:ShaderForge.SFN_Color,id:797,x:32529,y:32849,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Vector1,id:9248,x:32308,y:33104,varname:node_9248,prsc:2,v1:2;n:type:ShaderForge.SFN_Tex2d,id:5664,x:32200,y:32567,ptovrint:False,ptlb:Texture_blured,ptin:_Texture_blured,varname:_Texture_blured,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-9494-OUT;n:type:ShaderForge.SFN_TexCoord,id:9074,x:31252,y:32298,varname:node_9074,prsc:2,uv:0;n:type:ShaderForge.SFN_ValueProperty,id:6400,x:31648,y:32594,ptovrint:False,ptlb:tile,ptin:_tile,varname:_tile,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Multiply,id:9494,x:31835,y:32372,varname:node_9494,prsc:2|A-9213-OUT,B-6400-OUT;n:type:ShaderForge.SFN_Add,id:645,x:32358,y:32517,varname:node_645,prsc:2|A-5664-RGB,B-5664-RGB;n:type:ShaderForge.SFN_Add,id:2381,x:32535,y:32480,varname:node_2381,prsc:2|A-645-OUT,B-6074-RGB;n:type:ShaderForge.SFN_Lerp,id:5747,x:32714,y:32334,varname:node_5747,prsc:2|A-6074-RGB,B-2381-OUT,T-1576-OUT;n:type:ShaderForge.SFN_Slider,id:1576,x:32344,y:32710,ptovrint:False,ptlb:blur factor,ptin:_blurfactor,varname:_blurfactor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_ValueProperty,id:7920,x:32975,y:32887,ptovrint:False,ptlb:power,ptin:_power,varname:_power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Append,id:9213,x:31622,y:32244,varname:node_9213,prsc:2|A-4980-OUT,B-2447-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1345,x:31180,y:32105,ptovrint:False,ptlb:U,ptin:_U,varname:_U,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_ValueProperty,id:2725,x:31374,y:32087,ptovrint:False,ptlb:V,ptin:_V,varname:_V,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Add,id:2447,x:31477,y:32401,varname:node_2447,prsc:2|A-2725-OUT,B-9074-V;n:type:ShaderForge.SFN_Add,id:4980,x:31423,y:32209,varname:node_4980,prsc:2|A-1345-OUT,B-9074-U;proporder:797-6400-1576-7920-6074-5664-1345-2725;pass:END;sub:END;*/

Shader "Almgp/Glow" {
    Properties {
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _tile ("tile", Float ) = 2
        _blurfactor ("blur factor", Range(0, 1)) = 0
        _power ("power", Float ) = 1
        _Texture ("Texture", 2D) = "white" {}
        _Texture_blured ("Texture_blured", 2D) = "white" {}
        _U ("U", Float ) = 0
        _V ("V", Float ) = 0
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
            uniform sampler2D _Texture; uniform float4 _Texture_ST;
            uniform float4 _TintColor;
            uniform sampler2D _Texture_blured; uniform float4 _Texture_blured_ST;
            uniform float _tile;
            uniform float _blurfactor;
            uniform float _power;
            uniform float _U;
            uniform float _V;
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
                float2 node_9494 = (float2((_U+i.uv0.r),(_V+i.uv0.g))*_tile);
                float4 _Texture_var = tex2D(_Texture,TRANSFORM_TEX(node_9494, _Texture));
                float4 _Texture_blured_var = tex2D(_Texture_blured,TRANSFORM_TEX(node_9494, _Texture_blured));
                float3 emissive = (_TintColor.rgb*lerp(_Texture_var.rgb,((_Texture_blured_var.rgb+_Texture_blured_var.rgb)+_Texture_var.rgb),_blurfactor)*_power);
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
