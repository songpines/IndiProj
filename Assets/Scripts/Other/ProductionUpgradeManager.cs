using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

public class ProductionUpgradeManager : MonoBehaviour
{
    public static ProductionUpgradeManager Instance;

    [SerializeField] private int kowangStonePrice;
    [SerializeField] private int kowangCoralPrice;
    [SerializeField] private int memeteStonePrice;
    [SerializeField] private int memeteCoralPrice;

    [SerializeField] private float upgradeTimeMultiplier;
    [SerializeField] private float kowangSummonTime;
    [SerializeField] private float memetesSummonTime;

    private int memetAttackUpLevel;
    private int memetAttackSpeedUpLevel;
    private int memetAProductionLevel;


    private int kowangAttackUpLevel;
    private int kowangAttackSpeedUpLevel;
    private int kowangAProductionLevel;

    //private Dictionary<ProductionType, float> productionTimeReferenceDict;
    private Dictionary<ProductionType, int2> unitProductionPrice;
    private Dictionary<UpgradeType, int> memetUpgradeLevel;
    private Dictionary<UpgradeType, int> kowangUpgradeLevel;
    private Dictionary<UpgradeType, bool> memetIsUpgrading;
    private Dictionary<UpgradeType, bool> kowangIsUpgrading;
    private Dictionary<ProductionType, Dictionary<UpgradeType, int>> productUpgradeLevel;
    private Dictionary<ProductionType, Dictionary<UpgradeType, bool>> isUpgradingDict;


    private void Awake()
    {
        Instance = this;
    }


    private void Start()
    {
        unitProductionPrice = new Dictionary<ProductionType, int2>
        {
            {ProductionType.Kowang, new int2(kowangStonePrice, kowangCoralPrice)},
            {ProductionType.Memetes, new int2(memeteStonePrice, memeteCoralPrice) }
        };

        memetUpgradeLevel = new Dictionary<UpgradeType, int>
        {
            {UpgradeType.AttackUp , memetAttackUpLevel},
            {UpgradeType.AttackSpeedUp , memetAttackSpeedUpLevel},
            {UpgradeType.ProductionSpeedUp , memetAProductionLevel},
        };

        kowangUpgradeLevel = new Dictionary<UpgradeType, int>
        {
            {UpgradeType.AttackUp , kowangAttackUpLevel},
            {UpgradeType.AttackSpeedUp , kowangAttackSpeedUpLevel},
            {UpgradeType.ProductionSpeedUp , kowangAProductionLevel},
        };

        memetIsUpgrading = new Dictionary<UpgradeType, bool>
        {
            {UpgradeType.AttackUp , false},
            {UpgradeType.AttackSpeedUp , false},
            {UpgradeType.ProductionSpeedUp , false},
        };

        kowangIsUpgrading = new Dictionary<UpgradeType, bool>
        {
            {UpgradeType.AttackUp , false},
            {UpgradeType.AttackSpeedUp , false},
            {UpgradeType.ProductionSpeedUp , false},
        };

        //업그레이드 시간 및 상태
        productUpgradeLevel = new Dictionary<ProductionType, Dictionary<UpgradeType, int>>
        {
            { ProductionType.Memetes, memetUpgradeLevel },
            { ProductionType.Kowang, kowangUpgradeLevel },
        };

        //업그레이드 하고 있는지
        isUpgradingDict = new Dictionary<ProductionType, Dictionary<UpgradeType, bool>>
        {
            { ProductionType.Memetes, memetIsUpgrading },
            { ProductionType.Kowang, kowangIsUpgrading }
        };
    }

    public int2 GetProductionPrice(ProductionType productionType)
    {
        return unitProductionPrice[productionType];
    }
    public float GetProductionTime(ProductionType productionType)
    {
        if(productionType == ProductionType.Kowang)
        {
            return kowangSummonTime;
        }
        else
        {
            return memetesSummonTime;
        }
    }
    public float UpgradeTime(ProductionType productionType, UpgradeType upgradeType)
    {
        return productUpgradeLevel[productionType][upgradeType] * upgradeTimeMultiplier;
    }

    public void Upgrade(ProductionType productionType, UpgradeType upgradeType)
    {
        productUpgradeLevel[productionType][upgradeType]++;
    }

    public bool isUpgrading(ProductionType productionType, UpgradeType upgradeType)
    {
        return isUpgradingDict[productionType][upgradeType];
    }

    

}

public enum UpgradeType
{
    AttackUp,
    AttackSpeedUp,
    ProductionSpeedUp,
}
