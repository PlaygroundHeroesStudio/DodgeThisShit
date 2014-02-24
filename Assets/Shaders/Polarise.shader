Shader "Custom/Polarise"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	
	SubShader
	{
		CGPROGRAM
		
		#pragma surface Polar Lambert

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
		};

		void InvPolar(Input IN, inout SurfaceOutput o)
		{
			float Angle = IN.uv_MainTex.x * 6.28318f;
			
			float2 UV = 0.5f - float2(sin(Angle), cos(Angle)) * 0.5f * IN.uv_MainTex.y;
			
			half4 c = tex2D(_MainTex, UV);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		
		void Polar(Input IN, inout SurfaceOutput o)
		{
			float2 Diff = IN.uv_MainTex - 0.5f;
			
			float Angle = atan2(Diff.y, Diff.x);
			
			float Length = length(Diff) * 2;
			
			if (Length > 1.0f)
			{
				o.Albedo = 0.0f;
				o.Alpha = 0.0f;
			}
			else
			{
				float2 UV = float2(sin(Angle + 1.5708f) * 0.5f + 0.5f, 1.0f - Length);
				
				half4 c = tex2D(_MainTex, UV);
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
		}
		
		ENDCG
	}
	
	FallBack Off
}