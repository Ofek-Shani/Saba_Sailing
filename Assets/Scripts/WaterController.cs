using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using Unity.VisualScripting;
using UnityEngine.UIElements;

public class WaterController : MonoBehaviour
{

    public Sprite prefabSprite;
    [Range(1f, 5f)] public float waveCycle = 2f;
    //[Range(0f,1f)] public float dx = 0.5f;
    //[Range(0f,1f)] public float dy = 0.5f;
    Pixel[] dots;
    SpriteRenderer spriteRenderer;
    Color[] origColors, newColors, colors;
    Texture2D texture;
    int width, height, border = 1, length;
    Color innerColor, borderColor; 
    WindController wc;
    float direction, speed;
    //texture.GetPixels()[texture.width-1]; //colors[texture.width * texture.height /2 + texture.width /2];
    void Start()
    {
        wc = GameObject.FindGameObjectWithTag("Wind").GetComponent<WindController>();
        direction = wc.windDirection * Mathf.Deg2Rad;
        speed = wc.windStrength / 5f;

        innerColor = new Color(0.020f, 0.267f, 0.580f, 1.000f);
        borderColor = Color.Lerp(innerColor, Color.white, 0.5f); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        Sprite sprite = spriteRenderer.sprite;
        texture = sprite.texture;
        width = texture.width;
        height = texture.height;
        length = width * height;
        colors = new Color[length];
        origColors = new Color[length]; 
        prepareTexture(true, Vector2.zero);
    }

    struct Pixel {
        public int x,y,r;
        public float delay;
        int mx, my;
        int width, height;
        float state;
        int dir;
        float xrange, yrange;
        public Pixel(int mx, int my, int radius, int width, int height, float xr, float yr) {
            delay = UnityEngine.Random.Range(-5f,5f);
            this.mx = mx;
            this.my = my;
            x = this.mx;
            y = this.my;
            r = radius;
            this.width = width;
            this.height = height;
            state = 0f;
            dir = 1;
            xrange = xr;
            yrange = yr;
            Update();
        }
        public void Update() {
            x = (width + Mathf.RoundToInt(mx + UnityEngine.Random.Range(-xrange/2f, xrange/2f))) % width;
            y = (height + Mathf.RoundToInt(my + UnityEngine.Random.Range(-yrange/2f, yrange/2f))) % height;
        }
        public float UpdateState(float waveCycle) {
            int oldDir = dir;
            float oldState = state;
            state = (1f + Mathf.Sin((Time.timeSinceLevelLoad + delay) * waveCycle)) / 2f;
            this.dir = state > oldState ? 1 : -1;
            if (oldDir > 0 && this.dir < 0) Update();
            return state;
        }
    }
    void prepareTexture(bool initialize, Vector2 offset) {
        float directionRad = direction;
        int length = width * height;
        if (initialize) {
            dots = CreateRandomDots(width, height, 2, 2, 15, 5);
            initializeTexture(origColors, width, height, border, borderColor, innerColor);
        }
        Array.Copy(origColors, colors, length);
        for (int pi = 0; pi < dots.Length; pi++) {
            float state = dots[pi].UpdateState(waveCycle);
            Pixel dot = dots[pi];
            float cycle = Time.timeSinceLevelLoad * waveCycle % Mathf.PI;
            float vanish = Mathf.Max(0.5f,state);
            Color targetColor = initialize ? Color.white : Color.Lerp(Color.white, innerColor, state);
            float x,y;
            Vector2 vnew = new(0,0);
            float dotx = (width + dot.x + offset.x) % width;
            float doty = (height + dot.y + offset.y) % height;
            Vector2 vback = new Vector2(dotx - dot.r * Mathf.Cos(directionRad), doty - dot.r * Mathf.Sin(directionRad));
            Vector2 vback2 = new Vector2(dotx - dot.r * Mathf.Cos(directionRad) * 4, doty - dot.r * Mathf.Sin(directionRad) * 4);
            Vector2 vdot = new Vector2(dotx, doty);
            for (x = dotx - dot.r; x < dotx + dot.r; x+=1f) {
                for (y = doty - dot.r; y < doty + dot.r; y+=1f) {
                    vnew.x = x; vnew.y = y;
                    float d = Vector2.Distance(vnew, vdot);
                    int fx = Mathf.RoundToInt(x);
                    int fy = Mathf.RoundToInt(y);
                    if ( d <= dot.r && 
                              Vector2.Distance(vnew, vback) / (dot.r * 2f) >= (0.2f + vanish) &&
                              Vector2.Distance(vnew, vback2) > dot.r * 4f ) {
                        float frac = Mathf.Sin(Mathf.PI/2 * (1 - d / dot.r));           //Mathf.Min(1f, dot.r - d);
                        int index = (length + fy*width + fx) % length;
                        colors[index] = Color.Lerp(origColors[index], targetColor, frac);
            }   }   }
        }
        texture.SetPixels(colors);
        texture.Apply();
    }
    void  initializeTexture(Color[] colors, int width, int height, int border, Color borderColor, Color innerColor) {
        for (int i = 0; i < width; i++) for (int j = 0; j < height; j++) {
            colors[i + j * width] = (i < border || j < border )? borderColor : innerColor;
        }
    }
    Pixel[] CreateRandomDots(int xn, int yn, int minDistance=2, int border=3, int r = 5, int limit = 3) {
        List<Pixel> dots = new List<Pixel>();
        //int limit = Math.Min(10, yn * xn / 30);
        int validDots = 0;
        int[] coord = {0,0};
        int i=0, j=0;
        float il,jl = Mathf.Sqrt(limit);
        il = jl;
        // return new Pixel[]{new Pixel(new int[]{Mathf.RoundToInt(dx * width), Mathf.RoundToInt(dy * height)}, 10)};
        while (validDots < limit) {
            int radius = UnityEngine.Random.Range(r/2,r);
            if (i > il) j++;
            if (j > jl) break;
            // MakeRandomDot(coord,i,j,xn,yn);
            //int tries = 0;
            //while(!ValidDot(coord, radius) && (++tries) < 10)
            //    MakeRandomDot(coord);
            dots.Add(new Pixel(Mathf.RoundToInt(i * xn/(il+1)), Mathf.RoundToInt(j * yn/(jl+1)), 
                radius, xn, yn, xn/(il + 1), yn/(jl + 1)));
            validDots++;
            i++;
        }
        return dots.ToArray();

        //void MakeRandomDot(int[] coord, int i, int j)
        //{
        //    coord[0] = UnityEngine.Random.Range(0, xn-1);
        //   coord[1] = UnityEngine.Random.Range(0, yn-1);
        //}
        bool ValidDot(int[] coord, int radius) {
            for (int i = 0; i <validDots; i++)
                if (Distance(coord, dots[i]) <= (minDistance + radius + dots[i].r)) return false;
            return true;    
        }
        float Distance(int[] dot1, Pixel dot2) {
            return Mathf.Max(Math.Abs(dot1[0] - dot2.x), Math.Abs(dot1[1] - dot2.y));
        }
    }
    private Vector2 offset = Vector2.zero;
    float state = 1f;
    int change = -1;
    // Update is called once per frame
    void Update()
    {
        direction = wc.windDirection * Mathf.Deg2Rad;
        speed = wc.windStrength / 5f;
        float delta = Time.deltaTime * speed * 4f;
        offset.x += delta * Mathf.Cos(direction);
        offset.y += delta * Mathf.Sin(direction);
        offset.x = (width + offset.x) % width;
        offset.y = (height + offset.y) % height;
        prepareTexture(false, offset);
    }
}
