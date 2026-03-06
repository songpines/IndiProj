using Unity.Entities;
using UnityEngine;

public class BaseBuildingUnitProductionAuthoring : MonoBehaviour
{
   
    public class Baker : Baker<BaseBuildingUnitProductionAuthoring>
    {
        public override void Bake(BaseBuildingUnitProductionAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BaseBuildingUnitProduction());
            SetComponentEnabled<BaseBuildingUnitProduction>(entity, false);
        }
    }
}

public struct BaseBuildingUnitProduction : IComponentData, IEnableableComponent
{
   

    //나중에 넣어주기
    public ProductionType productionType;
    public float timerMax;
    public ProductionType currentProductionType;
}
