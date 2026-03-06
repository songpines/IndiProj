using System.Collections;
using System.Threading;
using Unity.Entities;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    private EntityManager entityManager;

    [SerializeField] private GameObject middleUnitSelectionPanel;
    [SerializeField] private GameObject buildingSelectedPanel;
    private BuildingSelectedUI buildingSelectedUI;
    [SerializeField] private GameObject buildingSelectedJobProgressingPanel;

    private Panels panels;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        //빌딩 선택되었을 때 ui
        buildingSelectedUI = buildingSelectedPanel.GetComponent<BuildingSelectedUI>();
        //buildingSelectedUI.OnProductButtonPressed += BuildingSelectedUI_OnProductButtonPressed;
        //buildingSelectedUI.OnUpgradeButtonPressed += BuildingSelectedUI_OnUpgradeButtonPressed;




        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    //private void BuildingSelectedUI_OnProductButtonPressed(object sender, BuildingSelectedUI.OnProductionButtonPressedEventArgs e)
    //{
    //    throw new System.NotImplementedException();
    //}

    //private void BuildingSelectedUI_OnUpgradeButtonPressed(object sender, BuildingSelectedUI.OnUpgradeButtonPressedEventArgs e)
    //{
    //    throw new System.NotImplementedException();
    //}


    // Update is called once per frame
    void Update()
    {

    }

    public void panelChange(Panels panels)
    {
        AllShut();
        switch (panels)
        {
            case Panels.unitSelection:
                middleUnitSelectionPanel.SetActive(true);
                break;
            case Panels.buildingSelected:
                buildingSelectedPanel.SetActive(true);
                break;
            //case Panels.buildingSelectedJobProgressing:
            //    buildingSelectedJobProgressingPanel.SetActive(true);
            //    break;
        }
    }


    public void AllShut()
    {
        middleUnitSelectionPanel.SetActive(false);
        buildingSelectedPanel.SetActive(false);
        buildingSelectedJobProgressingPanel.SetActive(false);
    }

    private void OnJob()
    {

    }
}
//    IEnumerator JobTime(ProductionType type)
//    {
//        Entity entity = UnitSelectionManager.Instance.selectedBuildingEntity;
//        entityManager.getcomppo
//        float timer = 0;
//        float timerMax = ProductionUpgradeManager.Instance.GetProductionTime(type);
//        while(timer < timerMax)
//        {
//            timer += Time.deltaTime;
//            yield return null;
//        }
//        entityManager.SetComponentData<>
//    }
//}


public enum Panels
{
    unitSelection,
    buildingSelected,
    //buildingSelectedJobProgressing,
}
