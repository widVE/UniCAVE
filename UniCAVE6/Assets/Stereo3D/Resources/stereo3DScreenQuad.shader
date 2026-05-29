Shader "Stereo3D Screen Quad" 
{
	Properties 
	{
	   [NoScaleOffset] _MainTex ("Main Texture", 2D) = "white" {} //Graphics.Blit require this to work correct and not correct with _RightTex
	   [NoScaleOffset] _LeftTex ("Left Texture", 2D) = "white" {}
	   [NoScaleOffset] _RightTex ("Right Texture", 2D) = "white" {}
	   //[NoScaleOffset] _CanvasTex ("Canvas Texture", 2D) = "white" {}

	   _LeftCol ("Left Color", Color) = (1, 0, 0)
	   _RightCol ("Right Color", Color) = (0, 1, 1)

	   _Columns ("Columns", Int) = 1
	   _FirstColumn ("FirstColumn", Int) = 0
	   _Rows ("Rows", Int) = 1
	   _FirstRow ("FirstRow", Int) = 0
	   _OddFrame ("OddFrame", Int) = 0
	   //_Procedural ("Procedural", Int) = 0
	   //_Clockwise ("Clockwise", Int) = 0
	   _Flipped ("Flipped", Int) = 0
	   //_FlipX ("FlipX", Int) = 0
	   //_FlipY ("FlipY", Int) = 0
	   //_GUI ("GUI", Int) = 0
	   _ShiftX ("ShiftX", Float) = 0
	   _ShiftY ("ShiftY", Float) = 0
	}

	SubShader 
	{
		ZWrite Off
		ZTest Always
		Cull Off
        //Blend One OneMinusSrcAlpha

		//pass0 Interleaved Row
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles
		
			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//int _Procedural;
			//int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// // o.pos = float4(verticesPosBuffer[vertexID], 0, 1);
				// // o.uv = verticesUVBuffer[vertexID];

				// //o.pos = float4(0, 0, 0, 1);

				// if (vertexID == 0)
				// {
				// 	o.pos.xy = float2(-1, -1);
				// 	o.uv = float2(0, 0);
				// }
				// else
				// 	if (vertexID == 1)
				// 	{
				// 		o.pos.xy = float2(-1, 1);
				// 		o.uv = float2(0, 1);
				// 	}
				// 	else
				// 		if (vertexID == 2)
				// 		{
				// 			o.pos.xy = float2(1, 1);
				// 			o.uv = float2(1, 1);
				// 		}
				// 		else
				// 			if (vertexID == 3)
				// 			{
				// 				o.pos.xy = float2(1, -1);
				// 				o.uv = float2(1, 0);
				// 			}

				// o.pos = float4(o.pos.xy, 0, 1);

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			int _Rows;
			int _FirstRow;
			//int _GUI;
            // float _ShiftX;
            // float _ShiftY;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			//Texture2DMS<float4, 8>  _LeftTex;
			//Texture2DMS<float4, 8>  _RightTex;
			SamplerState clamp_point_sampler;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				float4 left = _LeftTex.Sample(clamp_point_sampler, i.uv);
				float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	//float4 canvas = _CanvasTex.Sample(clamp_point_sampler, i.uv);
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y + _ShiftY));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	// left = float4(left.rgb * (1 - canvas.a) + canvas.rgb * canvas.a, left.a); //SrcAlpha OneMinusSrcAlpha
				// 	// right = float4(right.rgb * (1 - canvas.a) + canvas.rgb * canvas.a, right.a);
				// 	// left = float4(clamp(left.rgb + canvas.rgb * canvas.a, 0, 1), left.a);
				// 	// right = float4(clamp(right.rgb + canvas.rgb * canvas.a, 0, 1), right.a);
				// 	// left = float4(left.rgb * (1 - canvas.a) + canvas.rgb, left.a); //One OneMinusSrcAlpha
				// 	// right = float4(right.rgb * (1 - canvas.a) + canvas.rgb, right.a);
				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);

				// 	// float3 oneMinusCanvasAlpha = 1 - canvas.a;
				// 	// float3 canvasColorAlpha = canvas.rgb * canvas.a;
				// 	// left = float4(left.rgb * oneMinusCanvasAlpha + canvasColorAlpha, left.a);
				// 	// right = float4(right.rgb * oneMinusCanvasAlpha + canvasColorAlpha, right.a);

				// 	// left = float4(lerp(left.rgb, canvas.rgb, canvas.a), left.a); //SrcAlpha OneMinusSrcAlpha
				// 	// right = float4(lerp(right.rgb, canvas.rgb, canvas.a), right.a);

				// 	// left = float4(pow(lerp(pow(left.rgb, 2.2), pow(canvas.rgb, 2.2), canvas.a), 1/2.2), left.a);
				// 	// right = float4(pow(lerp(pow(right.rgb, 2.2), pow(canvas.rgb, 2.2), canvas.a), 1/2.2), right.a);

				// 	// float3 canvasLinear = pow(canvas.rgb, 2.2);
				// 	// left = float4(lerp(left.rgb, canvasLinear, canvas.a), left.a);
				// 	// right = float4(lerp(right.rgb, canvasLinear, canvas.a), right.a);

				// 	// float outAlphaLeft = canvas.a + left.a * (1 - canvas.a);
				// 	// left = float4((canvas.rgb * canvas.a + left.rgb * left.a * (1 - canvas.a)) / outAlphaLeft, left.a);

				// 	// float outAlphaRight = canvas.a + right.a * (1 - canvas.a);
				// 	// right = float4((canvas.rgb * canvas.a + right.rgb * right.a * (1 - canvas.a)) / outAlphaRight, right.a);

				// }

				//float4 left = _LeftTex.Load(i.pos, 0);
				//float4 right = _RightTex.Load(i.pos, 0);

				//for (int j = 1; j < 8; j++)
				//{
				//	left += _LeftTex.Load(i.pos, j);
				//	right += _RightTex.Load(i.pos, j);
				//}

				//left /= 8;
				//right /= 8;

				//uint row = i.uv.y * _Rows;
				uint row = i.uv.y * _Rows + _FirstRow;
				//uint row = (1 - i.uv.y) * _Rows + _FirstRow;
				//uint odd = row - row / 2 * 2;
				uint odd = row & 1 ? 1 : 0;
				return odd == 0 ? left : right;
				//return row & 1 ? right : left;
			}
			ENDCG
		}

		//pass1 Interleaved Column
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles
		
			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//int _Procedural;
			//int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			int _Columns;
			int _FirstColumn;
			//int _GUI;
            // float _ShiftX;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			SamplerState clamp_point_sampler;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				float4 left = _LeftTex.Sample(clamp_point_sampler, i.uv);
				float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);
				// }

				//uint column = i.uv.x * _Columns;
				uint column = i.uv.x * _Columns + _FirstColumn;
				//uint odd = column - column / 2 * 2;
				uint odd = column & 1 ? 1 : 0;
				return odd == 0 ? left : right;
				//return column & 1 ? right : left;
			}
			ENDCG
		}

		//pass2 Interleaved Checkerboard
		Pass 
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles
		
			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//int _Procedural;
			//int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			int _Columns;
			int _FirstColumn;
			int _Rows;
			int _FirstRow;
			//int _GUI;
            // float _ShiftX;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			SamplerState clamp_point_sampler;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				float4 left = _LeftTex.Sample(clamp_point_sampler, i.uv);
				float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);
				// }

				// uint row = i.uv.y * _Rows;
				// uint column = i.uv.x * _Columns;
				uint row = i.uv.y * _Rows + _FirstRow;
				uint column = i.uv.x * _Columns + _FirstColumn;
				//uint oddRow = row - row / 2 * 2;
				uint oddRow = row & 1 ? 1 : 0;
				//uint oddColumn = column - column / 2 * 2;
				uint oddColumn = column & 1 ? 1 : 0;
				//uint switcher = oddRow + oddColumn - 2 * oddRow * oddColumn;

				//return switcher == 0 ? left : right;
				return oddRow ^ oddColumn ? left : right;
			}
			ENDCG
		}
   
		//pass3 Side By Side
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//int _Procedural;
			//int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			//int _GUI;
            //float _ShiftX;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			//SamplerState repeat_point_sampler;
			SamplerState clamp_point_sampler;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				// //return i.uv.x < 1 ? _LeftTex.Sample(repeat_point_sampler, i.uv) : _RightTex.Sample(repeat_point_sampler, i.uv);
				// //return i.uv.x < 1 ? _LeftTex.Sample(repeat_point_sampler, i.uv) : _RightTex.Sample(repeat_point_sampler, i.uv - float2(1, 0));
				// return i.uv.x < 0.5f ? _LeftTex.Sample(clamp_point_sampler, float2(i.uv.x * 2, i.uv.y)) : _RightTex.Sample(clamp_point_sampler, float2(i.uv.x * 2 - 1, i.uv.y));

				float4 left = _LeftTex.Sample(clamp_point_sampler, float2(i.uv.x * 2, i.uv.y));
				float4 right = _RightTex.Sample(clamp_point_sampler, float2(i.uv.x * 2 - 1, i.uv.y));

				// if (_GUI == 1)
				// {
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2((i.uv.x + _ShiftX) * 2, i.uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2((i.uv.x - _ShiftX) * 2 - 1, i.uv.y));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);
				// }

				return i.uv.x < 0.5f ? left : right;
			}
			ENDCG
		}

		//pass4 Over Under
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			//int _Procedural;
			//int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			//int _GUI;
            //float _ShiftX;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			//SamplerState repeat_point_sampler;
			SamplerState clamp_point_sampler;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				// //return i.uv.y < 1 ? _LeftTex.Sample(repeat_point_sampler, i.uv) : _RightTex.Sample(repeat_point_sampler, i.uv);
				// //return i.uv.y < 1 ? _LeftTex.Sample(repeat_point_sampler, i.uv) : _RightTex.Sample(repeat_point_sampler, i.uv - float2(0, 1));
				// return i.uv.y < 0.5f ? _LeftTex.Sample(clamp_point_sampler, float2(i.uv.x, i.uv.y * 2)) : _RightTex.Sample(clamp_point_sampler, float2(i.uv.x, i.uv.y * 2 - 1));

				float4 left = _LeftTex.Sample(clamp_point_sampler, float2(i.uv.x, i.uv.y * 2));
				float4 right = _RightTex.Sample(clamp_point_sampler, float2(i.uv.x, i.uv.y * 2 - 1));

				// if (_GUI == 1)
				// {
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2((i.uv.x + _ShiftX), i.uv.y * 2));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2((i.uv.x - _ShiftX), i.uv.y * 2 - 1));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);
				// }

				return i.uv.y < 0.5f ? left : right;
			}
			ENDCG
		}

		//pass5 Sequential
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// int _Procedural;
			// int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			//int _GUI;
            //float _ShiftX;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			int _OddFrame;
			SamplerState clamp_point_sampler;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				//return _OddFrame == 0 ? _LeftTex.Sample(clamp_point_sampler, i.uv) : _RightTex.Sample(clamp_point_sampler, i.uv);

				float4 left = _LeftTex.Sample(clamp_point_sampler, i.uv);
				float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);
				// }

				return _OddFrame == 0 ? left : right;
			}
			ENDCG
		}

		//pass6 Anaglyph
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			// StructuredBuffer<float2> verticesPosBuffer;
			// StructuredBuffer<float2> verticesUVBuffer;

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// int _Procedural;
			// int _Clockwise;

			//vertexDataOutput vert(uint vertexID : SV_VertexID)
			//vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	float2 pos;
				// 	float2 uv;

				// 	if (_Clockwise == 1)
				// 	{
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 					}
				// 	}
				// 	else
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;
				}

				return o;
			}

			//int _GUI;
            //float _ShiftX;
			Texture2D  _LeftTex;
			Texture2D  _RightTex;
			//Texture2D  _CanvasTex;
			SamplerState clamp_point_sampler;
			float4 _LeftCol;
			float4 _RightCol;

			float4 frag(vertexDataOutput i) : SV_Target
			{
				// float4 leftColor = _LeftTex.Sample(clamp_point_sampler, i.uv) * _LeftCol;
				// float4 rightColor = _RightTex.Sample(clamp_point_sampler, i.uv) * _RightCol;
				//return leftColor + rightColor;

				float4 left = _LeftTex.Sample(clamp_point_sampler, i.uv);
				float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a);
				// }

				left *= _LeftCol;
				right *= _RightCol;

				return left + right;
			}
			ENDCG
		}

		//pass7 Method.Two_Displays & Method.Two_Displays_MirrorX & Method.Two_Displays_MirrorY with flip
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// int _Procedural;
			// int _Clockwise;
			// int _FlipX;
			// int _FlipY;
			int _Flipped;

			// //Output main(uint vertexID : SV_VertexID)
			// //float4 vert(uint vertexID : SV_VertexID, out float3 color : COLOR, out float2 uv : TEXCOORD0) : SV_POSITION
			// //float4 vert(uint vertexID : SV_VertexID, out float2 uv : TEXCOORD0) : SV_POSITION
			// //vertexDataOutput vert(uint vertexID : SV_VertexID)
			// vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			// //vertexDataOutput vert(uint vertexID : SV_VertexID, float3 pos : POSITION, float2 uv : TEXCOORD0)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	//uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0

				// 	float2 pos;
				// 	float2 uv;

				// 	// if (vertexID == 0)
				// 	// {
				// 	// 	pos = float2(-1, -1);
				// 	// 	uv = float2(0, 0);
				// 	// 	//uv = float2(1, 0);
				// 	// }
				// 	// else
				// 	// 	if (vertexID == 1)
				// 	// 	{
				// 	// 		pos = float2(-1, 1);
				// 	// 		uv = float2(0, 1);
				// 	// 		//uv = float2(1, 1);
				// 	// 	}
				// 	// 	else
				// 	// 		if (vertexID == 2)
				// 	// 		{
				// 	// 			pos = float2(1, 1);
				// 	// 			uv = float2(1, 1);
				// 	// 			//uv = float2(0, 1);
				// 	// 		}
				// 	// 		else
				// 	// 			if (vertexID == 3)
				// 	// 			{
				// 	// 				pos = float2(1, -1);
				// 	// 				uv = float2(1, 0);
				// 	// 				//uv = float2(0, 0);
				// 	// 			}

				// 	if (_Clockwise == 1)
				// 	{
				// 		//clockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 			//uv = float2(1, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 				//uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 					//uv = float2(0, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 						//uv = float2(0, 0);
				// 					}
				// 	}
				// 	else
				// 		//counterclockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	if (_FlipX != 0)
				// 		//uv.x = uv.x * -1 + 1;
				// 		uv.x = 1 - uv.x;

				// 	if (_FlipY != 0)
				// 		//uv.y = uv.y * -1 + 1;
				// 		uv.y = 1 - uv.y;

				// 	//Output output;
				// 	//output.uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0
				// 	//output.color = float4(output.uv, 1, 1);
				// 	//output.pos = float4(output.uv, 0, 1);
				// 	//return output;

				// 	// //color = float3(uv, 1);
				// 	// //color = float3(1, 1, 1);
				// 	// //return float4(uv * 2 - 1, 0, 1);
				// 	// return float4(pos, 0, 1);

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					//o.pos = float4(i.pos, 1);
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;

					// o.pos = float4(pos.x * 2 - 1, pos.y * 2 - 1, pos.z, 1);
					// o.uv = uv;

					if (_Flipped != 0)
						o.uv.y = 1 - o.uv.y;
				}

				// if (_FlipX != 0)
				// 	o.uv.x = 1 - o.uv.x;

				// if (_FlipY != 0)
				// 	o.uv.y = 1 - o.uv.y;

				return o;
			}

			//struct Input
			//{
			//	float4 pos : SV_POSITION;
			//	float2 coords : TEXCOORD0;
			//	float4 color : COLOR;
			//};

			//Texture2D SimpleTexture : register(t0);
			//int _GUI;
            //float _ShiftX;
			Texture2D _MainTex;
			//Texture2D  _CanvasTex;
			//Texture2D  _LeftTex;
			SamplerState clamp_point_sampler : register(s0);

			//float4 main(Input input) : SV_TARGET
			//float4 main(float4 pos : SV_POSITION, float3 color : COLOR) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float3 color : COLOR, float2 uv : TEXCOORD0) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float2 uv : TEXCOORD0) : SV_TARGET
			float4 frag(vertexDataOutput i) : SV_TARGET
			{
				// // //return float4(color, 1);
				// // //return input.color;
				// // return _MainTex.Sample(clamp_point_sampler, uv);
				// // //return _LeftTex.Sample(clamp_point_sampler, uv);

				// // //float4 left = _LeftTex.Sample(clamp_point_sampler, i.uv);
				// // float4 left = _MainTex.Sample(clamp_point_sampler, uv);
				// // //float4 left = _MainTex.Sample(clamp_point_sampler, i.uv);

				// float4 left;

				// if (_Flipped == 1)
				// 	//left = _MainTex.Sample(clamp_point_sampler, float2(uv.x, 1 - uv.y));
				// 	left = _MainTex.Sample(clamp_point_sampler, float2(i.uv.x, 1 - i.uv.y));
				// else
				// 	//left = _MainTex.Sample(clamp_point_sampler, uv);
				// 	left = _MainTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	//float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(uv.x + _ShiftX, uv.y));
				// 	float4 canvasLeft = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y));

				// 	left = float4(left.rgb * (1 - canvasLeft.a) + canvasLeft.rgb, left.a); //One OneMinusSrcAlpha
				// }

				// return left;
				return _MainTex.Sample(clamp_point_sampler, i.uv);
			}
			ENDCG
		}

		//pass8 Method.Two_Displays & Method.Two_Displays_MirrorX & Method.Two_Displays_MirrorY without flip
		Pass
		{
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// int _Procedural;
			// int _Clockwise;
			int _Flipped;

			// //Output main(uint vertexID : SV_VertexID)
			// //float4 vert(uint vertexID : SV_VertexID, out float3 color : COLOR, out float2 uv : TEXCOORD0) : SV_POSITION
			// //float4 vert(uint vertexID : SV_VertexID, out float2 uv : TEXCOORD0) : SV_POSITION
			// //vertexDataOutput vert(uint vertexID : SV_VertexID)
			// vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	//uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0

				// 	float2 pos;
				// 	float2 uv;

				// 	// if (vertexID == 0)
				// 	// {
				// 	// 	pos = float2(-1, -1);
				// 	// 	uv = float2(0, 0);
				// 	// 	//uv = float2(1, 0);
				// 	// }
				// 	// else
				// 	// 	if (vertexID == 1)
				// 	// 	{
				// 	// 		pos = float2(-1, 1);
				// 	// 		uv = float2(0, 1);
				// 	// 		//uv = float2(1, 1);
				// 	// 	}
				// 	// 	else
				// 	// 		if (vertexID == 2)
				// 	// 		{
				// 	// 			pos = float2(1, 1);
				// 	// 			uv = float2(1, 1);
				// 	// 			//uv = float2(0, 1);
				// 	// 		}
				// 	// 		else
				// 	// 			if (vertexID == 3)
				// 	// 			{
				// 	// 				pos = float2(1, -1);
				// 	// 				uv = float2(1, 0);
				// 	// 				//uv = float2(0, 0);
				// 	// 			}

				// 	if (_Clockwise == 1)
				// 	{
				// 		//clockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 			//uv = float2(1, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 				//uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 					//uv = float2(0, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 						//uv = float2(0, 0);
				// 					}
				// 	}
				// 	else
				// 		//counterclockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	//Output output;
				// 	//output.uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0
				// 	//output.color = float4(output.uv, 1, 1);
				// 	//output.pos = float4(output.uv, 0, 1);
				// 	//return output;

				// 	// //color = float3(uv, 1);
				// 	// //color = float3(1, 1, 1);
				// 	// //return float4(uv * 2 - 1, 0, 1);
				// 	// return float4(pos, 0, 1);

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					//o.pos = float4(i.pos, 1);
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					o.uv = i.uv;

					if (_Flipped != 0)
						o.uv.y = 1 - o.uv.y;
				}

				return o;
			}

			//struct Input
			//{
			//	float4 pos : SV_POSITION;
			//	float2 coords : TEXCOORD0;
			//	float4 color : COLOR;
			//};

			//Texture2D SimpleTexture : register(t0);
			//int _GUI;
            //float _ShiftX;
			Texture2D _MainTex;
			//Texture2D  _CanvasTex;
			//Texture2D  _RightTex;
			SamplerState clamp_point_sampler : register(s0);

			//float4 main(Input input) : SV_TARGET
			//float4 main(float4 pos : SV_POSITION, float3 color : COLOR) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float3 color : COLOR, float2 uv : TEXCOORD0) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float2 uv : TEXCOORD0) : SV_TARGET
			float4 frag(vertexDataOutput i) : SV_TARGET
			{
				// // //return float4(color, 1);
				// // //return input.color;
				// // return _MainTex.Sample(clamp_point_sampler, uv);
				// // //return _RightTex.Sample(clamp_point_sampler, uv);

				// // //float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);
				// // float4 right = _MainTex.Sample(clamp_point_sampler, uv);
				// // //float4 right = _MainTex.Sample(clamp_point_sampler, i.uv);

				// float4 right;

				// if (_Flipped == 1)
				// 	//right = _MainTex.Sample(clamp_point_sampler, float2(uv.x, 1 - uv.y));
				// 	right = _MainTex.Sample(clamp_point_sampler, float2(i.uv.x, 1 - i.uv.y));
				// else
				// 	//right = _MainTex.Sample(clamp_point_sampler, uv);
				// 	right = _MainTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	//float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(uv.x - _ShiftX, uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a); //One OneMinusSrcAlpha
				// }

				// return right;
				return _MainTex.Sample(clamp_point_sampler, i.uv);
			}
			ENDCG
		}

		//pass9 CanvasBlit left
		Pass
		{
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// int _Procedural;
			// int _Clockwise;
            float _ShiftX;
            float _ShiftY;
			//int _Flipped;

			// //Output main(uint vertexID : SV_VertexID)
			// //float4 vert(uint vertexID : SV_VertexID, out float3 color : COLOR, out float2 uv : TEXCOORD0) : SV_POSITION
			// //float4 vert(uint vertexID : SV_VertexID, out float2 uv : TEXCOORD0) : SV_POSITION
			// //vertexDataOutput vert(uint vertexID : SV_VertexID)
			// vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	//uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0

				// 	float2 pos;
				// 	float2 uv;

				// 	// if (vertexID == 0)
				// 	// {
				// 	// 	pos = float2(-1, -1);
				// 	// 	uv = float2(0, 0);
				// 	// 	//uv = float2(1, 0);
				// 	// }
				// 	// else
				// 	// 	if (vertexID == 1)
				// 	// 	{
				// 	// 		pos = float2(-1, 1);
				// 	// 		uv = float2(0, 1);
				// 	// 		//uv = float2(1, 1);
				// 	// 	}
				// 	// 	else
				// 	// 		if (vertexID == 2)
				// 	// 		{
				// 	// 			pos = float2(1, 1);
				// 	// 			uv = float2(1, 1);
				// 	// 			//uv = float2(0, 1);
				// 	// 		}
				// 	// 		else
				// 	// 			if (vertexID == 3)
				// 	// 			{
				// 	// 				pos = float2(1, -1);
				// 	// 				uv = float2(1, 0);
				// 	// 				//uv = float2(0, 0);
				// 	// 			}

				// 	if (_Clockwise == 1)
				// 	{
				// 		//clockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 			//uv = float2(1, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 				//uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 					//uv = float2(0, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 						//uv = float2(0, 0);
				// 					}
				// 	}
				// 	else
				// 		//counterclockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	//Output output;
				// 	//output.uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0
				// 	//output.color = float4(output.uv, 1, 1);
				// 	//output.pos = float4(output.uv, 0, 1);
				// 	//return output;

				// 	// //color = float3(uv, 1);
				// 	// //color = float3(1, 1, 1);
				// 	// //return float4(uv * 2 - 1, 0, 1);
				// 	// return float4(pos, 0, 1);

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					//o.pos = float4(i.pos, 1);
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					//o.uv = i.uv;
					o.uv = float2(i.uv.x + _ShiftX, i.uv.y + _ShiftY);

					// if (_Flipped != 0)
					// 	o.uv.y = 1 - o.uv.y;
				}

				return o;
			}

			//struct Input
			//{
			//	float4 pos : SV_POSITION;
			//	float2 coords : TEXCOORD0;
			//	float4 color : COLOR;
			//};

			//Texture2D SimpleTexture : register(t0);
			//int _GUI;
            // float _ShiftX;
            // float _ShiftY;
			Texture2D _MainTex;
			//Texture2D  _CanvasTex;
			//Texture2D  _RightTex;
			SamplerState clamp_point_sampler : register(s0);

			//float4 main(Input input) : SV_TARGET
			//float4 main(float4 pos : SV_POSITION, float3 color : COLOR) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float3 color : COLOR, float2 uv : TEXCOORD0) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float2 uv : TEXCOORD0) : SV_TARGET
			float4 frag(vertexDataOutput i) : SV_TARGET
			{
				// // //return float4(color, 1);
				// // //return input.color;
				// // return _MainTex.Sample(clamp_point_sampler, uv);
				// // //return _RightTex.Sample(clamp_point_sampler, uv);

				// // //float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);
				// // float4 right = _MainTex.Sample(clamp_point_sampler, uv);
				// // //float4 right = _MainTex.Sample(clamp_point_sampler, i.uv);

				// float4 right;

				// if (_Flipped == 1)
				// 	//right = _MainTex.Sample(clamp_point_sampler, float2(uv.x, 1 - uv.y));
				// 	right = _MainTex.Sample(clamp_point_sampler, float2(i.uv.x, 1 - i.uv.y));
				// else
				// 	//right = _MainTex.Sample(clamp_point_sampler, uv);
				// 	right = _MainTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	//float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(uv.x - _ShiftX, uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a); //One OneMinusSrcAlpha
				// }

				// return right;
				return _MainTex.Sample(clamp_point_sampler, i.uv);
				//return _MainTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y + _ShiftY));
			}
			ENDCG
		}

		//pass10 CanvasBlit right
		Pass
		{
			Blend One OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			//#pragma exclude_renderers gles

			struct vertexDataInput
			{
				//uint vertexID : SV_VertexID;
				float3 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct vertexDataOutput
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			// int _Procedural;
			// int _Clockwise;
            float _ShiftX;
            //float _ShiftY;
			//int _Flipped;

			// //Output main(uint vertexID : SV_VertexID)
			// //float4 vert(uint vertexID : SV_VertexID, out float3 color : COLOR, out float2 uv : TEXCOORD0) : SV_POSITION
			// //float4 vert(uint vertexID : SV_VertexID, out float2 uv : TEXCOORD0) : SV_POSITION
			// //vertexDataOutput vert(uint vertexID : SV_VertexID)
			// vertexDataOutput vert(uint vertexID : SV_VertexID, vertexDataInput i)
			vertexDataOutput vert(vertexDataInput i)
			{
				vertexDataOutput o;

				// if (_Procedural)
				// {
				// 	//uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0

				// 	float2 pos;
				// 	float2 uv;

				// 	// if (vertexID == 0)
				// 	// {
				// 	// 	pos = float2(-1, -1);
				// 	// 	uv = float2(0, 0);
				// 	// 	//uv = float2(1, 0);
				// 	// }
				// 	// else
				// 	// 	if (vertexID == 1)
				// 	// 	{
				// 	// 		pos = float2(-1, 1);
				// 	// 		uv = float2(0, 1);
				// 	// 		//uv = float2(1, 1);
				// 	// 	}
				// 	// 	else
				// 	// 		if (vertexID == 2)
				// 	// 		{
				// 	// 			pos = float2(1, 1);
				// 	// 			uv = float2(1, 1);
				// 	// 			//uv = float2(0, 1);
				// 	// 		}
				// 	// 		else
				// 	// 			if (vertexID == 3)
				// 	// 			{
				// 	// 				pos = float2(1, -1);
				// 	// 				uv = float2(1, 0);
				// 	// 				//uv = float2(0, 0);
				// 	// 			}

				// 	if (_Clockwise == 1)
				// 	{
				// 		//clockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 0);
				// 			//uv = float2(1, 0);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(-1, 1);
				// 				uv = float2(0, 1);
				// 				//uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 1);
				// 					//uv = float2(0, 1);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(1, -1);
				// 						uv = float2(1, 0);
				// 						//uv = float2(0, 0);
				// 					}
				// 	}
				// 	else
				// 		//counterclockwise
				// 		if (vertexID == 0)
				// 		{
				// 			pos = float2(-1, -1);
				// 			uv = float2(0, 1);
				// 		}
				// 		else
				// 			if (vertexID == 1)
				// 			{
				// 				pos = float2(1, -1);
				// 				uv = float2(1, 1);
				// 			}
				// 			else
				// 				if (vertexID == 2)
				// 				{
				// 					pos = float2(1, 1);
				// 					uv = float2(1, 0);
				// 				}
				// 				else
				// 					if (vertexID == 3)
				// 					{
				// 						pos = float2(-1, 1);
				// 						uv = float2(0, 0);
				// 					}

				// 	//Output output;
				// 	//output.uv = float2(
				// 	//	vertexID & 1 ? 0 : 1,  // x: 0 | 1 | 0 | 1
				// 	//	vertexID & 2 ? 1 : 0); // y: 1 | 1 | 0 | 0
				// 	//output.color = float4(output.uv, 1, 1);
				// 	//output.pos = float4(output.uv, 0, 1);
				// 	//return output;

				// 	// //color = float3(uv, 1);
				// 	// //color = float3(1, 1, 1);
				// 	// //return float4(uv * 2 - 1, 0, 1);
				// 	// return float4(pos, 0, 1);

				// 	o.pos = float4(pos, 0, 1);
				// 	o.uv = uv;
				// }
				// else
				{
					//o.pos = float4(i.pos, 1);
					o.pos = float4(i.pos.x * 2 - 1, i.pos.y * 2 - 1, i.pos.z, 1);
					//o.uv = i.uv;
					o.uv = float2(i.uv.x - _ShiftX, i.uv.y);

					// if (_Flipped != 0)
					// 	o.uv.y = 1 - o.uv.y;
				}

				return o;
			}

			//struct Input
			//{
			//	float4 pos : SV_POSITION;
			//	float2 coords : TEXCOORD0;
			//	float4 color : COLOR;
			//};

			//Texture2D SimpleTexture : register(t0);
			//int _GUI;
            // float _ShiftX;
            // float _ShiftY;
			Texture2D _MainTex;
			//Texture2D  _CanvasTex;
			//Texture2D  _RightTex;
			SamplerState clamp_point_sampler : register(s0);

			//float4 main(Input input) : SV_TARGET
			//float4 main(float4 pos : SV_POSITION, float3 color : COLOR) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float3 color : COLOR, float2 uv : TEXCOORD0) : SV_TARGET
			//float4 frag(float4 pos : SV_POSITION, float2 uv : TEXCOORD0) : SV_TARGET
			float4 frag(vertexDataOutput i) : SV_TARGET
			{
				// // //return float4(color, 1);
				// // //return input.color;
				// // return _MainTex.Sample(clamp_point_sampler, uv);
				// // //return _RightTex.Sample(clamp_point_sampler, uv);

				// // //float4 right = _RightTex.Sample(clamp_point_sampler, i.uv);
				// // float4 right = _MainTex.Sample(clamp_point_sampler, uv);
				// // //float4 right = _MainTex.Sample(clamp_point_sampler, i.uv);

				// float4 right;

				// if (_Flipped == 1)
				// 	//right = _MainTex.Sample(clamp_point_sampler, float2(uv.x, 1 - uv.y));
				// 	right = _MainTex.Sample(clamp_point_sampler, float2(i.uv.x, 1 - i.uv.y));
				// else
				// 	//right = _MainTex.Sample(clamp_point_sampler, uv);
				// 	right = _MainTex.Sample(clamp_point_sampler, i.uv);

				// if (_GUI == 1)
				// {
				// 	//float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(uv.x - _ShiftX, uv.y));
				// 	float4 canvasRight = _CanvasTex.Sample(clamp_point_sampler, float2(i.uv.x - _ShiftX, i.uv.y));

				// 	right = float4(right.rgb * (1 - canvasRight.a) + canvasRight.rgb, right.a); //One OneMinusSrcAlpha
				// }

				// return right;
				return _MainTex.Sample(clamp_point_sampler, i.uv);
				//return _MainTex.Sample(clamp_point_sampler, float2(i.uv.x + _ShiftX, i.uv.y + _ShiftY));
			}
			ENDCG
		}
	}   
} 
