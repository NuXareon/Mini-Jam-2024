Shader "Platform Essentials/Wobble Effect"
{
	Properties
	{
		[SingleLineTexture]_MainTex("Shape Template", 2D) = "white" {}
		[SingleLineTexture]_MainTexture("Main Texture", 2D) = "white" {}
		[HDR]_Tint("Tint", Color) = (0,1.397985,2,1)
		_Scale("Scale", Range( 0 , 10)) = 1
		_Speed("Speed", Range( 0 , 1)) = 0.1
		_Opacity("Opacity", Range( 0 , 1)) = 0.17
		_TwirlSpeed("Twirl Speed", Range( 0 , 0.5)) = 0.15
		_DistortionScale("Distortion Scale", Range( 0 , 20)) = 5
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		GrabPass{ }
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#if defined(UNITY_STEREO_INSTANCING_ENABLED) || defined(UNITY_STEREO_MULTIVIEW_ENABLED)
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex);
		#else
		#define ASE_DECLARE_SCREENSPACE_TEXTURE(tex) UNITY_DECLARE_SCREENSPACE_TEXTURE(tex)
		#endif
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		uniform float4 _Tint;
		uniform sampler2D _MainTexture;
		uniform float _Scale;
		uniform float _Speed;
		uniform float _TwirlSpeed;
		uniform float _DistortionScale;
		uniform float _Opacity;
		ASE_DECLARE_SCREENSPACE_TEXTURE( _GrabTexture )
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Cutoff = 0.5;


		float2 UnityGradientNoiseDir( float2 p )
		{
			p = fmod(p , 289);
			float x = fmod((34 * p.x + 1) * p.x , 289) + p.y;
			x = fmod( (34 * x + 1) * x , 289);
			x = frac( x / 41 ) * 2 - 1;
			return normalize( float2(x - floor(x + 0.5 ), abs( x ) - 0.5 ) );
		}
		
		float UnityGradientNoise( float2 UV, float Scale )
		{
			float2 p = UV * Scale;
			float2 ip = floor( p );
			float2 fp = frac( p );
			float d00 = dot( UnityGradientNoiseDir( ip ), fp );
			float d01 = dot( UnityGradientNoiseDir( ip + float2( 0, 1 ) ), fp - float2( 0, 1 ) );
			float d10 = dot( UnityGradientNoiseDir( ip + float2( 1, 0 ) ), fp - float2( 1, 0 ) );
			float d11 = dot( UnityGradientNoiseDir( ip + float2( 1, 1 ) ), fp - float2( 1, 1 ) );
			fp = fp * fp * fp * ( fp * ( fp * 6 - 15 ) + 10 );
			return lerp( lerp( d00, d01, fp.y ), lerp( d10, d11, fp.y ), fp.x ) + 0.5;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		void surf( Input i , inout SurfaceOutputStandard o )
		{
			o.Normal = float3(0,0,1);
			float3 ase_objectScale = float3( length( unity_ObjectToWorld[ 0 ].xyz ), length( unity_ObjectToWorld[ 1 ].xyz ), length( unity_ObjectToWorld[ 2 ].xyz ) );
			float2 temp_cast_1 = (( _Time.y * _Speed )).xx;
			float2 uv_TexCoord14 = i.uv_texcoord * ( ase_objectScale * _Scale ).xy + temp_cast_1;
			float2 center45_g13 = float2( 0.5,0.5 );
			float2 delta6_g13 = ( i.uv_texcoord - center45_g13 );
			float angle10_g13 = ( length( delta6_g13 ) * 4.0 );
			float x23_g13 = ( ( cos( angle10_g13 ) * delta6_g13.x ) - ( sin( angle10_g13 ) * delta6_g13.y ) );
			float2 break40_g13 = center45_g13;
			float2 temp_cast_2 = (( _Time.y * _TwirlSpeed )).xx;
			float2 break41_g13 = temp_cast_2;
			float y35_g13 = ( ( sin( angle10_g13 ) * delta6_g13.x ) + ( cos( angle10_g13 ) * delta6_g13.y ) );
			float2 appendResult44_g13 = (float2(( x23_g13 + break40_g13.x + break41_g13.x ) , ( break40_g13.y + break41_g13.y + y35_g13 )));
			float2 temp_output_188_0 = appendResult44_g13;
			float gradientNoise189 = UnityGradientNoise(temp_output_188_0,_DistortionScale);
			gradientNoise189 = gradientNoise189*0.5 + 0.5;
			o.Albedo = ( _Tint * ( tex2D( _MainTexture, ( uv_TexCoord14 * ( gradientNoise189 * 0.01 ) ) ) * _Opacity ) ).rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			float3 temp_output_16_0_g14 = ( ase_worldPos * 100.0 );
			float3 crossY18_g14 = cross( ase_worldNormal , ddy( temp_output_16_0_g14 ) );
			float3 worldDerivativeX2_g14 = ddx( temp_output_16_0_g14 );
			float dotResult6_g14 = dot( crossY18_g14 , worldDerivativeX2_g14 );
			float crossYDotWorldDerivX34_g14 = abs( dotResult6_g14 );
			float gradientNoise69 = UnityGradientNoise(temp_output_188_0,_DistortionScale);
			gradientNoise69 = gradientNoise69*0.5 + 0.5;
			float temp_output_20_0_g14 = gradientNoise69;
			float3 crossX19_g14 = cross( ase_worldNormal , worldDerivativeX2_g14 );
			float3 break29_g14 = ( sign( crossYDotWorldDerivX34_g14 ) * ( ( ddx( temp_output_20_0_g14 ) * crossY18_g14 ) + ( ddy( temp_output_20_0_g14 ) * crossX19_g14 ) ) );
			float3 appendResult30_g14 = (float3(break29_g14.x , -break29_g14.y , break29_g14.z));
			float3 normalizeResult39_g14 = normalize( ( ( crossYDotWorldDerivX34_g14 * ase_worldNormal ) - appendResult30_g14 ) );
			float3 ase_worldTangent = WorldNormalVector( i, float3( 1, 0, 0 ) );
			float3 ase_worldBitangent = WorldNormalVector( i, float3( 0, 1, 0 ) );
			float3x3 ase_worldToTangent = float3x3( ase_worldTangent, ase_worldBitangent, ase_worldNormal );
			float3 worldToTangentDir42_g14 = mul( ase_worldToTangent, normalizeResult39_g14);
			float4 screenColor1 = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_GrabTexture,( ase_grabScreenPosNorm + ( ase_grabScreenPosNorm * float4( worldToTangentDir42_g14 , 0.0 ) ) ).xy);
			o.Emission = screenColor1.rgb;
			o.Alpha = 1;
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			clip( tex2D( _MainTex, uv_MainTex ).a - _Cutoff );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 screenPos : TEXCOORD2;
				float4 tSpace0 : TEXCOORD3;
				float4 tSpace1 : TEXCOORD4;
				float4 tSpace2 : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
