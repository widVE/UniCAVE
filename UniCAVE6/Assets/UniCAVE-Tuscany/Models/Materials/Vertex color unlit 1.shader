Shader "Unlit/Vertex Color Emmissive" {

Properties {
    _Emission ("Emmisive Color", Color) = (0,0,0,0)
    _MainTex ("Texture", 2D) = "white" {}
}

Category {
    Tags { "Queue"="Geometry" }
    Lighting Off
    BindChannels {
        Bind "Color", color
        Bind "Vertex", vertex
        Bind "TexCoord", texcoord
    }
   
    SubShader {
        Pass {
            SetTexture [_MainTex]
            {
                Combine texture * primary DOUBLE
            }
            SetTexture [_MainTex]
			{	
				constantColor [_Emission]
				Combine previous + constant
			}
        }
    }
}
}