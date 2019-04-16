using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Returns only mesh collider")]
    public bool info;
    public static event System.Action<Vector2> mouseClickTexCoord;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hitinfo, 500))
            {
                if (hitinfo.collider.GetType() == typeof(MeshCollider))
                {

                    mouseClickTexCoord(hitinfo.textureCoord);
                }

            }
        }
    }
}
