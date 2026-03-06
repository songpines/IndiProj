using System;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class BuildingSelectedUI : MonoBehaviour
{
    private BaseBuildingAuthoring selectedBase;


    [SerializeField] private Button memetesProduceButton;
    [SerializeField] private Button kowangProduceButton;

    [SerializeField] private Button memetesAttackUpgradeButton;
    [SerializeField] private Button memetesAttackSpeed;
    [SerializeField] private Button memetesProduceSpeed;

    [SerializeField] private Button kowangAttackUpgradeButton;
    [SerializeField] private Button kowangAttackSpeed;
    [SerializeField] private Button kowangProductionSpeed;



    public event EventHandler<OnProductionButtonPressedEventArgs> OnProductButtonPressed;
    public class OnProductionButtonPressedEventArgs
    {
        public ProductionType productionType;
    }


    public event EventHandler<OnUpgradeButtonPressedEventArgs> OnUpgradeButtonPressed;
    public class OnUpgradeButtonPressedEventArgs
    {
        public UpgradeType upgradeType;
    }

    EntityManager entityManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        memetesProduceButton.onClick.AddListener(OnClickMemetesProduction);
        kowangProduceButton.onClick.AddListener(OnClickKowangProduction);
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnClickMemetesProduction()
    {
        if(ResourceManager.Instance.Stone < ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Memetes).x 
            || ResourceManager.Instance.Coral < ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Memetes).y)
        {
            SoundManager.Instance.PlayResourceLackSound();
            return;
        }
        ResourceManager.Instance.Stone -= ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Memetes).x;
        ResourceManager.Instance.Coral -= ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Memetes).y;

        Entity entity = UnitSelectionManager.Instance.selectedBuildingEntity;
        if (!entityManager.IsComponentEnabled<BaseBuildingUnitProduction>(entity)) {
            BaseBuildingUnitProduction unitProduction = entityManager.GetComponentData<BaseBuildingUnitProduction>(entity);
            unitProduction.timerMax = ProductionUpgradeManager.Instance.GetProductionTime(ProductionType.Memetes);
            unitProduction.productionType = ProductionType.Memetes;
            entityManager.SetComponentData<BaseBuildingUnitProduction>(entity, unitProduction);
            entityManager.SetComponentEnabled<BaseBuildingUnitProduction>(entity, true);
            BaseBuilding baseBuilding = entityManager.GetComponentData<BaseBuilding>(entity);
            baseBuilding.isSummoning = true;
        }

        OnProductButtonPressed?.Invoke(this, new OnProductionButtonPressedEventArgs { productionType = ProductionType.Memetes });
    }

    private void OnClickKowangProduction()
    {
        if (ResourceManager.Instance.Stone < ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Kowang).x
            || ResourceManager.Instance.Coral < ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Kowang).y)
        {
            SoundManager.Instance.PlayResourceLackSound();
            return;
        }
        ResourceManager.Instance.Stone -= ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Kowang).x;
        ResourceManager.Instance.Coral -= ProductionUpgradeManager.Instance.GetProductionPrice(ProductionType.Kowang).y;

        Entity entity = UnitSelectionManager.Instance.selectedBuildingEntity;
        if (!entityManager.IsComponentEnabled<BaseBuildingUnitProduction>(entity))
        {
            BaseBuildingUnitProduction unitProduction = entityManager.GetComponentData<BaseBuildingUnitProduction>(entity);
            unitProduction.timerMax = ProductionUpgradeManager.Instance.GetProductionTime(ProductionType.Kowang);
            unitProduction.productionType = ProductionType.Kowang;
            entityManager.SetComponentData<BaseBuildingUnitProduction>(entity, unitProduction);
            entityManager.SetComponentEnabled<BaseBuildingUnitProduction>(entity, true);
            BaseBuilding baseBuilding = entityManager.GetComponentData<BaseBuilding>(entity);
            baseBuilding.isSummoning = true;
        }

        OnProductButtonPressed?.Invoke(this, new OnProductionButtonPressedEventArgs { productionType = ProductionType.Kowang });
    }
    

    private void OnClickMemetesAtkUp()
    {

    }
    private void OnClickMemetesAtkSpdUp()
    {

    }
    private void OnClickMemetesProdSpdUp()
    {

    }
    private void OnClickKowangAtkUp()
    {

    }
    private void OnClickKowangAtkSpdUp()
    {

    }
    private void OnClickKowangPrdSpdUp()
    {

    }
}
