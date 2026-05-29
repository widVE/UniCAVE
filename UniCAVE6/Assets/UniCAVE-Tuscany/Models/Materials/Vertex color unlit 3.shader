Shader "Unlit/Vertex Color Reflective wLighting" {

Properties {
    //_MainTex ("Texture", 2D) = "white" {}
    _Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB), RefStrength (A)", 2D) = "white" {}
	_Reflect ("Reflection", 2D) = "white" { TexGen SphereMap } 
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
}

Category
{
    BindChannels
    {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }   
	
    SubShader
    {
    	Pass
        {	
			SetTexture [_Reflect]
			{	
				constantColor [_ReflectColor]
				combine texture * constant
			}
        }
        Pass
        {	
			Tags { "LightMode" = "Vertex" }
			
			Material
			{
				Diffuse [_Color]
				Ambient [_Color]
			}
			
    		Lighting On
    		
        	Blend One SrcAlpha
            SetTexture [_MainTex]
			{	
				Combine texture * primary, texture * primary
			}
        }
    }
}
}