using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct BaseAreaOccupyingSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<BaseAreaOccupying>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        var gridConfigSingleton = SystemAPI.GetSingleton<GridConfig>();
        var gridElementBufferSingleton = SystemAPI.GetSingletonBuffer<GridCellElement>(false);
        foreach ((EnabledRefRW<BaseAreaOccupying> enabledBaseAreaOccupying, RefRW<BaseAreaOccupying> baseAreaOccupying, RefRO<LocalTransform> localTransform)
            in SystemAPI.Query<EnabledRefRW<BaseAreaOccupying>, RefRW<BaseAreaOccupying>, RefRO<LocalTransform>>())
        {
            if (!baseAreaOccupying.ValueRO.isOccupying)
            {
                float3 occupantPosition = localTransform.ValueRO.Position;
                int2 baseSize = baseAreaOccupying.ValueRO.baseSize;
                float radius = baseAreaOccupying.ValueRO.radius;
                float cellSize = gridConfigSingleton.cellSize;

                float3 baseAreaVertexMin = new float3(occupantPosition.x - radius, 0f, occupantPosition.z - radius);

                int2 baseAreaVertexMinGrid = new int2(gridConfigSingleton.WorldToGrid(baseAreaVertexMin));

                for (int i = 0; i < baseSize.x; i++)
                {
                    for (int j = 0; j < baseSize.y; j++)
                    {

                        int2 currentGrid = new int2(baseAreaVertexMinGrid.x + i, baseAreaVertexMinGrid.y + j);
                        if (gridConfigSingleton.IsInGrid(currentGrid))
                        {
                            GridCellElement cell = gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)];
                            cell.isOccupied = true;
                            gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)] = cell;
                        }
                    }
                }
                baseAreaOccupying.ValueRW.isOccupying = true;
                enabledBaseAreaOccupying.ValueRW = false;
            }
        }
    }
}
