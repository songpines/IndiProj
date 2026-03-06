using Unity.Burst;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Transforms;

//유닛이 움직이고 난 후에 업데이트
[UpdateAfter(typeof(UnitMoverSystem))]
partial struct GridUpdateSystem : ISystem
{
    private BufferLookup<OccupyingGrid> occupyingGrid;
    private int lastIndex;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridConfig>();
        state.RequireForUpdate<BaseAreaOccupying>();
        //state.RequireForUpdate<OccupyingGrid>();
        //occupyingGrid = new BufferLookup<OccupyingGrid>();
    }


    //그리드 업데이트
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        //유닛
        var gridConfigSingleton = SystemAPI.GetSingleton<GridConfig>();
        var gridElementBufferSingleton = SystemAPI.GetSingletonBuffer<GridCellElement>();
        EntityCommandBuffer entityCommandBuffer =
            SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        foreach (RefRO<LocalTransform> localTransform in SystemAPI.Query<RefRO<LocalTransform>>().WithPresent<Unit>())
        {
            //만약 grid 영역 밖에 있으면 일단 continue
            if (!gridConfigSingleton.IsInGrid(gridConfigSingleton.WorldToGrid(localTransform.ValueRO.Position))) continue;

            
            int index = gridConfigSingleton.WorldToGridIndex(localTransform.ValueRO.Position);
            if(index != lastIndex)
            {
                //전 인덱스 isoccupied풀기
                var gridBuffer = gridElementBufferSingleton[lastIndex];
                gridBuffer.isOccupied = false;
                gridElementBufferSingleton[lastIndex] = gridBuffer;

                var newGridBuffer = gridElementBufferSingleton[index];
                //UnityEngine.Debug.Log(index);
                newGridBuffer.isOccupied = true;
                gridElementBufferSingleton[index] = newGridBuffer;
                lastIndex = index;
            }       
        }

        //구조물&빌딩
        //foreach ((EnabledRefRW<BaseAreaOccupying> enabledBaseAreaOccupying, RefRW<BaseAreaOccupying> baseAreaOccupying, RefRO<LocalTransform> localTransform)
        //    in SystemAPI.Query<EnabledRefRW<BaseAreaOccupying>, RefRW<BaseAreaOccupying>, RefRO<LocalTransform>>())
        //{
        //    //if (!baseAreaOccupying.ValueRO.isOccupying)
        //    //{
        //    //    float3 occupantPosition = localTransform.ValueRO.Position;
        //    //    int2 baseSize = baseAreaOccupying.ValueRO.baseSize;
        //    //    float radius = baseAreaOccupying.ValueRO.radius;
        //    //    float cellSize = gridConfigSingleton.cellSize;

        //    //    float3 baseAreaVertexMin = new float3(occupantPosition.x - radius, 0f, occupantPosition.z - radius);
        //    //    float3 baseAreaVertexMax = new float3(occupantPosition.x + radius, 0f, occupantPosition.z + radius);

        //    //    int2 baseAreaVertexMinGrid = new int2(gridConfigSingleton.WorldToGrid(baseAreaVertexMin));
        //    //    int2 baseAreaVertexMaxGrid = new int2(gridConfigSingleton.WorldToGrid(baseAreaVertexMax));

        //    //    for (int i = 0; i <= baseAreaVertexMaxGrid.x; i++)
        //    //    {
        //    //        for (int j = 0; j <= baseAreaVertexMaxGrid.y; j++)
        //    //        {

        //    //            int2 currentGrid = new int2(baseAreaVertexMinGrid.x + i, baseAreaVertexMinGrid.y + j);
        //    //            GridCellElement cell = gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)];
        //    //            cell.isOccupied = true;
        //    //            gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)] = cell;

        //    //            UnityEngine.Debug.Log(gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)].isOccupied);
        //    //        }
        //    //    }
        //    //    baseAreaOccupying.ValueRW.isOccupying = true;
        //    //    enabledBaseAreaOccupying.ValueRW = false;
        //    //}
           
        //}

        //isoccupy false 후 파괴
        foreach ((EnabledRefRW<BaseAreaOccupying> enabledBaseAreaOccupying, RefRW<BaseAreaOccupying> baseAreaOccupying, RefRO<LocalTransform> localTransform, Entity entity)
            in SystemAPI.Query<EnabledRefRW<BaseAreaOccupying>, RefRW<BaseAreaOccupying>, RefRO<LocalTransform>>().WithDisabled<BaseAreaOccupying>().WithEntityAccess())
        {
            if (!baseAreaOccupying.ValueRO.isOccupying)
            {
                float3 occupantPosition = localTransform.ValueRO.Position;
                int2 baseSize = baseAreaOccupying.ValueRO.baseSize;
                float radius = baseAreaOccupying.ValueRO.radius;
                float cellSize = gridConfigSingleton.cellSize;

                float3 baseAreaVertexMin = new float3(occupantPosition.x - radius, 0f, occupantPosition.z - radius);
                float3 baseAreaVertexMax = new float3(occupantPosition.x + radius, 0f, occupantPosition.z + radius);

                int2 baseAreaVertexMinGrid = new int2(gridConfigSingleton.WorldToGrid(baseAreaVertexMin));
                int2 baseAreaVertexMaxGrid = new int2(gridConfigSingleton.WorldToGrid(baseAreaVertexMax));

                for (int i = 0; i <= baseAreaVertexMaxGrid.x; i++)
                {
                    for (int j = 0; j <= baseAreaVertexMaxGrid.y; j++)
                    {

                        int2 currentGrid = new int2(baseAreaVertexMinGrid.x + i, baseAreaVertexMinGrid.y + j);
                        if (gridConfigSingleton.IsInGrid(currentGrid))
                        {
                            GridCellElement cell = gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)];
                            cell.isOccupied = false;
                            gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)] = cell;

                            //UnityEngine.Debug.Log(gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)].isOccupied);
                        }
                        
                    }
                }
                //여기서 파괴
                entityCommandBuffer.DestroyEntity(entity);
            }
            
        }
    }
}
