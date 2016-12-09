// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Shader created with Shader Forge v1.26 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.26;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:0,dpts:2,wrdp:False,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:True,fgod:False,fgor:False,fgmd:0,fgcr:0,fgcg:0,fgcb:0,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:True;n:type:ShaderForge.SFN_Final,id:4795,x:34173,y:32369,varname:node_4795,prsc:2|emission-4668-OUT;n:type:ShaderForge.SFN_Tex2d,id:6074,x:32235,y:32601,ptovrint:False,ptlb:MainTex,ptin:_MainTex,varname:_MainTex,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:fb1f2711600d5854caf2a211035f679f,ntxv:0,isnm:False|UVIN-8047-OUT;n:type:ShaderForge.SFN_Multiply,id:2393,x:32671,y:32526,varname:node_2393,prsc:2|A-6074-RGB,B-2053-RGB,C-797-RGB,D-4885-OUT;n:type:ShaderForge.SFN_VertexColor,id:2053,x:32235,y:32772,varname:node_2053,prsc:2;n:type:ShaderForge.SFN_Color,id:797,x:32005,y:32908,ptovrint:True,ptlb:Color,ptin:_TintColor,varname:_TintColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:8980,x:32470,y:32492,ptovrint:False,ptlb:power,ptin:_power,varname:_power,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Time,id:3364,x:31948,y:32351,varname:node_3364,prsc:2;n:type:ShaderForge.SFN_Sin,id:4474,x:32303,y:32190,varname:node_4474,prsc:2|IN-396-OUT;n:type:ShaderForge.SFN_Multiply,id:396,x:32129,y:32413,varname:node_396,prsc:2|A-3364-TSL,B-7164-OUT;n:type:ShaderForge.SFN_ValueProperty,id:7164,x:31757,y:32548,ptovrint:False,ptlb:speed,ptin:_speed,varname:_speed,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_RemapRange,id:6846,x:32496,y:32190,varname:node_6846,prsc:2,frmn:-1,frmx:1,tomn:0.5,tomx:1.5|IN-4474-OUT;n:type:ShaderForge.SFN_Multiply,id:5483,x:32667,y:32250,varname:node_5483,prsc:2|A-6846-OUT,B-8980-OUT;n:type:ShaderForge.SFN_Slider,id:8651,x:32445,y:32387,ptovrint:False,ptlb:pulse factor,ptin:_pulsefactor,varname:_pulsefactor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:4885,x:32847,y:32287,varname:node_4885,prsc:2|A-5483-OUT,B-8980-OUT,T-8651-OUT;n:type:ShaderForge.SFN_FragmentPosition,id:3477,x:31728,y:32700,varname:node_3477,prsc:2;n:type:ShaderForge.SFN_Append,id:6692,x:31905,y:32635,varname:node_6692,prsc:2|A-3477-X,B-3477-Z;n:type:ShaderForge.SFN_Divide,id:8047,x:32086,y:32575,varname:node_8047,prsc:2|A-6692-OUT,B-2778-OUT;n:type:ShaderForge.SFN_ValueProperty,id:2778,x:32082,y:32818,ptovrint:False,ptlb:size,ptin:_size,varname:_size,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:2;n:type:ShaderForge.SFN_Fresnel,id:6210,x:32945,y:32649,varname:node_6210,prsc:2|NRM-8311-OUT,EXP-9012-OUT;n:type:ShaderForge.SFN_NormalVector,id:8311,x:32713,y:32672,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:9012,x:32740,y:32883,ptovrint:False,ptlb:exp,ptin:_exp,varname:_exp,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.1,cur:0.8856825,max:10;n:type:ShaderForge.SFN_Color,id:5276,x:33022,y:32747,ptovrint:False,ptlb:addColor,ptin:_addColor,varname:_addColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:2039,x:33199,y:32727,varname:node_2039,prsc:2|A-3353-RGB,B-5276-RGB,T-6210-OUT;n:type:ShaderForge.SFN_Color,id:3353,x:33104,y:32996,ptovrint:False,ptlb:backColor,ptin:_backColor,varname:_backColor,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Add,id:3236,x:33145,y:32491,varname:node_3236,prsc:2|A-2393-OUT,B-2039-OUT;n:type:ShaderForge.SFN_Cubemap,id:1666,x:33149,y:32109,ptovrint:False,ptlb:cubemap,ptin:_cubemap,varname:_cubemap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:f58f75778490b164dac25cc5a8dc9100,pvfc:0|DIR-8038-OUT;n:type:ShaderForge.SFN_ViewReflectionVector,id:8038,x:32932,y:32096,varname:node_8038,prsc:2;n:type:ShaderForge.SFN_Fresnel,id:9273,x:33461,y:31969,varname:node_9273,prsc:2|NRM-8895-OUT,EXP-2643-OUT;n:type:ShaderForge.SFN_NormalVector,id:8895,x:33161,y:31874,prsc:2,pt:False;n:type:ShaderForge.SFN_Slider,id:2643,x:32817,y:31968,ptovrint:False,ptlb:cubemap exp,ptin:_cubemapexp,varname:_cubemapexp,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0.3,cur:1.052634,max:10;n:type:ShaderForge.SFN_Color,id:4988,x:33298,y:32263,ptovrint:False,ptlb:color cubemap,ptin:_colorcubemap,varname:_colorcubemap,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5,c2:0.5,c3:0.5,c4:1;n:type:ShaderForge.SFN_Multiply,id:9385,x:33537,y:32123,varname:node_9385,prsc:2|A-1666-RGB,B-4988-RGB,C-9273-OUT;n:type:ShaderForge.SFN_Add,id:7985,x:33476,y:32497,varname:node_7985,prsc:2|A-9385-OUT,B-3236-OUT;n:type:ShaderForge.SFN_Lerp,id:4668,x:33876,y:32477,varname:node_4668,prsc:2|A-1854-OUT,B-7985-OUT,T-4657-OUT;n:type:ShaderForge.SFN_Vector1,id:1854,x:33676,y:32435,varname:node_1854,prsc:2,v1:0;n:type:ShaderForge.SFN_Slider,id:4657,x:33557,y:32742,ptovrint:False,ptlb:final power,ptin:_finalpower,varname:_finalpower,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;proporder:6074-797-8980-7164-8651-2778-9012-5276-3353-1666-2643-4988-4657;pass:END;sub:END;*/

Shader "Cybedie/glow_grid" {
    Properties {
        _MainTex ("MainTex", 2D) = "white" {}
        _TintColor ("Color", Color) = (0.5,0.5,0.5,1)
        _power ("power", Float ) = 2
        _speed ("speed", Float ) = 2
        _pulsefactor ("pulse factor", Range(0, 1)) = 0
        _size ("size", Float ) = 2
        _exp ("exp", Range(0.1, 10)) = 0.8856825
        _addColor ("addColor", Color) = (1,1,1,1)
        _backColor ("backColor", Color) = (0,0,0,1)
        _cubemap ("cubemap", Cube) = "_Skybox" {}
        _cubemapexp ("cubemap exp", Range(0.3, 10)) = 1.052634
        _colorcubemap ("color cubemap", Color) = (0.5,0.5,0.5,1)
        _finalpower ("final power", Range(0, 1)) = 1
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
            uniform sampler2D _MainTex; uniform float4 _MainTex_ST;
            uniform float4 _TintColor;
            uniform float _power;
            uniform float _speed;
            uniform float _pulsefactor;
            uniform float _size;
            uniform float _exp;
            uniform float4 _addColor;
            uniform float4 _backColor;
            uniform samplerCUBE _cubemap;
            uniform float _cubemapexp;
            uniform float4 _colorcubemap;
            uniform float _finalpower;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 vertexColor : COLOR;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float4 vertexColor : COLOR;
                UNITY_FOG_COORDS(2)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.vertexColor = v.vertexColor;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 normalDirection = i.normalDir;
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
////// Lighting:
////// Emissive:
                float node_1854 = 0.0;
                float2 node_8047 = (float2(i.posWorld.r,i.posWorld.b)/_size);
                float4 _MainTex_var = tex2D(_MainTex,TRANSFORM_TEX(node_8047, _MainTex));
                float4 node_3364 = _Time + _TimeEditor;
                float3 emissive = lerp(float3(node_1854,node_1854,node_1854),((texCUBE(_cubemap,viewReflectDirection).rgb*_colorcubemap.rgb*pow(1.0-max(0,dot(i.normalDir, viewDirection)),_cubemapexp))+((_MainTex_var.rgb*i.vertexColor.rgb*_TintColor.rgb*lerp(((sin((node_3364.r*_speed))*0.5+1.0)*_power),_power,_pulsefactor))+lerp(_backColor.rgb,_addColor.rgb,pow(1.0-max(0,dot(i.normalDir, viewDirection)),_exp)))),_finalpower);
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
