using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct MoveOverrideSystem : ISystem
{
    //[BurstCompile]
    //public void OnCreate(ref SystemState state)
    //{
    //    foreach((RefRO<LocalTransform> localTransform, RefRW<MoveOverride> moveOverride) in SystemAPI.Query<RefRO<LocalTransform>, RefRW<MoveOverride>>())
    //    {
    //        moveOverride.ValueRW.initialPosition = localTransform.ValueRO.Position;
    //        moveOverride.ValueRW.targetPosition = moveOverride.ValueRO.initialPosition;
    //    }
    //}

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<MoveOverride>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridConfig = SystemAPI.GetSingleton<GridConfig>();
        foreach ((RefRO<LocalTransform> localTransform, RefRW<UnitMover> unitMover, RefRW<MoveOverride> moveOverride, EnabledRefRW<MoveOverride> moveOverrideEnabled) in 
            SystemAPI.Query<RefRO<LocalTransform>, RefRW<UnitMover>, RefRW<MoveOverride>, EnabledRefRW<MoveOverride>>())
        {
            if (!gridConfig.IsInGrid(gridConfig.WorldToGrid(moveOverride.ValueRO.targetPosition)))
            {
                moveOverride.ValueRW.targetPosition = localTransform.ValueRO.Position;
            }

            
            if(math.distancesq(localTransform.ValueRO.Position, moveOverride.ValueRO.targetPosition) > UnitMoverSystem.REACHED_DISTANCESQ)
            {
                //µµÂø ¸øÇÔ
                unitMover.ValueRW.targetPosition = moveOverride.ValueRO.targetPosition;
            }
            else
            {
                //µµÂø, moveoverride false
                moveOverrideEnabled.ValueRW = false;
            }
        }
    }

}
