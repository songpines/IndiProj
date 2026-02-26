using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ResourceGatheringOnProgressSystem : ISystem
{
    private ComponentLookup<Resource> resource;


    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceGatheringOnProgress>();
        resource = state.GetComponentLookup<Resource>(false);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        resource.Update(ref state);

        foreach ((EnabledRefRW<ResourceGatheringOnProgress> enabledResourceGatheringOnProgress,
            EnabledRefRW<ResourceDelivering> enabledResourceDeliveingOn,
            RefRW<ResourceGatheringOnProgress> resourceGatheringOnProgress,
            RefRW<ResourceGathering> resourceGathering,
            RefRW<UnitMover> unitMover,
            RefRO<LocalTransform> localTransform) in
            SystemAPI.Query<EnabledRefRW<ResourceGatheringOnProgress>, EnabledRefRW<ResourceDelivering>, 
            RefRW<ResourceGatheringOnProgress>, RefRW<ResourceGathering>, RefRW<UnitMover>, RefRO<LocalTransform>>().WithPresent<ResourceDelivering>())
        {
            //TODO 거리에 따른 채집 취소

            //채집 완료 시 -> DeliveringOn
            if (resourceGathering.ValueRO.isCarrying)
            {
                UnityEngine.Debug.Log("채집완료");
                enabledResourceGatheringOnProgress.ValueRW = false;
                enabledResourceDeliveingOn.ValueRW = true;

            }

            //채집 완료
            if(resourceGatheringOnProgress.ValueRO.elapsedTime >= resourceGatheringOnProgress.ValueRO.resourceGatherTime)
            {
                resourceGathering.ValueRW.isCarrying = true;
                
                Resource entityResource = resource[resourceGathering.ValueRO.resourceEntity];

                //자원 타입 설정
                resourceGathering.ValueRW.resourceType = entityResource.resourceType;

                //총량에서 빼기
                entityResource.resourceAmount -= resourceGathering.ValueRO.resourceDeliverAmount;
                resource[resourceGathering.ValueRO.resourceEntity] = entityResource;

                //시간 초기화
                resourceGatheringOnProgress.ValueRW.elapsedTime = 0f;
            }
            unitMover.ValueRW.targetPosition = localTransform.ValueRO.Position;
            resourceGatheringOnProgress.ValueRW.elapsedTime += SystemAPI.Time.DeltaTime;

        }
    }
}
