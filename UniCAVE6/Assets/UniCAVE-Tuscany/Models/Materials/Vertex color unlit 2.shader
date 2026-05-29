Shader "Unlit/Vertex Color Reflective" {

Properties {
    //_MainTex ("Texture", 2D) = "white" {}
	_MainTex ("Base (RGB), RefStrength (A)", 2D) = "white" {}
	_Reflect ("Reflection", 2D) = "white" { TexGen SphereMap } 
	_ReflectColor ("Reflection Color", Color) = (1,1,1,0.5)
}

Category {
    Tags { "Queue"="Geometry" }
    Lighting Off
    BindChannels {
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
        	Blend One SrcAlpha
            SetTexture [_MainTex]
			{	
				Combine texture * primary, texture * primary
			}
        }
    }
}
}