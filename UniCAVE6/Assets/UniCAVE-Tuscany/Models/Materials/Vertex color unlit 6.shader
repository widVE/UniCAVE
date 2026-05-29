Shader "Unlit/Color"
{
	Properties
	{
        _Color ("Main Color", COLOR) = (1,1,1,1)
    }
    
    Category
	{
	    Lighting On
	    Cull Back
	    SubShader
	    {
	        Pass
	        {
				Material
	            {
	                Diffuse [_Color]
	                Ambient [_Color]
	                Emission [_Color]
	            }
	        }
	    }
	}
}