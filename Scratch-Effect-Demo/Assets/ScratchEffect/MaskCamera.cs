using UnityEngine;
using System;
using System.Collections;

public class MaskCamera : MonoBehaviour
{
    public GameObject Dust;
    Rect ScreenRect;
    public RenderTexture rt;

    public Material EraserMaterial;
    private bool firstFrame;
    private Vector2? newHolePosition;

    public bool _requestReadPixel = false;
    private Action<float> callback;
    public void getPercent(Action<float> _callback)
    {
        callback = _callback;
        _requestReadPixel = true;
    }
    private void CutHole(Vector2 imageSize, Vector2 imageLocalPosition)
    {
        Rect textureRect = new Rect(0.0f, 0.0f, 1.0f, 1.0f);
        Rect positionRect = new Rect(
            (imageLocalPosition.x - 0.5f * EraserMaterial.mainTexture.width) / imageSize.x,
            (imageLocalPosition.y - 0.5f * EraserMaterial.mainTexture.height) / imageSize.y,
            EraserMaterial.mainTexture.width / imageSize.x,
            EraserMaterial.mainTexture.height / imageSize.y);
        GL.PushMatrix();
        GL.LoadOrtho();

        for (int i = 0; i < EraserMaterial.passCount; i++)
        {
            EraserMaterial.SetPass(i);
            GL.Begin(GL.QUADS);
            GL.Color(Color.white);
            GL.TexCoord2(textureRect.xMin, textureRect.yMax);
            GL.Vertex3(positionRect.xMin, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMax);
            GL.Vertex3(positionRect.xMax, positionRect.yMax, 0.0f);
            GL.TexCoord2(textureRect.xMax, textureRect.yMin);
            GL.Vertex3(positionRect.xMax, positionRect.yMin, 0.0f);
            GL.TexCoord2(textureRect.xMin, textureRect.yMin);
            GL.Vertex3(positionRect.xMin, positionRect.yMin, 0.0f);
            GL.End();
        }
        GL.PopMatrix();
        /////////////////////
    }

    public IEnumerator Start()
    {
        firstFrame = true;
        _requestReadPixel = false;
        //Get Erase effect boundary area
        ScreenRect.x = Dust.GetComponent<Renderer>().bounds.min.x;
        ScreenRect.y = Dust.GetComponent<Renderer>().bounds.min.y;
        ScreenRect.width = Dust.GetComponent<Renderer>().bounds.size.x;
        ScreenRect.height = Dust.GetComponent<Renderer>().bounds.size.y;
        //Create new render texture for camera target texture
        rt = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.Default);
        yield return rt.Create();
        _left = rt.width / 5;
        _top = rt.height / 4;
        tex = new Texture2D(rt.width - 2 * _left, rt.height - 2 * _top, TextureFormat.RGB24, false);
        GetComponent<Camera>().targetTexture = rt;
        //Set Mask Texture to dust material to Generate Dust erase effect
        Dust.GetComponent<Renderer>().material.SetTexture("_MaskTex", rt);
    }

    public void FixedUpdate()
    {
        newHolePosition = null;
        if (Input.GetMouseButton(0))
        {
            Vector2 v = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            if (ScreenRect.Contains(v))
            {
                newHolePosition = new Vector2(1600 * (v.x - ScreenRect.xMin) / ScreenRect.width, 1200 * (v.y - ScreenRect.yMin) / ScreenRect.height);
            }
        }
    }

    int _left;
    int _top;
    public void OnPostRender()
    {
        if (firstFrame)
        {
            firstFrame = false;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
        }
        if (newHolePosition != null)
        {
            CutHole(new Vector2(1600.0f, 1200.0f), newHolePosition.Value);
        }
        if (_requestReadPixel)
        {
            tex.ReadPixels(new Rect(_left, _top, rt.width - 2 * _left, rt.height - 2 * _top), 0, 0);
            _requestReadPixel = false;
            float percent = caculatorPrercent(tex);
            Debug.Log("Percent:" + percent);
            if (callback != null)
            {
                callback(percent);
            }
        }
    }
    // use to caculator
    Texture2D tex;
    public static float caculatorPrercent(Texture2D tex)
    {
        int count = 0;
        for (int x = 0; x < tex.width; x++)
        {
            for (int y = 0; y < tex.height; y++)
            {
                if (tex.GetPixel(x, y).r == 1)
                    count++;
            }
        }
        float _percent = (count * 100 / ((tex.width) * (tex.height)));
        return _percent;
    }
}