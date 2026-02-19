using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;

//유닛이 움직이고 난 후에 업데이트
[UpdateAfter(typeof(UnitMoverSystem))]
partial struct GridUpdateSystem : ISystem
{
    //그리드 업데이트
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridConfigSingleton = SystemAPI.GetSingleton<GridConfig>();
        var gridElementBufferSingleton = SystemAPI.GetSingletonBuffer<GridCellElement>();
        foreach(RefRO<LocalTransform> localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithPresent<Unit>())
        {
            //만약 grid 영역 밖에 있으면 일단 continue
            if (!gridConfigSingleton.IsInGrid(gridConfigSingleton.WorldToGrid(localTransform.ValueRO.Position))) continue;


            int index = gridConfigSingleton.WorldToGridIndex(localTransform.ValueRO.Position);
            var gridBuffer = gridElementBufferSingleton[index];
            //UnityEngine.Debug.Log(index);
            gridBuffer.isOccupied = true;
            gridElementBufferSingleton[index] = gridBuffer;

            //endsimulation의 resetsystem에서 reset
        }
    }
}
