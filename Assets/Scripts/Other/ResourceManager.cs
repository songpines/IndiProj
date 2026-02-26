using TMPro;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance { get; private set; }

    public int stone;
    public int Stone { 
        get { return stone; }
        set
        {
            stone = value;
            stoneUI.text = $"Stone : {stone}";
        }
    }

    public int coral;
    public int Coral
    {
        get { return coral; }
        set
        {
            coral = value;
            coralUI.text = $"Coral : {coral}";
        }
    }
    [SerializeField] private TextMeshProUGUI stoneUI;
    [SerializeField] private TextMeshProUGUI coralUI;

    private void Awake()
    {
        Instance = this;
    }
}
