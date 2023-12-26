Shader "Custom/TestShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 1.0
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _SphereCenter("SphereCenter", Vector) = (0, 0, 0, 0)
        _TestColorGreen("Green", Color) = (0, 1, 0, 1)
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
        // {39.4640007,9.28899956,-0.60799998};
        // 0.37;
        // I think the issue is not with data passing but rather formatting
        // or, where these are located?

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float bonusSphereRadius;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        fixed4 _SphereCenter;
        fixed4 _TestColorGreen;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            half4 testMe2 = half4(39.4640007,9.28899956,-0.60799998, 1.0);
            half4 testMe3 = _SphereCenter;
            
            // Testing
            int length = 5;
            // Hardcoded test spheres
            half3 points[5] = {half3(40.0999985,8.88998699,0), half3(39.4020004,8.88998699,0),
                half3(40.0289993,8.88998699,-0.758000016), half3(39.4640007,8.47900009,-0.60799998),
                half3(_SphereCenter.xyz)};
            // Above only works if hardcoded. 
            // scaling: 0.9, 1.1, 1, 0.7
            half radiusOfCollider = 0.5;
            half radius1 = 0.9 * radiusOfCollider; // 0.45
            half radius2 = 1.1 * radiusOfCollider; // 0.55
            half radius3 = 1 * radiusOfCollider; // 0.5
            half radius4 = 0.7 * radiusOfCollider; // 0.35

            // radii as weights
            half radii[5] = {radius1, radius2, radius3, radius4, 0.371665}; // even w/full hardcode something is off, possibly due to too-close radii...
            half minDist = 10000;
            int minI = -1;

            for (int i = 0; i < length; i++)
            {
                half dist = distance(IN.worldPos, points[i].xyz) - radii[i];
                if (dist < minDist)
                {
                    minDist = dist;
                    minI = i;
                }
            }

            // Determine which sphere we're on

            bool onSphere0 = distance(points[0].xyz, IN.worldPos) <= radii[0];
            bool onSphere1 = distance(points[1].xyz, IN.worldPos) <= radii[1];
            bool onSphere2 = distance(points[2].xyz, IN.worldPos) <= radii[2];
            bool onSphere3 = distance(points[3].xyz, IN.worldPos) <= radii[3];
            // only problem: specifying by radii might be an issue because some bubbles might have same radius
            bool onSphere4 = distance(points[4].xyz, IN.worldPos) <= radii[4];

            fixed4 c2 = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c2.rgb;
            o.Alpha = 0;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;

            if (onSphere0 && minI != 0)
            {
                discard;
            }
            if (onSphere1 && minI != 1)
            {
                discard;
            }
            if (onSphere2 && minI != 2)
            {
                discard;
            }
            if (onSphere3 && minI != 3)
            {
                discard;
            }
            if (onSphere4 && minI != 4)
            {
                // now it is somewhat discarding them but not fully
                // I am not sure if this gets entered
                //discard;

                // it DOES get entered, but the computation is incorrect
                // actually, it may be correct, but not with how others interact with it

                fixed4 c3 = tex2D (_MainTex, IN.uv_MainTex) * _TestColorGreen;
                o.Albedo = c3.rgb;
                o.Alpha = 0.5;
            }

            
        }
        ENDCG
    }
    FallBack "Diffuse"
}
