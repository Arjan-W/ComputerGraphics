Shader "Unlit/ToonEye" {

	Properties {
        _Depth   ("Depth"   , Range(0, 1  )) = 0.5
        _Gradient("Gradient", Range(0, 100)) = 20
        
        [Header(Iris)]
        _IrisBoundaryColor ("Iris Boundary Color" , Color      ) = (0, 0, 1, 1)
        _IrisBoundaryRadius("Iris Boundary Radius", Range(0, 1)) = 0.4
        _IrisInteriorColor ("Iris Interior Color" , Color      ) = (0, 1, 0, 1)
        _IrisInteriorRadius("Iris Interior Radius", Range(0, 1)) = 0.38		
        
        [Header(Pupil)]
        _PupilBoundaryColor ("Pupil Boundary Color" , Color      ) = (0.13333, 0.13725, 0.13725, 1)
        _PupilBoundaryRadius("Pupil Boundary Radius", Range(0, 1)) = 0.2
        _PupilInteriorColor ("Pupil Interior Color" , Color      ) = (0.53725, 0.55294, 0.54118, 1)
        _PupilInteriorRadius("Pupil Interior Radius", Range(0, 1)) = 0.19
        
        [Header(Sclera)]
        _ScleraColor("Sclera Color", Color) = (0.94118, 0.96471, 0.94118, 1)
    }
	
    SubShader{	
        Pass {
            Tags {"LightMode" = "ForwardBase"}
        
            CGPROGRAM
			
            #pragma fragment frag
            #pragma vertex vert
            
            #include "UnityCG.cginc"
            #include "UnityLightingCommon.cginc"

            struct v2f {
                fixed4 illumination   : COLOR0;
                float4 clipPosition   : SV_POSITION;
                float3 objectPosition : TEXCOORD0;
            };
            
            v2f vert (appdata_base v) {
                v2f o;
				half3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed4 diffuse = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz)) * _LightColor0;
				fixed3 ambient = ShadeSH9(half4(worldNormal, 1));
				o.illumination   = half4(ambient, 0) + diffuse;
				o.clipPosition   = UnityObjectToClipPos(v.vertex);
				o.objectPosition = v.vertex;
                return o;
            }
			
			float _Depth, _Gradient,  _IrisBoundaryRadius, _IrisInteriorRadius, _PupilBoundaryRadius, _PupilInteriorRadius;
			fixed4 _IrisBoundaryColor, _IrisInteriorColor, _PupilBoundaryColor, _PupilInteriorColor, _ScleraColor;
			
            fixed4 frag (v2f i) : SV_Target {
				float mask = distance(0, float3(i.objectPosition.xy, i.objectPosition.z - _Depth));
				float IrisBoundaryMask  = saturate(_Gradient * (1 - saturate(mask / _IrisBoundaryRadius )));
				float IrisInteriorMask  = saturate(_Gradient * (1 - saturate(mask / _IrisInteriorRadius )));
				float pupilBoundaryMask = saturate(_Gradient * (1 - saturate(mask / _PupilBoundaryRadius )));
				float pupilInteriorMask = saturate(_Gradient * (1 - saturate(mask / _PupilInteriorRadius )));
				
				fixed4 irisInterior  = (IrisInteriorMask  - pupilBoundaryMask) * lerp(_IrisInteriorColor , _IrisBoundaryColor , mask / _IrisInteriorRadius );
                fixed4 irisBoundary  = (IrisBoundaryMask  - IrisInteriorMask ) * _IrisBoundaryColor;
				fixed4 pupilInterior  = (pupilInteriorMask) * _PupilInteriorColor;
                fixed4 pupilBoundary  = (pupilBoundaryMask - pupilInteriorMask) * _PupilBoundaryColor;
				
				fixed4 baseEye = (1-IrisBoundaryMask) * _ScleraColor;
				
				fixed4 color = i.illumination * (baseEye + irisInterior + irisBoundary + pupilBoundary + pupilInterior);
				return color;
            }
            ENDCG
        }
    }
	
}