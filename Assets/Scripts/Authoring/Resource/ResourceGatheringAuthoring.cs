using Unity.Entities;
using UnityEngine;

public class ResourceGatheringAuthoring : MonoBehaviour
{

    public int resourceDeliverAmount;
    public float resourceGatherTime;

    public class Baker : Baker<ResourceGatheringAuthoring>
    {
        public override void Bake(ResourceGatheringAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new ResourceGathering
            {
                resourceDeliverAmount = authoring.resourceDeliverAmount,
                isCarrying = false,
                hasInitialized = false,
            });
            AddComponent(entity, new ResourceGatheringInitialization());
            AddComponent(entity, new ResourceMovingTo());
            AddComponent(entity, new ResourceGatheringOnProgress
            {
                resourceGatherTime = authoring.resourceGatherTime,
            });
            AddComponent(entity, new ResourceDelivering());
            SetComponentEnabled<ResourceGathering>(entity, false);
            SetComponentEnabled<ResourceGatheringInitialization>(entity, false);
            SetComponentEnabled<ResourceMovingTo>(entity, false);
            SetComponentEnabled<ResourceGatheringOnProgress>(entity, false);
            SetComponentEnabled<ResourceDelivering>(entity, false);
        }
    }
}

//자원 모으기 tag
public struct ResourceGathering : IComponentData, IEnableableComponent
{
    public Entity resourceEntity;
    //자원을 가져다 줄 베이스
    public Entity baseEntity;
    public int resourceDeliverAmount;
    public bool isCarrying;

    //intialized 체크
    public bool hasInitialized;

    //옮기는 자원 데이터
    public ResourceType resourceType;

}

//자원 모으기 시작
public struct ResourceGatheringInitialization : IComponentData, IEnableableComponent
{
    
}

public struct ResourceMovingTo : IComponentData, IEnableableComponent
{

}

//자원 캐기
public struct ResourceGatheringOnProgress : IComponentData, IEnableableComponent
{
    public float resourceGatherTime;
    public float elapsedTime;
}

//귀환
public struct ResourceDelivering : IComponentData, IEnableableComponent
{

}
