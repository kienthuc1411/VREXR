// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Sprites/lightweightGradientLerpSprite"
{
    Properties
    {
		_Color1("Color1", Color) = (1, 1, 1)
		_Color2("Color2", Color) = (1, 1, 1)
		_Val("Gradient Value", Float) = 0.5
    }
 
    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }
 
        Cull Off
        Lighting Off
        ZWrite Off
        Fog { Mode Off }
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile DUMMY PIXELSNAP_ON
            #include "UnityCG.cginc"
           
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };
 
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };
           
			float4 _Color1;
			float4 _Color2;

			float _Val;
 
            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
				OUT.color = lerp(_Color1, _Color2, _Val); 
                return OUT;
            }
 
            sampler2D _MainTex;
            uniform float _EffectAmount;
 
            fixed4 frag(v2f IN) : COLOR
            {
				fixed4 col = lerp(_Color1, _Color2, _Val);
				return col;
            }
        ENDCG
        }
    }
    Fallback "Sprites/Default"
}