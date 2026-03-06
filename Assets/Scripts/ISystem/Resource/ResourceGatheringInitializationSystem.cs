using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

partial struct ResourceGatheringInitializationSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceGatheringInitialization>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach((EnabledRefRW<ResourceGathering> enabledResourceGathering,
            EnabledRefRW <ResourceGatheringInitialization> enabledResourceGatheringInitialization,
            EnabledRefRW<ResourceMovingTo> enabledResourceMovingTo,
            EnabledRefRW<ResourceGatheringOnProgress> enabledResourceGatheringOnProgress,
            EnabledRefRW<ResourceDelivering> enabledResourceDeliveringOn,
            RefRW<ResourceGathering> resourceGathering,
            RefRW<ResourceGatheringInitialization> resourceGatheringInitialization) 
            in SystemAPI.Query< EnabledRefRW < ResourceGathering >,
            EnabledRefRW<ResourceGatheringInitialization>,
            EnabledRefRW<ResourceMovingTo>,
            EnabledRefRW<ResourceGatheringOnProgress>,
            EnabledRefRW<ResourceDelivering>,
                RefRW < ResourceGathering > ,
            RefRW<ResourceGatheringInitialization>>().
            WithPresent<ResourceMovingTo>().WithPresent<ResourceDelivering>().WithPresent<ResourceGatheringOnProgress>())
        {
            if (resourceGathering.ValueRO.resourceEntity == Entity.Null)
            {
                continue;
            }
            
            //자원을 전달할 baseEntity가 null이라면
            if(resourceGathering.ValueRO.baseEntity == Entity.Null)
            {

                //Todo basebuilding으로 변경
                //선택된 자원과 가장 가까운 base로 설정
                EntityQuery baseBuidlingQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Building>().WithAll<LocalTransform>().Build(ref state);

                //만약 아무 베이스도 없으면
                //alert처리
                if (baseBuidlingQuery.IsEmpty)
                {

                    enabledResourceGatheringInitialization.ValueRW = false;
                    enabledResourceGathering.ValueRW = false;
                    return;
                }

                //Todo 구획별로

                NativeArray<LocalTransform> baseBuidlingLocalTransformArray = baseBuidlingQuery.ToComponentDataArray<LocalTransform>(Allocator.Temp);
                NativeArray<Entity> baseBuidlingEntityArray = baseBuidlingQuery.ToEntityArray(Allocator.Temp);

                int closestIndex = 0;
                
                float closestDistance = math.distancesq(baseBuidlingLocalTransformArray[0].Position,
                        SystemAPI.GetComponent<LocalTransform>(resourceGathering.ValueRO.resourceEntity).Position);
                float currentDistance;
                for(int i = 0; i < baseBuidlingLocalTransformArray.Length; i++)
                {
                    currentDistance = math.distancesq(baseBuidlingLocalTransformArray[i].Position,
                        SystemAPI.GetComponent<LocalTransform>(resourceGathering.ValueRO.resourceEntity).Position);
                    if (currentDistance <= closestDistance)
                    {
                        closestDistance = currentDistance;
                        closestIndex = i;
                    }
                }
                //할당
                resourceGathering.ValueRW.baseEntity = baseBuidlingEntityArray[closestIndex];
            }

            if (resourceGathering.ValueRO.isCarrying)
            {

                enabledResourceDeliveringOn.ValueRW = true;
                enabledResourceGatheringOnProgress.ValueRW = false;
                enabledResourceMovingTo.ValueRW = false;
            }
            else
            {

                enabledResourceMovingTo.ValueRW = true;
                enabledResourceDeliveringOn.ValueRW = false;
                enabledResourceGatheringOnProgress.ValueRW = false;
            }

            //initialization 종료
            enabledResourceGatheringInitialization.ValueRW = false;
        }
    }


}
