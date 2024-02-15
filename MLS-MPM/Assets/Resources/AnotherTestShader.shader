Shader "Custom/AnotherTestShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _DebugGreen("_DebugGreen", Color) = (0, 1, 0, 1)
        _DebugPink("_DebugPink", Color) = (1, 0, 1, 1)
        _DebugBlue("_DebugBlue", Color) = (0, 0, 1, 1)
        _CountMe("_CountMe", Integer) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _DebugGreen;
        fixed4 _DebugPink;
        fixed4 _DebugBlue;
        float _TestNumbers[2];
        int _CountMe;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // IF the _TestNumbers is set properly, change it to green
            // placeholder
            if (_CountMe == 2)
            {
                if (_TestNumbers[0] == 7 && _TestNumbers[1] == 1924)
                {
                    // We want to see this if using FFFOptimized case
                    o.Albedo = _DebugGreen;
                    o.Alpha = 1;
                }
                else if (_TestNumbers[0] == 7 && _TestNumbers[1] == 666)
                {
                    // We want to see this if using VoronoiShaderDTO case
                    // Observation: It does turn blue, but then turns pink when I switch tabs. So the data is not preserved.
                    // I am not sure if this matters if it only happens when I am paused. 
                    // Possibly: Instead of discarding, try coloring, and see what happens.
                    // Does this also happen if NOT done here inside the DTO?
                    // what about in the 2d version?
                    o.Albedo = _DebugBlue;
                    o.Alpha = 1;
                }
                else if (_TestNumbers[0] == 0 && _TestNumbers[1] == 0)
                {
                    o.Albedo = _DebugPink;
                    o.Alpha = 1;
                }
            }
        }

        ENDCG
    }
    FallBack "Diffuse"
}
