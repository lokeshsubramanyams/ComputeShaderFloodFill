using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextureBlitShaderExportEditor : EditorWindow
{
    static TextureBlitShaderExportEditor window;
    Object source;
    static string path;


    List<Texture2D> inputTextures;
    List<Texture2D> outputTextures;
    int textureCount;
    Shader shader;

    private void OnEnable()
    {
        inputTextures = new List<Texture2D>();
        outputTextures = new List<Texture2D>();
    }
    [MenuItem("Tools/TextureBlit")]
    static void Init()
    {
        window = (TextureBlitShaderExportEditor)EditorWindow.GetWindow(typeof(TextureBlitShaderExportEditor));
        window.position = new Rect(0, 500, 500, 300);
        window.Show();

       

    }
    void OnGUI()
    {
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        textureCount = EditorGUILayout.IntField("TextureCount", textureCount);

        for (int i = 0; i < textureCount - inputTextures.Count;i++)
        {
            inputTextures.Add(null);
        }

        for (int i = 0; i < textureCount; i++)
        {
            inputTextures[i] = TextureField("Texture_"+i, inputTextures[i]);
        }
       

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        shader = (Shader)EditorGUILayout.ObjectField(shader, typeof(Shader), false);

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        if (GUILayout.Button("Blit"))
        {
            if(shader!=null)
            {
                for (int i = 0; i < textureCount; i++)
                {
                    if(inputTextures[i]!=null)
                    {
                        outputTextures.Add(Blit(inputTextures[i]));
                    }
                }
            }
        }

        EditorGUILayout.BeginHorizontal();

        for (int i = 0; i < outputTextures.Count; i++)
        {
            outputTextures[i] =  TextureField("Texture_" + i, outputTextures[i]);

            byte[] bytes = outputTextures[i].EncodeToPNG();
            Object.Destroy(outputTextures[i]);

            File.WriteAllBytes(Application.dataPath + "/../SavedScreen.png", bytes);
        }


        EditorGUILayout.EndHorizontal();

               
        /* if (GUILayout.Button("Select Export Folder"))
         {

             path = EditorUtility.OpenFolderPanel("Select Export Folder", "", "Assets");
             path = EditorUtilities.GetAssetPath(path);

         }

         if (GUILayout.Button("Export"))
         {
             if (source != null && !string.IsNullOrEmpty(path))
             {
                 ExportJson(source, path);
             }
         }*/
    }

    private Texture2D Blit(Texture2D texture)
    {
        RenderTexture tempTexture = new RenderTexture(texture.width, texture.height, 24, RenderTextureFormat.ARGB32);

        tempTexture.enableRandomWrite = true;

        Material mat = new Material(shader);

        Graphics.Blit(texture, tempTexture, mat);

        Texture2D newTexture = new Texture2D(texture.width, texture.height, TextureFormat.ARGB32, false);

        RenderTexture.active = tempTexture;

        newTexture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);

        newTexture.Apply();

        RenderTexture.active = null;

        tempTexture.Release();

        return newTexture;
    }

    void ExportJson(object obj, string path)
    {
        string json = JsonUtility.ToJson(obj);
        Debug.Log(json);
        path += "/" + obj.GetType().ToString() + ".json";
        File.WriteAllText(path, json);
    }

    private static Texture2D TextureField(string name, Texture2D texture)
    {
        GUILayout.BeginVertical();
        var style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.UpperCenter;
        style.fixedWidth = 70;
        GUILayout.Label(name, style);
        var result = (Texture2D)EditorGUILayout.ObjectField(texture, typeof(Texture2D), false, GUILayout.Width(70), GUILayout.Height(70));
        GUILayout.EndVertical();
        return result;
    }
}
