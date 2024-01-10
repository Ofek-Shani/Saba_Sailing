Shader "Custom/Waives"
{
    Properties
    {
        _Direction ("Direction", Range(0,2)) = 0.5 // angle in PI
        //_Number ("Number", Range(1,100)) = 1
        //_Frequency ("Frequency", Range(1, 20)) = 1
        //_Amplitude ("Amplitude", Range(0, 1)) = 0.5
        //_NoiseScale ("Noise Scale", Range(0, 1)) = 0.1
        //_NoiseSpeed ("Noise Speed", Range(0, 10)) = 1
        _Speed ("Speed", Range(0, 10)) = 1
        _Color ("Color", Color) = (0,0,1,1)
        _MainTex ("MainTex", 2D) = "white" {}
        //_Glossiness ("Smoothness", Range(0,1)) = 0.5
        // _Specullar ("Specullar", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
// Upgrade NOTE: excluded shader from DX11, OpenGL ES 2.0 because it uses unsized arrays
#pragma exclude_renderers d3d11 gles
        // Physically based Standard lighting model, and enable shadows on all light types
        // #pragma surface surf Standard fullforwardshadows
        #pragma surface surf Lambert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        //half _Glossiness;
        half _Specullar;
        fixed4 _Color;
        float4 _Direction;
        //float _Offset;
        //float _Frequency;
        //float _Amplitude;
        float _Speed;
        int _Number;
        float _NoiseScale;
        //float _NoiseSpeed; 

        struct Input
        {
            float2 uv_MainTex;
        };

        float Noise(float2 p)
        {
            return frac(sin(dot(p, float2(12.9898, 78.233))) * 43758.5453);
        }

        float Rand() { return frac(sin(_Time.x) * 43758.5453);}
        
        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)
        fixed4 myWhite = fixed4(1,1,1,1);
        int n = 0;
        float3 dots[1];
        void initDots() {
            if (n == _Number) return;
            dots[0] = float3(0.5,0.5,0.2);
            for (int i = 0; i < n; i++) {
                dots[i][0] = Rand();
                dots[i][1] = Rand();
                dots[i][2] = 0.1 + 0.2 * Rand();
            }
        }
        float manDist(float3 loci, float2 uv) {
            return max(abs(loci.x-uv.x), abs(loci.y-uv.y));
        }
        bool closeTo(float3 loci, float2 uv) {
            return manDist(loci, uv) <= loci[2];
        }
        bool inDot(float2 uv) {
           for (int i= 0; i < n; i++) {
              if (closeTo(dots[i], uv)) return true;
           }
           return false;
        }
        void surf (Input IN, inout SurfaceOutput o)
        {
            //initDots();
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            // Apply spatial noise to the UV coordinates
            // float2 noiseOffset = _NoiseScale * PerlinNoise(IN.uv_MainTex * _NoiseSpeed + _Time.y);

            // Calculate the position along the direction
            //float position = ( dot(IN.uv_MainTex, _Direction.xy) + _Speed * _Time.w) * _Frequency; 
            // Calculate the offset based on the position
            //float offset = sin(position) * _Amplitude ;// - Noise(IN.uv_MainTex);
            
            // Apply the offset to the UV coordinates
            //float2 uvOffset = IN.uv_MainTex + offset * _Direction.xy; // + noiseOffset;
            
            // Sample the color from the offset UV coordinates
            //fixed4 stripeColor = tex2D(_MainTex, uvOffset) * _Color;
            // c = ;
            o.Albedo = c.rgb;// * abs(sin(IN.uv_MainTex.x * 10)); //fixed4(1,0.5,0.5,1); //(_Color.rgb * abs(_Time.w)));
            //((inDot(IN.uv_MainTex)) ? myWhite : _Color * 4).rgb; 
            //o.Albedo = c; //(c * offset).rgb  * 4 ; //stripeColor.rgb; // (uvOffset > 0.5 ? fixed4(1.0,1.0,1.0,1.0) : c).rgb * 4;
            // Metallic and smoothness come from slider variables
            // o.Metallic = _Metallic;
            // o.Smoothness = _Glossiness;
            
            o.Alpha = _Color.a; //1;//_Time.w;
            //o.Normal =  UnpackNormal (tex2D (_MainTex, IN.uv_MainTex));
            // o.Specullar = _Specullar;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
