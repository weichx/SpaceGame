Shader "Unlit/Checkerboard"
{
    Properties
    {
        _Color0("Color0", Color) = (0, 0, 0, 0)
        _Color1("Color0", Color) = (1, 1, 1, 1)
        _Transparency("Transparency", Range(0.0,0.5)) = 0.25
        _Density ("Density", Range(2,50)) = 30
    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        //ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float _Density;
            float _Transparency;
            float4 _Color0;
            float4 _Color1;
            
            v2f vert (float4 pos : POSITION, float2 uv : TEXCOORD0)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(pos);
                o.uv = uv * _Density;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 c = i.uv;
                c = floor(c) / 2;
                float checker = frac(c.x + c.y) * 2;
                fixed4 color = _Color0;
                if(checker == 1) {
                    color = _Color1;
                    color.a = _Transparency;
                }
                return color;
            }
            ENDCG
        }
    }
}