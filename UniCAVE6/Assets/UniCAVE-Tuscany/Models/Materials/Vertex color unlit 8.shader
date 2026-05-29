Shader "Unlit/Vertex Color X4"
{
	Properties
	{
	    _MainTex ("Texture", 2D) = "white" {}
	}
	
	Category
	{
	    Lighting Off
	    Cull Back
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
	            SetTexture [_MainTex]
	            {
	                Combine texture * primary QUAD
	            }
	        }
	    }
	}
}