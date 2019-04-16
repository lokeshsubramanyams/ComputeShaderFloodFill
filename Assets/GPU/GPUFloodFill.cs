using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GPUFloodFill : MonoBehaviour
{

    public ComputeShader CS_NodeFloodFill;
    public Texture2D source;
    public Color fillColor;
    public bool useLineShader = true;
    public Material testMaterial;

    private int CS_FloodFill_KID;
    private RenderTexture processingRT;
    private Material lineShader;
    private Vector2Int threadXY;

    private ComputeBuffer fillStackbuffer;

    private pixelInfo[] infos;
    void Start()
    {
            InputManager.mouseClickTexCoord += OnMouseClick;
            processingRT = new RenderTexture(source.width, source.height, 24)
            {
                enableRandomWrite = true
            };
            processingRT.enableRandomWrite = true;

            if (useLineShader)
            {
                lineShader = new Material(Shader.Find("Unlit/LinesShader"));
                Graphics.Blit(source, processingRT, lineShader);
            }
            else
                Graphics.Blit(source, processingRT);

            CS_FloodFill_KID = CS_NodeFloodFill.FindKernel("CSMain");

            CS_NodeFloodFill.SetTexture(CS_FloodFill_KID, "Result", processingRT);

            CS_NodeFloodFill.SetInt("sizeX", processingRT.width);

            CS_NodeFloodFill.SetInt("sizeY", processingRT.height);
        
            Vector2Int click = new Vector2Int(Mathf.RoundToInt(source.width * 0.5f), Mathf.RoundToInt(source.height * 0.5f));

            fillStackbuffer = SetBuffer(click, source);

            CS_NodeFloodFill.SetBuffer(CS_FloodFill_KID, "pixerInfos", fillStackbuffer);

            threadXY = new Vector2Int(Mathf.RoundToInt(processingRT.width / 8), Mathf.RoundToInt(processingRT.height / 8));

            CS_NodeFloodFill.SetInt("process", 0);//Process = 0 will collect all info about textures

            CS_NodeFloodFill.Dispatch(CS_FloodFill_KID, threadXY.x, threadXY.y, 1);

            CS_NodeFloodFill.SetInt("process", 1);//process = 1 will start filling based on click info

            testMaterial.mainTexture = processingRT;

    }

    private void OnMouseClick(Vector2 click)
    {
        Debug.Log("Click position=" + click);

        infos = new pixelInfo[processingRT.width * processingRT.height];

        Vector2Int texSpace = new Vector2Int(Mathf.RoundToInt(click.x * processingRT.width), Mathf.RoundToInt(click.y * processingRT.height));

        fillStackbuffer.GetData(infos);

        int clickIndex = Mathf.RoundToInt(texSpace.x + texSpace.y * processingRT.width);

        if (infos[clickIndex].dontfill ==0)
        {
            infos[clickIndex].color = fillColor.ToVector();

            infos[clickIndex].seedPixel = 1;

            fillStackbuffer.SetData(infos);
        }
        else
        {
           // lineShader.color = fillColor;
           //ToDo: blit back to line shader to change line color
        }

    }

    private void Update()
    {
        CS_NodeFloodFill.Dispatch(CS_FloodFill_KID, threadXY.x, threadXY.y, 1);
        CS_NodeFloodFill.Dispatch(CS_FloodFill_KID, threadXY.x, threadXY.y, 1);
        CS_NodeFloodFill.Dispatch(CS_FloodFill_KID, threadXY.x, threadXY.y, 1);
        CS_NodeFloodFill.Dispatch(CS_FloodFill_KID, threadXY.x, threadXY.y, 1);
        CS_NodeFloodFill.Dispatch(CS_FloodFill_KID, threadXY.x, threadXY.y, 1);
    }
    private void OnDisable()
    {
        fillStackbuffer.Release();
        fillStackbuffer.Dispose();
        InputManager.mouseClickTexCoord -= OnMouseClick;
    }
    void DebugBUffer(int count)
    {
        pixelInfo[] infos = new pixelInfo[processingRT.width * processingRT.height];

        fillStackbuffer.GetData(infos);
        int length = count < 0 ? infos.Length : count;
        for (int i = 0; i < length; i++)
        {
            Debug.LogFormat("{0};north ={1};south ={2};Color = {3}",
                infos[i].id.value, infos[i].north.value, infos[i].south.value, infos[i].color);
        }
    }
    
    private ComputeBuffer SetBuffer(Vector2Int seedPixel, Texture2D texture)//Change Texture2D to RenderTexture
    {
        int sizeOfpixelInfo = System.Runtime.InteropServices.Marshal.SizeOf(typeof(pixelInfo));

        int length = texture.width * texture.height;

        pixelInfo[] stack = new pixelInfo[length];

        ComputeBuffer Stackbuffer = new ComputeBuffer(length, sizeOfpixelInfo);

        Stackbuffer.SetData(stack);

        return Stackbuffer;
    }

}
