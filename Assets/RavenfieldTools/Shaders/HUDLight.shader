﻿// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "HUD/Light"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent+1"}
		//Blend SrcAlpha One
		ZTest Off
		Cull Off
		Lighting Off
		ZWrite Off

		LOD 100

		Pass
		{
			Stencil {
				Ref 1
				ReadMask 1
				Comp Equal
				Pass Zero
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.color = v.color;
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv)*_TintColor;
				// Unity reports all black vertex colors sometimes, so this randomly bugs out.
				//fixed4 col = tex2D(_MainTex, i.uv)*_TintColor*i.color;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG
		}
	}
}
