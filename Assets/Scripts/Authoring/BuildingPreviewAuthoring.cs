using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public class BuildingPreviewAuthoring : MonoBehaviour
{
    public int2 baseSize;
    public class Baker : Baker<BuildingPreviewAuthoring>
    {
        public override void Bake(BuildingPreviewAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BuildingPreview
            {
                baseSize = authoring.baseSize,
            });
            AddComponent(entity, new URPMaterialPropertyBaseColor());
        }
    }
}


public struct BuildingPreview : IComponentData
{
    public bool canBuildHere;
    //∫£¿ÃΩ∫ :
    public int2 baseSize;
}