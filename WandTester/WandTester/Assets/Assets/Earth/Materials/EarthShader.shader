Shader "Custom/EarthShader"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Bumpmap", 2D) = "bump" {}
		_Detail("Detail", 2D) = "black" {}
		_DetailIntensity("Detail Intensity", Range(0, 3)) = 0.0
		_SpecularTex("Specular (RGB)", 2D) = "black" {}
		_SpecularPower("Specular Power", Range(0, 8)) = 0.0
		_NightTex("Night Detail (RGB)", 2D) = "black" {}
		_NightIntensity("Night Detail Intensity", Range(0, 1)) = 0.0
		_NightTransitionVariable("Night Transition Variable", Range(1, 64)) = 4
		_Smoothness("Smoothness", Range(0,1)) = 0.5
		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5, 64.0)) = 3.0
		_AtmosNear("Atmosphere Near Color", Color) = (0.1686275,0.7372549,1,1)
		_AtmosFar("Atmosphere Far Color", Color) = (0.4557808,0.5187039,0.9850746,1)
		_AtmosFalloff("Atmosphere Falloff", Range(0.1, 64)) = 12
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" "Queue" = "Geometry" "IgnoreProjection" = "False" }
		LOD 200

		CGPROGRAM

		#pragma surface surf CustomLighting
		#pragma target 3.0

		sampler2D _MainTex;
		sampler2D _BumpMapTex;
		sampler2D _DetailTex;
		sampler2D _SpecularTex;
		sampler2D _NightTex;

		struct Input
		{
			fixed3 viewDir;
			fixed2 uv_MainTex;
			fixed2 uv_DetailTex;
		};

		struct SurfaceOutputStandardSpecularCustom
		{
			fixed3 Albedo;      // diffuse color
			fixed3 Specular;    // specular color
			fixed3 Normal;      // tangent space normal, if written
			fixed3 Emission;
			fixed Smoothness;    // 0=rough, 1=smooth
			fixed Occlusion;     // occlusion (default 1)
			fixed Alpha;        // alpha for transparencies
			fixed3 NightColor;
			fixed4 ExtraColor;
		};

		fixed4 _Color;
		fixed _Smoothness;
		fixed _DetailIntensity;
		fixed _SpecularPower;
		fixed _NightIntensity;
		fixed _NightTransitionVariable;
		fixed4 _RimColor;
		fixed _RimPower;
		fixed4 _AtmosNear;
		fixed4 _AtmosFar;
		fixed _AtmosFalloff;

		/*
		inline half4 LightingCustomLighting_PrePass(SurfaceOutputStandardSpecularCustom s, half4 light)
		{
			fixed spec = light.a * s.Smoothness;
			fixed4 c;
			c.rgb = (lerp(s.NightColor, s.Albedo * light.rgb * s.Specular.rgb * _SpecularPower, saturate(_NightTransitionVariable * light.a)));
			c.a = s.Alpha;
			return c;
		}
		*/

		inline half4 LightingCustomLighting(SurfaceOutputStandardSpecularCustom s, fixed3 lightDir, fixed3 viewDir, fixed atten)
		{
			fixed3 h = normalize(lightDir + viewDir);
			fixed d = max(0, dot(s.Normal, lightDir));
			fixed diffuseMultiplier = d * atten;
			fixed nh = max(0, dot(s.Normal, h));
			fixed spec = pow(nh, 48.0) * s.Smoothness;
			fixed4 c;
			fixed3 dayColor = (s.Albedo * _LightColor0.rgb * diffuseMultiplier) + (_LightColor0.rgb * spec * _SpecularPower);
			c.rgb = lerp(s.NightColor, dayColor, saturate(_NightTransitionVariable * diffuseMultiplier));
			c.a = s.Alpha;
			c.rgb += (s.ExtraColor.rgb * atten * nh * s.ExtraColor.a);

			return c;
		}

		inline fixed3 AtmosphereColor(Input IN)
		{
			fixed4 Fresnel1 = fixed4(0, 0, 1, 1);
			fixed4 Fresnel2 = (1.0 - dot(normalize(fixed4(IN.viewDir.xyz, 0)), normalize(Fresnel1))).xxxx;
			fixed4 Pow0 = pow(Fresnel2, _AtmosFalloff);
			fixed4 Saturate0 = saturate(Pow0);
			fixed4 Lerp0 = lerp(_AtmosNear, _AtmosFar, Saturate0);
			fixed4 color = Lerp0 * Saturate0;

			return color;
		}

		void surf(Input IN, inout SurfaceOutputStandardSpecularCustom o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
			o.NightColor = tex2D(_NightTex, IN.uv_MainTex).rgb * _NightIntensity;
			fixed maxNight = min(1, o.NightColor.g + 0.8);
			o.NightColor *= pow(maxNight, 4);
			o.Albedo = c.rgb += (tex2D(_DetailTex, IN.uv_DetailTex).rgb * _DetailIntensity);
			o.Normal = UnpackNormal(tex2D(_BumpMapTex, IN.uv_MainTex));
			o.Specular = tex2D(_SpecularTex, IN.uv_MainTex);
			o.Smoothness = _Smoothness;
			o.Alpha = c.a;
			fixed rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
			o.ExtraColor = fixed4(_RimColor.rgb * pow(rim, _RimPower) + AtmosphereColor(IN), 2);
		}

		ENDCG
	}
	FallBack "Diffuse"
}