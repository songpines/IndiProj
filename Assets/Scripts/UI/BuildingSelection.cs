using UnityEngine;
using UnityEngine.UI;

public class BuildingSelection : MonoBehaviour
{
    [SerializeField] private Button skullBuildingButton;
    [SerializeField] private Button wallButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skullBuildingButton.onClick.AddListener(OnSkullBuildingClick);
        wallButton.onClick.AddListener(OnWallBuildingClick);
    }

    private void OnSkullBuildingClick()
    {
        BuildingManager.Instance.EnterBuildingMode(BuildingType.BaseBuilding);
    }

    private void OnWallBuildingClick()
    {
        BuildingManager.Instance.EnterBuildingMode(BuildingType.Wall);
    }
}
