Shader "Custom/DistanceDarken"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_MaxDist ("Saturation Distance", Float) = 100.0
	}
	
	SubShader
	{
		CGPROGRAM
		
		#pragma surface surf Unlit
		#include "UnityCG.cginc"
		
		float4 _Color;
		sampler2D _MainTex;
		float _MaxDist;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		void surf(Input IN, inout SurfaceOutput o)
		{
			float Length = length(_WorldSpaceCameraPos - IN.worldPos);
			half4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color * saturate(1.0f - Length / _MaxDist);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		half4 LightingUnlit(SurfaceOutput s, half3 lightDir, half atten)
		{
			return half4(s.Albedo, s.Alpha);
		}
		
		ENDCG
	}
	
	FallBack "Self-Illumin/Diffuse"
}