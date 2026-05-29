Shader "Unlit/Vertex Color Multiply" {

Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
    _MainTex ("Texture", 2D) = "white" {}
}

Category {
	Tags { "Queue"="Geometry" }	
    Lighting Off
    
    Fog {Mode Off}
    
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }
   
    SubShader {
        Pass { 
        
            SetTexture [_MainTex] {
                Combine texture * primary
            }
            
            SetTexture [_MainTex] {
            	constantColor [_Color]
				combine previous * constant
            }
        }
    }
}
}