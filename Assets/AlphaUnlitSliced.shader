Shader "Custom Shaders/AlphaUnlitSliced" {
	Properties{
		_MainTex("SelfIllum Color (RGB) Alpha (A)", 2D) = "white"
		_SliceGuide("_SliceGuide (RGB)", 2D) = "white" {}
	_SliceAmount("Slice Amount", Range(0.0, 1.0)) = 0.5
	}
		SubShader{
		Tags{ "RenderType" = "Opaque" }
		Lighting Off
		ZWrite Off
		Cull Back
		CGPROGRAM
		//#pragma surface surf Lambert  //Instead of this line add the next 8
#pragma surface surf NoLighting 
		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}
	struct Input {
		float2 uv_MainTex;
		float2 uv_SliceGuide;
		float _SliceAmount;
	};
	sampler2D _MainTex;
	sampler2D _SliceGuide;
	float _SliceAmount;
	void surf(Input IN, inout SurfaceOutput o) {
		clip(tex2D(_SliceGuide, IN.uv_SliceGuide).rgb - _SliceAmount);
		o.Albedo = tex2D(_MainTex, IN.uv_MainTex).rgb;
	}

	ENDCG
	}
		Fallback "Diffuse"
}