Shader "Unlit/RaymarchCam"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            sampler2D _MainTex;
            uniform sampler2D _CameraDepthTexture;
            uniform float4x4 _CamFrustrum, _CamToWorld;

            uniform float3 _lightDir;
            uniform float _lightIntensity;
            uniform float _shadowIntensity;
            uniform float _aoIntensity;

            uniform fixed4 _mainColor;
            uniform fixed4 _secColor;
            uniform fixed4 _skyColor;


            uniform int _iterations;
            uniform float _precision;
            uniform float _scaleFactor;
            uniform float _smoothRadius;
            uniform float3 _modOffsetPos;
            uniform float _maxDistance;
            uniform float _GlobalScale;
            uniform float4x4 _globalTransform;
            uniform float4x4 _iterationTransform;

            uniform float3 _player;
            uniform float _forceFieldRad;


            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ray : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                half index = v.vertex.z;
                v.vertex.z = 0;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;

                o.ray = _CamFrustrum[(int)index].xyz;

                o.ray /= abs(o.ray.z);

                o.ray = mul(_CamToWorld, o.ray);

                return o;
            }

            float pMod ( float p, float size)
            {
                float halfsize = size * 0.5;
                float c = floor((p+halfsize)/size);
                p = fmod(p+halfsize,size)-halfsize;
                p = fmod(p-halfsize,size)+halfsize;
                return p;
            }

            // Sphere
            // s: radius
            float sdSphere(float3 p, float s)
            {
	            return length(p) - s;
            }

            float sdforceField(float3 p)
            {
                return sdSphere(p - _player , _forceFieldRad);
            }

            // Box
            // b: size of box in x/y/z
            float sdBox(float3 p, float3 b)
            {
	            float3 d = abs(p) - b;
	            return min(max(d.x, max(d.y, d.z)), 0.0) +
		            length(max(d, 0.0));
            }

            // InfBox
            // b: size of box in x/y/z
            float sd2DBox( in float2 p, in float2 b )
            {
                float2 d = abs(p)-b;
                return length(max(d,float2(0,0))) + min(max(d.x,d.y),0.0);
            }


            // Cross
            // s: size of cross
            float sdCross( in float3 p, float b )
            {
              float da = sd2DBox(p.xy,float2(b,b));
              float db = sd2DBox(p.yz,float2(b,b));
              float dc = sd2DBox(p.zx,float2(b,b));
              return min(da,min(db,dc));
            }



            //mergerSponge
            float2 sdMerger( in float3 p, float b, int _iterations, float3 _modOffsetPos , float4x4 _iterationTransform, float4x4 _globalTransform, float _smoothRadius, float _scaleFactor)
            {
   
               p = mul(_globalTransform, float4(p,1)).xyz;
   
   
               float2 d = float2(sdBox(p,float3(b- _smoothRadius,b- _smoothRadius,b- _smoothRadius)),0)- _smoothRadius;

               float s = 1.0;
               for( int m=0; m<_iterations; m++ )
               {
                    p = mul(_iterationTransform, float4(p,1)).xyz;
                    p.x = pMod(p.x,b*_modOffsetPos.x * 2/s);
                    p.y = pMod(p.y,b*_modOffsetPos.y * 2/s);
                    p.z = pMod(p.z,b*_modOffsetPos.z * 2/s);
      
                    s *= _scaleFactor * 3;
                    float3 r =(p)*s; 
                    float c = (sdCross(r,b- _smoothRadius/s)- _smoothRadius)/s;
        
                    if(-c>d.x)
                    {
                        d.x = -c;
                        d = float2( d.x, m);
            
                    }
               }  
               return d;
            }

            float2 distanceField(float3 p)
            {
                float2 dist = sdMerger(p,_GlobalScale, _iterations,_modOffsetPos ,_iterationTransform, _globalTransform, _smoothRadius, _scaleFactor);
                return dist;
            }


            // returns the normal in a single point of the fractal
            float3 getNormal(float3 p)
            {

              float d = distanceField(p).x;
                const float2 e = float2(.01, 0);
              float3 n = d - float3(distanceField(p - e.xyy).x,distanceField(p - e.yxy).x,distanceField(p - e.yyx).x);
              return normalize(n);

            }


            // calcutates soft shadows in a point
            float shadowCalc( in float3 ro, in float3 rd, float mint, float maxt, float k )
            {
                float res = 1.0;
                float ph = 1e20;
                for( float t=mint; t<maxt; )
                {
                    float h = min(distanceField(ro + rd*t),sdforceField(ro + rd*t));
                    if( h<0.001 )
                        return 0.0;
                    float y = h*h/(2.0*ph);
                    float d = sqrt(h*h-y*y);
                    res = min( res, k*d/max(0.0,t-y) );
                    ph = h;
                    t += h;
                }
                return res;
            }


            fixed4 raymarching(float3 ro, float3 rd, float depth)
            {   

                fixed4 result = fixed4(0,0,0,0.5); // default
                int max_iteration = 400; // max amount of steps
                float t = 0; //distance traveled
                bool _forceFieldHit = false;



                for (int i = 0; i < max_iteration; i++)
                {
                    //sends out ray from the camera
                    float3 p = ro + rd * t;

                    //return distance to forcefield
                    float _forceFieldDist = sdforceField(p);

                    if (abs( _forceFieldDist) < _precision && _forceFieldHit == false) //hit forcefield
                    {
                        _forceFieldHit = true;
                    }
                              
                    if(t > _maxDistance || t >= depth)
                    {
                        result = fixed4(rd,0); 
                        break;
                    }

                    //return distance to fractal
                    float2 d = distanceField(p);
                    
                    if ((d.x) < _precision) //hit
                    {
                        float3 colorDepth;
                        float light;
                        float shadow;

                        float3 color = float3(_mainColor.rgb*(_iterations-d.y)/_iterations + _secColor.rgb*(d.y)/_iterations);

                        float3 n = getNormal(p);
                        light = 1.0;
                        shadow = (shadowCalc(p, -_lightDir, 0.1, _maxDistance, 3) * (1 - _shadowIntensity) + _shadowIntensity);

                        float ao = (1 - 2 * i/float(max_iteration)) * (1 - _aoIntensity) + _aoIntensity; // ambient occlusion
                        float3 colorLight = float3 (color * light * shadow * ao); /* multiplying all values between 0 and 1 to return final color */
                        colorDepth = float3 (colorLight*(_maxDistance-t)/(_maxDistance) + _skyColor.rgb*(t)/(_maxDistance)); // multiplying with distance
                       
                        result = fixed4(colorDepth ,1);
                        break;

                    }

                    if(_forceFieldHit == false)
                    {  
                        // closer points get higher precicion to limit overstepping
                        if((d.x) < 10)
                        {
                            t+=  min(d.x * 0.75f, _forceFieldDist);
                        }
                        else if( abs(d.x) < 2)
                        {
                            t+= min(d.x * 0.5f, _forceFieldDist);
                        }
                        else t+= min(d.x, _forceFieldDist);
                        
                        
                    }
                    else t += d.x;
                }

                return result;
            }


            fixed4 frag (v2f i) : SV_Target
            {
               float depth = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).r);
               depth *= length(i.ray);
               fixed3 col = tex2D(_MainTex, i.uv);
               
               float3 rayDirection = normalize(i.ray.xyz);
               float3 rayOrigin = _WorldSpaceCameraPos;
               fixed4 result = raymarching(rayOrigin, rayDirection, depth);
               return fixed4(col * (1.0 - result.w) + result.xyz * result.w ,1.0);
            }
            ENDCG
        }
    }
}
