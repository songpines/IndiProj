using System.Collections;
using Unity.Entities;
using UnityEngine;

public class BaseBuildingAuthoring : MonoBehaviour
{

    public class Baker : Baker<BaseBuildingAuthoring>
    {
        public override void Bake(BaseBuildingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BaseBuilding());
        }
    }





    //[SerializeField] private ProductionUpgradeManager upgradeManager;


    //private BuildingJobState currentBuildingJobState;

    //private float timer;
    //private float timerMax;

    //void Start()
    //{
    //    currentBuildingJobState = BuildingJobState.Idle;
    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}

    //public void BaseJob()
    //{
    //    switch (currentBuildingJobState)
    //    {
    //        case BuildingJobState.Idle:

    //            break;

    //        case BuildingJobState.Upgrading:

    //            break;

    //        case BuildingJobState.Producing:

    //            break;
    //    }
    //}

    //public void Upgrade(ProductionType ptype, UpgradeType utype)
    //{
    //    if (ProductionUpgradeManager.Instance.isUpgrading(ptype, utype)) return;

    //    //같은 업그레이드 중이 아니라면
    //    StartCoroutine(UpgradeCoroutine(ptype, utype));
    //}

    //public void Produce(ProductionType type)
    //{
    //    StartCoroutine(ProduceCoroutine(type));
    //}

    //IEnumerator UpgradeCoroutine(ProductionType ptype, UpgradeType utype)
    //{
    //    currentBuildingJobState = BuildingJobState.Upgrading;

    //    timerMax = ProductionUpgradeManager.Instance.UpgradeTime(ptype, utype);
    //    while(timer < timerMax)
    //    {
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
    //    //종료시 업그레이드
    //    timer = 0f;
    //    ProductionUpgradeManager.Instance.Upgrade(ptype, utype);
    //}


    //IEnumerator ProduceCoroutine(ProductionType type)
    //{
    //    currentBuildingJobState = BuildingJobState.Producing;

    //    //유닛 instantiate는 System에서

    //    timerMax = ProductionUpgradeManager.Instance.GetProductionTime(type);
    //    while (timer < timerMax)
    //    {
    //        timer += Time.deltaTime;
    //        yield return null;
    //    }
        
    //    timer = 0;
    //}


}

public struct BaseBuilding : IComponentData
{
    public bool isUpgrading;
    public bool isSummoning;
}
public enum ProductionType
{
    Memetes,
    Kowang
}

public enum BuildingJobState
{
    Idle,
    Upgrading,
    Producing,
}

