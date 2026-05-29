Shader "Unlit/Vertex Color wLighting" {

Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
}

Category {
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }
   
    SubShader {
        Pass {
        
        	Tags { "LightMode" = "Vertex" }
			
			Material
			{
				Diffuse [_Color]
				Ambient [_Color]
			}
			
    		Lighting On
            SetTexture [_MainTex] {
                Combine texture * primary
            }
        }
    }
}
}