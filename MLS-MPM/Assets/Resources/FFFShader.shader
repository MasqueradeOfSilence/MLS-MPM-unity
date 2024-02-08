Shader "Custom/FFFShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        //_SphereCenters ("_SphereCenters", Vector) = (1, 0, 0, 1)
        //_SphereRadii ("_SphereRadii", Float) = 1.0
        _Count("_Count", Integer) = 0
        _DebugCyan("_DebugCyan", Color) = (0, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM

        // Uncomment me if you are on MAC OS
        // Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
        //#pragma exclude_renderers d3d11 gles
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        // it can't seem to handle length 4096
        float4 _SphereCenters[900];
        float _SphereRadii[900];
        int _Count;
        fixed4 _DebugCyan;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 c2 = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c2.rgb;
            o.Alpha = 0;
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
