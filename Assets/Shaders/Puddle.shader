Shader "Custom/Puddle"
{
	Properties
	{
		_Color ("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_Speed ("Speed", Float) = 1.0
	}
	
	SubShader
	{
		Tags { "Queue" = "Transparent" }
		
		Blend SrcAlpha OneMinusSrcAlpha
		ZTest Always
		
		CGPROGRAM
		#pragma surface surf Lambert

		float4 _Color;
		sampler2D _MainTex;
		float _Speed;

		struct Input
		{
			float2 uv_MainTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			float Length = length(IN.uv_MainTex - 0.5f);
			
			float CTime = cos(_Time.y * _Speed);
			float STime = sin(_Time.y * _Speed);
			
			float2 UV1 = IN.uv_MainTex + _Time.x;//STime * Length;
			float2 UV2 = IN.uv_MainTex + -_Time.x;//float2(-STime * 0.5f, CTime * 0.25f) * Length;
			
			half4 c = tex2D(_MainTex, UV1) * tex2D(_MainTex, UV2) * _Color;
			o.Albedo = c.rgb;
			o.Alpha = ceil(length(c.rgb) - 0.5f);
		}
		
		ENDCG
	}
	
	FallBack "Diffuse"
}