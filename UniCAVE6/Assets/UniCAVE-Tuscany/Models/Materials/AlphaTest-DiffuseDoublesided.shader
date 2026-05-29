Shader "Transparent/Cutout/Diffuse Double sided" {
Properties {
	_Color ("Main Color", Color) = (1,1,1,1)
	_MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
}

SubShader {
	UsePass "Transparent/Cutout/DiffuseBack/FORWARD"
  UsePass "Transparent/Cutout/Diffuse/FORWARD"
}

Fallback "Transparent/Cutout/VertexLit"
}
