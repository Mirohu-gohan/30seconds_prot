// Editor上でノイズテクスチャを生成するスクリプト（使い捨てOK）
using UnityEngine;
using UnityEditor;

public class NoiseTexGen : MonoBehaviour
{
    [MenuItem("Tools/Generate Noise Texture")]
    static void Generate()
    {
        var tex = new Texture2D(256, 256);
        for (int y = 0; y < 256; y++)
            for (int x = 0; x < 256; x++)
            {
                float v = Mathf.PerlinNoise(x * 0.05f, y * 0.05f);
                tex.SetPixel(x, y, new Color(v, v, v));
            }
        tex.Apply();
        System.IO.File.WriteAllBytes("Assets/NoiseTex.png",
            tex.EncodeToPNG());
        AssetDatabase.Refresh();
    }
}