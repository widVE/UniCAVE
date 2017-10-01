// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.36 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.36;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:33460,y:32578,varname:node_3138,prsc:2|emission-3924-OUT;n:type:ShaderForge.SFN_Color,id:7241,x:32828,y:32566,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Color,id:6504,x:32828,y:32727,ptovrint:False,ptlb:node_6504,ptin:_node_6504,varname:node_6504,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.1081315,c2:0.9191176,c3:0.1081315,c4:1;n:type:ShaderForge.SFN_Lerp,id:3924,x:33259,y:32578,varname:node_3924,prsc:2|A-7241-RGB,B-6504-RGB,T-6239-OUT;n:type:ShaderForge.SFN_TexCoord,id:7105,x:32408,y:32944,varname:node_7105,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ComponentMask,id:8851,x:32583,y:32854,varname:node_8851,prsc:2,cc1:0,cc2:-1,cc3:-1,cc4:-1|IN-7105-V;n:type:ShaderForge.SFN_Add,id:8930,x:32773,y:32944,varname:node_8930,prsc:2|A-8851-OUT,B-4878-OUT;n:type:ShaderForge.SFN_Time,id:9435,x:32391,y:33089,varname:node_9435,prsc:2;n:type:ShaderForge.SFN_Multiply,id:1045,x:32965,y:32944,varname:node_1045,prsc:2|A-1174-OUT,B-8930-OUT,C-7854-OUT;n:type:ShaderForge.SFN_Sin,id:6239,x:33141,y:32944,varname:node_6239,prsc:2|IN-1045-OUT;n:type:ShaderForge.SFN_Vector1,id:1174,x:32743,y:32874,varname:node_1174,prsc:2,v1:2;n:type:ShaderForge.SFN_Tau,id:7854,x:32842,y:33089,varname:node_7854,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4878,x:32602,y:33089,varname:node_4878,prsc:2|A-9435-T,B-5344-OUT;n:type:ShaderForge.SFN_Vector1,id:5344,x:32534,y:33260,varname:node_5344,prsc:2,v1:0.5;proporder:7241-6504;pass:END;sub:END;*/

Shader "Shader Forge/NewShader" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _node_6504 ("node_6504", Color) = (0.1081315,0.9191176,0.1081315,1)
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
                float4 node_9435 = _Time + _TimeEditor;
                float3 emissive = lerp(_Color.rgb,_node_6504.rgb,sin((2.0*(i.uv0.g.r+(node_9435.g*0.5))*6.28318530718)));
                float3 finalColor = emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
