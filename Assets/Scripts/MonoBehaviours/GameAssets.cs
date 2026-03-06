using UnityEngine;

public class GameAssets : MonoBehaviour
{
    public const int UNITS_LAYER = 6;
    public const int BUILDING_LAYER = 7;
    public const int RESOURCE_LAYER = 15;


    public static GameAssets Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }
}
