// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33261,y:32450,varname:node_3138,prsc:2|emission-1374-OUT;n:type:ShaderForge.SFN_Color,id:646,x:32564,y:32456,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:6112,x:32564,y:32628,ptovrint:False,ptlb:node_6504,ptin:_node_6504,varname:node_6504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.9558824,c2:0.1204063,c3:0,c4:1;n:type:ShaderForge.SFN_Lerp,id:1374,x:33038,y:32613,varname:node_1374,prsc:2|A-646-RGB,B-6112-RGB,T-5026-OUT;n:type:ShaderForge.SFN_TexCoord,id:8467,x:32204,y:32889,varname:node_8467,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ComponentMask,id:8854,x:32376,y:32889,varname:node_8854,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-8467-V;n:type:ShaderForge.SFN_Add,id:6158,x:32585,y:32889,varname:node_6158,prsc:2|A-8854-OUT,B-5590-OUT;n:type:ShaderForge.SFN_Time,id:3903,x:32204,y:33074,varname:node_3903,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1483,x:32775,y:32842,varname:node_1483,prsc:2|A-8146-OUT,B-6158-OUT,C-370-OUT;n:type:ShaderForge.SFN_Sin,id:5026,x:32983,y:32842,varname:node_5026,prsc:2|IN-1483-OUT;n:type:ShaderForge.SFN_Vector1,id:8146,x:32513,y:32796,varname:node_8146,prsc:2,v1:2;n:type:ShaderForge.SFN_Tau,id:370,x:32601,y:33026,varname:node_370,prsc:2;n:type:ShaderForge.SFN_Multiply,id:5590,x:32437,y:33074,varname:node_5590,prsc:2|A-3903-T,B-6489-OUT;n:type:ShaderForge.SFN_Vector1,id:6489,x:32204,y:33208,varname:node_6489,prsc:2,v1:0.5;proporder:646-6112;pass:END;sub:END;*/

Shader "Shader Forge/RedShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _node_6504 ("node_6504", Color) = (0.9558824,0.1204063,0,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _TimeEditor;
            uniform float4 _Color;
            uniform float4 _node_6504;
            struct VertexInput {
                float4 vertex : POSITION;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.pos = UnityObjectToClipPos(v.vertex );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
////// Lighting:
////// Emissive:
                float4 node_3903 = _Time + _TimeEditor;
                float3 emissive = lerp(_Color.rgb,_node_6504.rgb,sin((2.0*(i.uv0.g.r+(node_3903.g*0.5))*6.28318530718)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
