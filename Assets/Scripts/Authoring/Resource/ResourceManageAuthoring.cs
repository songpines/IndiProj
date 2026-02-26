using Unity.Entities;
using UnityEngine;

public class ResourceManageAuthoring : MonoBehaviour
{
    public class Baker : Baker<ResourceManageAuthoring>
    {
        public override void Bake(ResourceManageAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new ResourceManage
            {
                //initial value
                CoralResource = 0,
                StoneResource = 0
            });
        }
    }
}


public struct ResourceManage : IComponentData
{
    public int CoralResource;
    public int StoneResource;
}