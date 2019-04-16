Shader "Unlit/LinesShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor("LineColor",Color) = (0,0,0,1)
        _conditionThreshold("Threshold Condition",float) = 1.71 //As play test this value came up
        _LineStrength("Line Strength ",float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _LineColor;
            float _conditionThreshold;
            float _LineStrength;
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
             
             fixed4 col = tex2D(_MainTex, i.uv);
              
             float multy = length(col);//Considering black and white textures only
            
             return _LineColor * step(multy * _LineStrength, _conditionThreshold);
             
            }
            ENDCG
        }
      
    }
}
