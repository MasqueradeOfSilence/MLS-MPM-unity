Shader "Custom/TestShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _TexRed ("Albedo (RGB)", 2D) = "red" {}
        _TexGreen ("Albedo (RGB)", 2D) = "green" {}
        _TexBlue ("Albedo (RGB)", 2D) = "blue" {}
        _TexYellow ("Albedo (RGB)", 2D) = "yellow" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _TexRed;
        sampler2D _TexGreen;
        sampler2D _TexBlue;
        sampler2D _TexYellow;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Testing
            int length = 4;
            // Hardcoded test spheres
            half3 points[4] = {half3(40.0999985,8.88998699,0), half3(39.4020004,8.88998699,0),
                half3(40.0289993,8.88998699,-0.758000016), half3(39.4640007,8.47900009,-0.60799998)};
            // scaling: 0.9, 1.1, 1, 0.7
            half radiusOfCollider = 0.5;
            half radius1 = 0.9 * radiusOfCollider; // 0.45
            half radius2 = 1.1 * radiusOfCollider; // 0.55
            half radius3 = 1 * radiusOfCollider; // 0.5
            half radius4 = 0.7 * radiusOfCollider; // 0.35

            // radii as weights
            half radii[4] = {radius1, radius2, radius3, radius4};
            half4 colors[4] = {half4(1.0f, 0.0f, 0.0f, 1.0f), half4(0.0f, 1.0f, 0.0f, 1.0f), half4(0.0f, 0.0f, 1.0f, 1.0f), half4(1.0f, 1.0f, 0.0f, 1.0f)};
            half minDist = 10000;
            int minI = 0;

            for (int i = 0; i < length; i++)
            {
                half dist = distance(IN.worldPos, points[i].xyz) - radii[i];
                if (dist < minDist)
                {
                    minDist = dist;
                    minI = i;
                }
            }

            fixed4 c2;
            bool invisibleTest = false;
            if (minI == 0)
            {
                if (invisibleTest)
                {
                    // Discard will make it invisible
                    discard;
                }
                c2 = tex2D (_TexRed, IN.uv_MainTex) * colors[minI];
            }
            else if (minI == 1)
            {
                c2 = tex2D (_TexGreen, IN.uv_MainTex) * colors[minI];
            }
            else if (minI == 2)
            {
                c2 = tex2D (_TexBlue, IN.uv_MainTex) * colors[minI];
            }
            else if (minI == 3)
            {
                c2 = tex2D (_TexYellow, IN.uv_MainTex) * colors[minI];
            }
            else
            {
                c2 = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            }
            o.Albedo = c2.rgb;
            o.Alpha = 0.9;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
