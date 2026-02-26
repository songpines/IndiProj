using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ResourceGatheringMovingToSystem : ISystem
{
    private ComponentLookup<LocalTransform> resourceLocalPosition;
    private ComponentLookup<Resource> resources;
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceMovingTo>();
        resourceLocalPosition = state.GetComponentLookup<LocalTransform>(true);
        resources = state.GetComponentLookup<Resource>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        resourceLocalPosition.Update(ref state);
        resources.Update(ref state);

        foreach ((EnabledRefRW<ResourceMovingTo> enabledResourceMovingTo,
            EnabledRefRW<MoveOverride> enabledMoveOverride,
            EnabledRefRW < ResourceGatheringOnProgress > enabledResourceGatheringOnProgress,
            RefRO <ResourceGathering> resourceGathering,
            RefRO<LocalTransform> localTransform,
            RefRW<MoveOverride> moveOverride) in
            SystemAPI.Query<EnabledRefRW<ResourceMovingTo>, EnabledRefRW<MoveOverride>, 
            EnabledRefRW<ResourceGatheringOnProgress>,RefRO<ResourceGathering>, RefRO<LocalTransform>,RefRW <MoveOverride>>().
            WithPresent<MoveOverride>().WithPresent<ResourceGatheringOnProgress>())
        {
            if (resourceGathering.ValueRO.resourceEntity == Entity.Null) return;

            //moveoverride 실행
            if (enabledMoveOverride.ValueRO == false) enabledMoveOverride.ValueRW = true;

            //경로 설정
            float3 targetPosition = resourceLocalPosition[resourceGathering.ValueRO.resourceEntity].Position;
            moveOverride.ValueRW.targetPosition = targetPosition;

            //자원 반지름
            //TODO const로 GameAsset에서 세팅
            float resourceRadius = resources[resourceGathering.ValueRO.resourceEntity].radius;

            //일정 거리 내로 들어오면
            if (math.max(math.distancesq(localTransform.ValueRO.Position.x, targetPosition.x),
                math.distancesq(localTransform.ValueRO.Position.z, targetPosition.z)) <= 
                (resourceRadius + UnitMoverSystem.REACHED_DISTANCESQ) * (resourceRadius + UnitMoverSystem.REACHED_DISTANCESQ))
            {
                UnityEngine.Debug.Log("채집시작");
                //채집시작
                moveOverride.ValueRW.targetPosition = localTransform.ValueRO.Position;
                enabledResourceGatheringOnProgress.ValueRW = true;
                enabledResourceMovingTo.ValueRW = false;
            }
        }
    }

}
