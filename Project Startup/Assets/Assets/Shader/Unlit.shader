﻿Shader "My Pipeline/Unlit" {
	Properties {}
	SubShader {
		Pass{
			HLSLPROGRAM

			
			#pragma vertex UnlitPassVertex
			#pragma fragment UnlitPassFragment
			
			#include "Unlit.hlsl"

			ENDLHSL

		}
	}
}