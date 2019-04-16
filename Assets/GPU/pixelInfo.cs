using UnityEngine;

struct vec4Int
{
    public float x;
    public float y;
    public float z;
    public float a;

    public string value
    {
        get
        {
            return "(" + x + "," + y + "," + z + "," + a + ")";
        }
    }
}
struct vec2Int
{
    public uint x;
    public uint y;
    public string value
    {
        get
        {
            return "(" + x + "," + y + ")";
        }
    }
};
struct pixelInfo
{
    public vec2Int id;
    public vec2Int north, south, east, west, north_east, north_west, south_east, south_west;
    public Vector4 color;
    public int seedPixel;
    public int dontfill;//its local for compute shader
};
