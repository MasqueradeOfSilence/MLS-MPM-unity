Shader "Custom/WhiteBubbleShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Count("_Count", Integer) = 0
        _DebugCyan("_DebugCyan", Color) = (0, 1, 1, 1)
        _DebugWhite("_DebugWhite", Color) = (1, 1, 1, 1)
        _DebugPink("_DebugPink", Color) = (1, 0, 1, 1)
        _White("_White", Color) = (1, 1, 1, 1)

        _FilmThickness ("Film Thickness", Range(0.0, 1.0)) = 0.1
        _FilmIor ("Film IOR", Range(1.0, 2.0)) = 1.33
        _FilmColor ("Film Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM

        #pragma surface surf Standard fullforwardshadows alpha
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 worldNormal;
            float3 viewDir;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float4 _SphereCenters[900];
        float _SphereRadii[900];
        int _Count;
        fixed4 _DebugCyan;
        fixed4 _DebugWhite;
        fixed4 _DebugPink;
        fixed4 _White;
        float _FilmThickness;
        float _FilmIor;
        fixed4 _FilmColor;

        UNITY_INSTANCING_BUFFER_START(Props)
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c2 = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            float3 viewDir = normalize(IN.viewDir);
            float3 normal = normalize(o.Normal);

            // Calculate interference color
            float thickness = _FilmThickness * 1000.0; // Scale thickness to be more visible
            float ior = _FilmIor;
            float wavelength[3] = { 380.0, 500.0, 700.0 }; // Red, Green, Blue wavelengths in nm

            float3 interferenceColor = float3(0.0, 0.0, 0.0);
            for (int i = 0; i < 3; i++)
            {
                float phaseShift = 2.0 * 3.14159 * 2.0 * thickness * ior / wavelength[i];
                float cosTheta = dot(viewDir, normal);
                float intensity = 0.5 * (1.0 + cos(phaseShift * cosTheta));
                interferenceColor[i] = intensity;
            }
            interferenceColor = saturate(interferenceColor);

            // Blend the interference color with the base white color
            float blendFactor = 0.5; // Adjust this to make the interference more or less visible
            float3 finalColor = lerp(c2.rgb, c2.rgb * _FilmColor.rgb * interferenceColor, blendFactor);
            
            o.Albedo = finalColor;
            o.Alpha = 0.1;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            
            float radiusOfCollider = 0.5; // Default radius for Unity colliders
            half minDist = 10000;
            int minI = -1;
            for (int i = 0; i < _Count; i++)
            {
                half dist = distance(IN.worldPos, _SphereCenters[i].xyz) - (_SphereRadii[i] * radiusOfCollider);
                if (dist < minDist)
                {
                    minDist = dist;
                    minI = i;
                }
            }
    
            for (int j = 0; j < _Count; j++)
            {
                bool onCurrentSphere = distance(_SphereCenters[j].xyz, IN.worldPos) <= (_SphereRadii[j] * radiusOfCollider);
                if (onCurrentSphere && minI != j)
                {
                    discard;
                }
            }
        }
        ENDCG
    }
    FallBack "Diffuse"
}
