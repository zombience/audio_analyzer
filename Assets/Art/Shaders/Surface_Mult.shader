Shader "Custom/Surface - Multiply" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_AltTex("Base (RGB)", 2D) = "black" {}
		_mult("mult", Range(0, 1)) = 0
		_scale("scale", float) = 10
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _AltTex;
		float _mult;
		float _scale;

		struct Input {
			float2 uv_MainTex	: TEXCOORD0;
			float2 uv_AltTex	: TEXCOORD1;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * (tex2D(_AltTex, IN.uv_AltTex) * _mult * _scale);
			
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}

Fallback "Legacy Shaders/VertexLit"
}
