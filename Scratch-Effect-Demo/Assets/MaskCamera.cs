using UnityEngine;
using System.Collections;
using System;

public class MaskCamera : MonoBehaviour
{
    public GameObject Dust;
    Rect ScreenRect;
    RenderTexture rt;
    int holeWidth;
    int holeHeight;
    public Material EraserMaterial;
    private bool firstFrame;
    private Vector2? newHolePosition;

    private Texture2D tex;    
    bool _requestReadPixel = false;
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
    }

    public IEnumerator Start()
    {
        holeWidth = Screen.width;
        holeHeight = Screen.height;
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
        tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
        GetComponent<Camera>().targetTexture = rt;
        //Set Mask Texture to dust material to Generate Dust erase effect
        Dust.GetComponent<Renderer>().material.SetTexture("_MaskTex", rt);
    }

    public void Update()
    {
        newHolePosition = null;
        if (Input.GetMouseButton(0))
        {
            Vector2 v = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
            if (ScreenRect.Contains(v))
            {
                newHolePosition = new Vector2(holeWidth * (v.x - ScreenRect.xMin) / ScreenRect.width, holeHeight * (v.y - ScreenRect.yMin) / ScreenRect.height);
            }
        }
    }

	public void OnPostRender()
	{
	    if (firstFrame)
	    {
	        firstFrame = false;
            GL.Clear(false, true, new Color(0.0f, 0.0f, 0.0f, 0.0f));
	    }
        if (newHolePosition != null)
        {
            CutHole(new Vector2(holeWidth, holeHeight), newHolePosition.Value);
        }
        if (_requestReadPixel)
        {
            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            float _percent = caculatoPrercent(tex);
            _requestReadPixel = false;
            if (callback != null)
            {
                callback(_percent);
            }
        }
	}
    float caculatoPrercent(Texture2D tex)
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
