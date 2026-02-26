using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

partial struct ResourceGatheringSystem : ISystem
{
    //private ComponentLookup<LocalTransform> resourceLocalTransform;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<ResourceGathering>();
        //resourceLocalTransform = state.GetComponentLookup<LocalTransform>(true);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //resourceLocalTransform.Update(ref state);
        //흐름 제어

        //foreach (var (enabledResourceGathering,
        //     enabledResourceGatheringInitialization,
        //    enabledResourceMovingTo,
        //    enabledResourceGatherOnProgress,
        //    enabledResourceDeliveringOn) in
        //    SystemAPI.Query<EnabledRefRO<ResourceGathering>,
        //    EnabledRefRW<ResourceGatheringInitializationTag>,
        //    EnabledRefRO<ResourceMovingTo>,
        //    EnabledRefRO<ResourceGatheringOnProgress>,
        //    EnabledRefRO<ResourceDelivering>>().WithOptions(EntityQueryOptions.IgnoreComponentEnabledState))
        foreach (var (enabledResourceGathering, enabledResourceGatheringInitialization, enabledResourceMovingTo, enabledResourceGatherOnProgress, enabledResourceDeliveringOn, resourceGathering)
            in SystemAPI.Query<EnabledRefRW<ResourceGathering>, EnabledRefRW<ResourceGatheringInitialization>,
            EnabledRefRW<ResourceMovingTo>, EnabledRefRW<ResourceGatheringOnProgress>,
            EnabledRefRW<ResourceDelivering>, RefRW<ResourceGathering>>().WithPresent<ResourceGatheringInitialization>().
            WithPresent<ResourceMovingTo>().WithPresent<ResourceGatheringOnProgress>().
            WithPresent<ResourceDelivering>())
        {
            //initialization으로
            //if (enabledResourceMovingTo.ValueRO == false && 
            //    enabledResourceGatherOnProgress.ValueRO == false && 
            //    enabledResourceDeliveringOn.ValueRO == false)
            //{
            //Todo 차원 채취 과정 초기화

            if (resourceGathering.ValueRO.hasInitialized == false)
            {
                UnityEngine.Debug.Log("Initialization");
                enabledResourceGatheringInitialization.ValueRW = true;
                resourceGathering.ValueRW.hasInitialized = true;
            }
        }
    }

}
