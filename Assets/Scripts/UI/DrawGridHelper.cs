using UnityEngine;

public class DrawGridHelper : MonoBehaviour
{
    public int Width;
    public int Height;
    public float cellSize;
    // Start is called once before the first execution of Update after the MonoBehaviour is created



    // Update is called once per frame
    void Update()
{
        for (int i = 0; i < Width; i++)
        {
            for (int j = 0; j < Height; j++)
            {
                Debug.DrawLine(new Vector3(0, 0, j), new Vector3(Width, 0, j));
                Debug.DrawLine(new Vector3(i, 0, 0), new Vector3(i, 0, Height));
            }
        }
    }
}
