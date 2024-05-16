Shader "Custom/DimTopToBottom" {
    Properties{
        _MainTex("Base (RGB)", 2D) = "white" { }
        _DimAmount("Dim Amount", Range(0, 1)) = 0.5
    }

        SubShader{
            Tags { "Queue" = "Overlay" }
            Pass {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                // include UnityCG for accessing Unity's shader functions
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float3 normal : NORMAL;
                };

                struct v2f {
                    float4 pos : POSITION;
                };

                // vertex shader
                v2f vert(appdata v) {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    return o;
                }

                // fragment shader
                fixed4 _MainTex_ST;
                fixed _DimAmount;

                fixed4 frag(v2f i) : COLOR {
                    // Calculate the gradient from top to bottom
                    fixed gradient = i.pos.y / _MainTex_ST.y;

                // Clamp the gradient to the range [0, 1]
                gradient = saturate(gradient);

                // Calculate the final color with dimming effect
                fixed4 col = tex2D(_MainTex, i.pos.xy / i.pos.w);
                col.rgb *= 1.0 - gradient * _DimAmount;

                return col;
            }
            ENDCG
        }
        }
}