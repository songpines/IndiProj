using System;
using System.Diagnostics;
using Unity.Burst;
using Unity.Entities;

[UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
partial struct ResetEventSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach(RefRW<Selected> selected in
            SystemAPI.Query<RefRW<Selected>>().WithPresent<Selected>())
        {
            selected.ValueRW.onSelected = false;
            selected.ValueRW.onDeselected = false;
        }

        foreach(RefRW<Health> health in SystemAPI.Query<RefRW<Health>>())
        {
            health.ValueRW.onHealthChanged = false;
        }

        //그리드 리셋
        var gridBufferSingleton = SystemAPI.GetSingletonBuffer<GridCellElement>();
        for(int i = 0; i < gridBufferSingleton.Length; i++)
        {
            if(!gridBufferSingleton[i].isOccupied)
            {
                continue;
            }
            var buffer = gridBufferSingleton[i];
            //UnityEngine.Debug.Log(i);
            buffer.isOccupied = false;
            gridBufferSingleton[i] = buffer;
        }
    }

}
