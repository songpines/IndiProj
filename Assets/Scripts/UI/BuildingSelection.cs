using UnityEngine;
using UnityEngine.UI;

public class BuildingSelection : MonoBehaviour
{
    [SerializeField] private Button skullBuildingButton;
    [SerializeField] private Button wallButton;
    [SerializeField] private Button destroyButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        skullBuildingButton.onClick.AddListener(OnSkullBuildingClick);
        wallButton.onClick.AddListener(OnWallBuildingClick);
        destroyButton.onClick.AddListener(OnDestroyClick);
    }

    private void OnSkullBuildingClick()
    {
        BuildingManager.Instance.ExitBuidlingMode();
        BuildingManager.Instance.EnterBuildingMode(BuildingType.BaseBuilding);
    }

    private void OnWallBuildingClick()
    {
        BuildingManager.Instance.ExitBuidlingMode();
        BuildingManager.Instance.EnterBuildingMode(BuildingType.Wall);
    }

    private void OnDestroyClick()
    {
        BuildingManager.Instance.ExitBuidlingMode();
        BuildingManager.Instance.EnterDeconstructionMode();
    }
}
