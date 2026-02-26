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
            
            //자원을 전달할 baseEntity가 null이라면
            if(resourceGathering.ValueRO.baseEntity == Entity.Null)
            {
                UnityEngine.Debug.Log("기지 설정");
                //Todo basebuilding으로 변경
                //선택된 자원과 가장 가까운 base로 설정
                EntityQuery baseBuidlingQuery = state.GetEntityQuery(ComponentType.ReadOnly<Building>(), ComponentType.ReadOnly<LocalTransform>());

                //만약 아무 베이스도 없으면
                //alert처리
                if (baseBuidlingQuery.IsEmpty)
                {
                    UnityEngine.Debug.Log("베이스 없음");
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
                UnityEngine.Debug.Log("자원 가지고 가기.");
                enabledResourceDeliveringOn.ValueRW = true;
                enabledResourceGatheringOnProgress.ValueRW = false;
                enabledResourceMovingTo.ValueRW = false;
            }
            else
            {
                UnityEngine.Debug.Log("자원 가지러 가기.");
                enabledResourceMovingTo.ValueRW = true;
                enabledResourceDeliveringOn.ValueRW = false;
                enabledResourceGatheringOnProgress.ValueRW = false;
            }

            //initialization 종료
            enabledResourceGatheringInitialization.ValueRW = false;
        }
    }


}
