using Unity.Entities;
using UnityEngine;

public class BaseBuildingUnitUpgradeAuthoring : MonoBehaviour
{
    public class Baker : Baker<BaseBuildingUnitUpgradeAuthoring>
    {
        public override void Bake(BaseBuildingUnitUpgradeAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BaseBuildingUnitUpgrade());
            SetComponentEnabled<BaseBuildingUnitUpgrade>(entity, false);
        }
    }
}

public struct BaseBuildingUnitUpgrade : IComponentData, IEnableableComponent
{

}
