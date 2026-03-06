using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class BuildingAuthoring : MonoBehaviour
{
    //가격

    public int coralPrice;
    public int stonePrice;
    public class Baker : Baker<BuildingAuthoring>
    {
        public override void Bake(BuildingAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            var collider = GetComponent<BoxCollider>();
            //xz축만
            Vector3 colliderSize = collider.bounds.size;
            float colliderSideSize = math.max(colliderSize.x, colliderSize.z);
            AddComponent(entity, new Building
            {
                radius = colliderSideSize * 0.5f,
                hasBuilt = false,
                coralPrice = authoring.coralPrice,
                stonePrice = authoring.stonePrice,
            });
            DynamicBuffer<OccupyingGrid> occupyingGrids = AddBuffer<OccupyingGrid>(entity);
            //여기 해야함
        }
    }
}


public struct Building : IComponentData
{
    //건물 반지름
    public float radius;

    //지어졌는치 철거 되었는지
    public bool hasBuilt;

    //빌딩 deconstruct시 dispose
    //public DynammicBuffer<>

    public int coralPrice;
    public int stonePrice;
}

public struct OccupyingGrid : IBufferElementData
{
    //occupied
    public int2 occupyingGrid;

}