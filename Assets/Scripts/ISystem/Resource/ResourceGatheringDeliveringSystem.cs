using System.Resources;
using TMPro;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ResourceGatheringDeliveringSystem : ISystem
{
    private ComponentLookup<Building> buildingBase;
    private ComponentLookup<LocalTransform> buildingBaseLocalTransform;

    private RefRW<ResourceManage> resourceManage;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceDelivering>();
        state.RequireForUpdate<ResourceManage>();
        //readonly
        buildingBase = state.GetComponentLookup<Building>(true);
        buildingBaseLocalTransform = state.GetComponentLookup<LocalTransform>(true);

        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        buildingBase.Update(ref state);
        buildingBaseLocalTransform.Update(ref state);
        
        foreach ((EnabledRefRW<ResourceGathering> enabledResourceGathering,
            EnabledRefRW<ResourceDelivering> enabledResourceDelivering,
            EnabledRefRW<ResourceMovingTo> enabledResourceMovingTo,
            EnabledRefRW <MoveOverride> enabledMoverOverride,
            RefRW <ResourceGathering> resourceGathering,
            RefRW<MoveOverride> moveOverride,
            RefRO<LocalTransform> localTransform) in
            SystemAPI.Query< EnabledRefRW < ResourceGathering >,
            EnabledRefRW<ResourceDelivering>,
            EnabledRefRW< ResourceMovingTo >,
            EnabledRefRW<MoveOverride>,
             RefRW<ResourceGathering>,
            RefRW < MoveOverride>, RefRO<LocalTransform>>().
            WithPresent<ResourceMovingTo>().WithPresent<MoveOverride>())
        {
            if (resourceGathering.ValueRO.baseEntity == Entity.Null)
            {
                enabledResourceDelivering.ValueRW = false;
                return;
            }

            //moveoverride 실행
            if (enabledMoverOverride.ValueRO == false) enabledMoverOverride.ValueRW = true;

            float3 targetPosition = buildingBaseLocalTransform[resourceGathering.ValueRO.baseEntity].Position;
            moveOverride.ValueRW.targetPosition = targetPosition;

            //기지 반지름
            //TODO const로 GameAsset에서 세팅
            float baseRadius = buildingBase[resourceGathering.ValueRO.baseEntity].radius;

            //일정 거리 내로 들어오면
            if(math.min(math.distancesq(localTransform.ValueRO.Position.x, targetPosition.x), 
                math.distancesq(localTransform.ValueRO.Position.z, targetPosition.z)) <= (baseRadius + UnitMoverSystem.REACHED_DISTANCESQ)* (baseRadius + UnitMoverSystem.REACHED_DISTANCESQ))
            {
                //자원 업데이트
                resourceManage = SystemAPI.GetSingletonRW<ResourceManage>();
                //자원획득
                if (resourceGathering.ValueRO.resourceType == ResourceType.Stone)
                {
                    resourceManage.ValueRW.StoneResource += resourceGathering.ValueRO.resourceDeliverAmount;
                }
                else
                {
                    resourceManage.ValueRW.CoralResource += resourceGathering.ValueRO.resourceDeliverAmount;

                }
                UnityEngine.Debug.Log($"{resourceGathering.ValueRO.resourceType} 자원획득 : {resourceGathering.ValueRO.resourceDeliverAmount}");
                UnityEngine.Debug.Log(resourceManage.ValueRO.StoneResource);
                UnityEngine.Debug.Log(resourceManage.ValueRO.CoralResource);
                //자원 리셋
                resourceGathering.ValueRW.isCarrying = false;
                //다시 자원으로
                enabledResourceDelivering.ValueRW = false;
                enabledResourceMovingTo.ValueRW = true;
            }

        }
    }

    
}
