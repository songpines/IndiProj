using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Transforms;

//유닛이 움직이고 난 후에 업데이트
[UpdateAfter(typeof(UnitMoverSystem))]
partial struct GridUpdateSystem : ISystem
{
    private BufferLookup<OccupyingGrid> occupyingGrid;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfig>();
        state.RequireForUpdate<OccupyingGrid>();
        occupyingGrid = new BufferLookup<OccupyingGrid>();
    }


    //그리드 업데이트
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        occupyingGrid.Update(ref state);
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

        //빌딩 그리드 체크
        foreach((RefRO<Building> building, Entity entity) in SystemAPI.Query<RefRO<Building>>().WithEntityAccess())
        {
            if (building.ValueRO.hasBuilt)
            {
                if (!occupyingGrid[entity].IsEmpty)
                {
                    for (int i = 0; i < occupyingGrid[entity].Length; i++)
                    {
                        GridCellElement cell = gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(occupyingGrid[entity][i].occupyingGrid)];
                        cell.isOccupied = true;
                        gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(occupyingGrid[entity][i].occupyingGrid)] = cell;
                    }
                }
            }
            else if(!occupyingGrid[entity].IsEmpty)
            {
                for (int i = 0; i < occupyingGrid[entity].Length; i++)
                {
                    GridCellElement cell = gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(occupyingGrid[entity][i].occupyingGrid)];
                    cell.isOccupied = false;
                    gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(occupyingGrid[entity][i].occupyingGrid)] = cell;
                }
                occupyingGrid[entity].Clear();
            }
            
        }
    }
}
