Shader "Custom/FFFShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        //_SphereCenters ("_SphereCenters", Vector) = (1, 1, 1, 1)
        //_SphereRadii ("_SphereRadii", Float) = 1.0
        _Count("_Count", Integer) = 0
        _DebugCyan("_DebugCyan", Color) = (0, 1, 1, 1)
        _DebugWhite("_DebugWhite", Color) = (1, 1, 1, 1)
        _DebugPink("_DebugPink", Color) = (1, 0, 1, 1)
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
        fixed4 _DebugWhite;
        fixed4 _DebugPink;
        //float4 _TheData[900];
        uniform float arrayName[2];

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
            //_SphereCenters[0].x = 5;
            for (int i = 0; i < _Count; i++)
            {
                half dist = distance(IN.worldPos, _SphereCenters[i].xyz) - (_SphereRadii[i] * radiusOfCollider);
        
                // Always white for some reason, even though we set it in the DTO
                //if (_SphereCenters[i].x == 0 && _SphereCenters[i].y == 0 && _SphereCenters[i].z == 0)
        
                // Always white
                //if (_SphereRadii[i] == 0)
        
                // Never white, so set correctly.
                //if (_Count == 0)
                if (dist < minDist)
                {
                    minDist = dist;
                    minI = i;
                }
            }
    
            for (int j = 0; j < _Count; j++)
            {
                bool onCurrentSphere = distance(_SphereCenters[j].xyz, IN.worldPos) <= (_SphereRadii[j] * radiusOfCollider);
                // (11.32, 4.24, 3.17, 0.00)
                if (_SphereCenters[j].x == 0 && _SphereCenters[j].y == 0 && _SphereCenters[j].z == 0)
                {
                    // TEST uncommenting -- if they are zeroes it will all be CYAN
                    o.Albedo = _DebugCyan;
                    o.Alpha = 1;
                }
        
                // always cyan in 3D (incorrect)
                //if (distance(_SphereCenters[j].xyz, IN.worldPos) > 5)
                //{
                    //o.Albedo = _DebugCyan;
                    //o.Alpha = 1;
                //}
                //else
                //{
                    //o.Albedo = _DebugWhite;
                    //o.Alpha = 1;
                //}
                if (onCurrentSphere && minI != j)
                {
                    // for ease of debug
                    o.Albedo = _DebugPink;
                    o.Alpha = 1;
                    //discard;
                }
            }
            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
