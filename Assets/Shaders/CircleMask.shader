// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/**
 * A really crappy shader to try and make a circular mask with an outline
 * for the game window.
 * TODO: The outline should ideally be 1px thick all around, but I don't
 *   know how to do that.
 */
Shader "Custom/CircleMask" {
    
    Properties {
        _Color("Color", Color) = (1,1,1,1)
        _StrokeThickness("Stroke Thickness", Float) = 1
        _StrokeColor("Stroke Color", Color) = (1,1,1,1)
        _Radius("Radius", Float) = 0.5
    }

    SubShader {
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _Color;
            Float _StrokeThickness;
            fixed4 _StrokeColor;
            Float _Radius;

            struct fragmentInput {
                float4 pos : SV_POSITION;
                float2 uv : TEXTCOORD0;
            };

            fragmentInput vert(appdata_base v) {
                fragmentInput o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord.xy - fixed2(0.5,0.5);
                return o;
            }

            fixed4 frag(fragmentInput i) : SV_Target {
                float distance = sqrt(pow(i.uv.x, 2) + pow(i.uv.y,2)) * 2;
                if (distance < _Radius) {
                    discard;
                }
                if (distance < (_Radius + _StrokeThickness)) {
                    return _StrokeColor;
                }
                return _Color;
            }
            ENDCG
        }   
    }
}
