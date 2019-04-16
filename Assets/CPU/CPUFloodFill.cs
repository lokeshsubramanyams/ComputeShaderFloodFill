using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPUFloodFill : MonoBehaviour
{
    public Texture2D source;
    public Color fillColor;
    public Material testmaterial;
    private Material lineShader;
    private Texture2D validating;

    private Stack<Vector2Int> fillStack;

    private void OnEnable()
    {
        InputManager.mouseClickTexCoord += OnMouseClick;
    }

    void Start()
    {
        validating = new Texture2D(source.width, source.height);

        ////////////////////////////////////////////////////////////////////////

        RenderTexture TemperaryIntermediateTex2D = RenderTexture.GetTemporary(source.width, source.height, 24, RenderTextureFormat.ARGB32);

        lineShader = new Material(Shader.Find("Unlit/LinesShader"));
        
        Graphics.Blit(source, TemperaryIntermediateTex2D, lineShader);

        RenderTexture.active = TemperaryIntermediateTex2D;

        validating.ReadPixels(new Rect(0,0, source.width, source.height), 0, 0);

        RenderTexture.active = null;

        TemperaryIntermediateTex2D.Release();

        validating.Apply();

        ////////////////////////////////////////////////////////////////////////
        
        testmaterial.mainTexture = validating;
    }
     private void OnMouseClick(Vector2 texCoord)
    {
        FloodFill(GetClickPixelSpaceCoord(texCoord, validating));
       
    }
    

    private void FloodFill(Vector2Int pixelCoord)
    {
         Color ClickedColor = validating.GetPixel(pixelCoord.x, pixelCoord.y);

        Debug.Log("Clicked Pixel  = " + ClickedColor);

        fillStack = new Stack<Vector2Int>();

        fillStack.Push(pixelCoord);
        
        while (fillStack.Count > 0)
        {
            Vector2Int currentPixel = fillStack.Pop();

            validating.SetPixel(currentPixel.x, currentPixel.y, fillColor);

            FillStackWithNeighbours(currentPixel, ClickedColor);
           
        }
        validating.Apply();

    }
    private Vector2Int GetClickPixelSpaceCoord(Vector2 uv, Texture2D source)
    {
        return new Vector2Int(Mathf.RoundToInt(source.width * uv.x), Mathf.RoundToInt(source.height * uv.y));
    }
    private void FillStackWithNeighbours(Vector2Int pixel,Color validateColor)
    {
        Vector2Int neighbour = new Vector2Int(pixel.x + 1, pixel.y);
        FillStack(neighbour, validateColor);

        neighbour = new Vector2Int(pixel.x , pixel.y+1);
        FillStack(neighbour, validateColor);

        neighbour = new Vector2Int(pixel.x-1, pixel.y);
        FillStack(neighbour, validateColor);

        neighbour = new Vector2Int(pixel.x , pixel.y-1);
        FillStack(neighbour, validateColor);

        neighbour = new Vector2Int(pixel.x-1, pixel.y - 1);
        FillStack(neighbour, validateColor);

        neighbour = new Vector2Int(pixel.x + 1, pixel.y +1);
        FillStack(neighbour, validateColor);
    }

    private void FillStack(Vector2Int nPixel,Color valideColor)
    {
        if (nPixel.x < 0 || nPixel.y < 0) return;
        if (nPixel.x > validating.width || nPixel.y > validating.height) return;

        Color _color = validating.GetPixel(nPixel.x, nPixel.y);

        if (_color == valideColor)
        {
            fillStack.Push(nPixel);
            //Debug.Log(nPixel);
        }
    }

    private void OnDisable()
    {
        InputManager.mouseClickTexCoord -= OnMouseClick;
    }
}
