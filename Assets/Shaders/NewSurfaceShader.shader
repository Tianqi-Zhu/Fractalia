Shader "Unlit/Raymarch3"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
    }
        SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag


            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitpos : TEXCOORD2;
                float4 light : TEXCOORD3;

            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = mul(unity_WorldToObject, float4(_WorldSpaceCameraPos, 1));
                o.hitpos = v.vertex;
                o.light = _WorldSpaceLightPos0;
                return o;
            }


            // The MIT License
            // Copyright © 2013 Inigo Quilez
            // Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions: The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software. THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

            // http://www.iquilezles.org/www/articles/menger/menger.htm


            #define mod(x,y) (x-y*floor(x/y))

            float maxcomp(in float3 p) { return max(p.x,max(p.y,p.z)); }

            float sdBox(float3 p, float3 b)
            {
              float3  di = abs(p) - b;
              float mc = maxcomp(di);
              return min(mc,length(max(di,0.0)));
            }

            float4 GetDist(in float3 p)
            {
                float d = sdBox(p,1.0);
                float4 res = float4(d, 1.0, 0.0, 0.0);

                float s = 1.0;
                for (int m = 0; m < 4; m++)
                {
                    float3 a = mod(p * s, 2.0) - 1.0;
                    s *= 3.0;
                    float3 r = abs(1.0 - 3.0 * abs(a));
                    float da = max(r.x,r.y);
                    float db = max(r.y,r.z);
                    float dc = max(r.z,r.x);
                    float c = (min(da,min(db,dc)) - 1.0) / s;

                    if (c > d)
                    {
                      d = c;
                      res = float4(d, min(res.y,0.2 * da * db * dc), (1.0 + float(m)) / 4.0, 0.0);
                    }
                }

                return res;
            }

            float4 Raymarch(in float3 ro, in float3 rd)
            {
                float t = 0.0;
                float4 res = -1.0;
                float4 h = 1.0;
                for (int i = 0; i < 64; i++)
                {
                    if (h.x < 0.002 || t>50.0) break;
                    h = GetDist(ro + rd * t);
                    res = float4(t,h.yzw);
                    t += h.x;
                }
                if (t > 50.0) res = -1.0;
                return res;
            }

            float softshadow(in float3 ro, in float3 rd, float mint, float k)
            {
                float res = 1.0;
                float t = mint;
                float h = 1.0;
                for (int i = 0; i < 32; i++)
                {
                    h = GetDist(ro + rd * t).x;
                    res = min(res, k * h / t);
                    t += clamp(h, 0.005, 0.1);
                }
                return clamp(res,0.0,1.0);
            }

            float3 calcNormal(in float3 pos)
            {
                float3  eps = float3(.001,0.0,0.0);
                float3 nor;
                nor.x = GetDist(pos + eps.xyy).x - GetDist(pos - eps.xyy).x;
                nor.y = GetDist(pos + eps.yxy).x - GetDist(pos - eps.yxy).x;
                nor.z = GetDist(pos + eps.yyx).x - GetDist(pos - eps.yyx).x;
                return normalize(nor);
            }


            float3 render(float3 ro, float3 rd, float3 light)
            {
                // background color
                float3 col = lerp(float3(0.3,0.2,0.1) * 0.5, float3(0.7, 0.9, 1.0), 0.5 + 0.5 * rd.y);

                float4 tmat = Raymarch(ro,rd);
                if (tmat.x > 0.0)
                {
                    float3  pos = ro + tmat.x * rd;
                    float3  nor = calcNormal(pos);

                    float occ = tmat.y;
                    float sha = softshadow(pos, light, 0.01, 64.0);

                    float dif = max(0.1 + 0.9 * dot(nor,light),0.0);
                    float sky = 0.5 + 0.5 * nor.y;
                    float bac = max(0.4 + 0.6 * dot(nor,float3(-light.x,light.y,-light.z)),0.0);

                    float3 lin = 0.0;
                    lin += 1.00 * dif * float3(1.10,0.85,0.60) * sha;
                    lin += 0.50 * sky * float3(0.10,0.20,0.40) * occ;
                    lin += 0.10 * bac * float3(1.00,1.00,1.00) * (0.5 + 0.5 * occ);
                    lin += 0.25 * occ * float3(0.15,0.17,0.20);

                    float3 matcol = float3(
                        0.5 + 0.5 * cos(0.0 + 2.0 * tmat.z),
                        0.5 + 0.5 * cos(1.0 + 2.0 * tmat.z),
                        0.5 + 0.5 * cos(2.0 + 2.0 * tmat.z));
                    col = mul(matcol, lin);
                }

                return pow(col, 0.4545);
            }


            fixed4 frag(v2f i) : SV_Target
            {
                /* float3 ro = 1.1*float3(2.5*sin(0.25*_Time.y),1.0+1.0*cos(_Time.y*.13),2.5*cos(0.25*_Time.y)); */

                float3 ro = i.ro;
                float3 rd = normalize(i.hitpos - ro);
                float3 col = render(ro, rd, i.light.xyz);

                return float4(col,1.0);
            }
            ENDCG
        }
    }
}
