using JetBrains.Annotations;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Entities.UniversalDelegates;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.VisualScripting;

//그리드 업데이트 후 실행
[UpdateAfter(typeof(GridUpdateSystem))] 
partial struct OnBuildingModePreviewSetSystem : ISystem
{

    private BufferLookup<OccupyingGrid> occupyingGrid;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        //preview 생성되었는지 체크
        state.RequireForUpdate<BuildingPreview>();
       // state.RequireForUpdate<OccupyingGrid>();
        //occupyingGrid = new BufferLookup<OccupyingGrid>();
    }


    public void OnUpdate(ref SystemState state) 
    {
        //occupyingGrid.Update(ref state);
        var gridConfigSingleton = SystemAPI.GetSingleton<GridConfig>();
        var gridElementBufferSingleton = SystemAPI.GetSingletonBuffer<GridCellElement>(true);
        foreach ((RefRW<LocalTransform> localTransform, RefRW<URPMaterialPropertyBaseColor> urp, RefRW < BuildingPreview> buildingPreview, Entity entity) in 
            SystemAPI.Query<RefRW<LocalTransform>, RefRW<URPMaterialPropertyBaseColor>, RefRW <BuildingPreview>>().WithEntityAccess())
        {
            //프리뷰 모델이 마우스 위치를 따라다니고 건설 가능한지 판별하는 system
            //TODO mousePosition systembase로
            int2 gridPosition = gridConfigSingleton.WorldToGrid(MouseWorldPosition.Instance.GetPosition());
            //UnityEngine.Debug.Log(gridPosition);
            float3 modelPreviewPosition = gridConfigSingleton.GridToWorld(gridPosition);


            //베이스 축에 따라 위치 보정
            float2 offset = new float2(0f,0f);
            if (buildingPreview.ValueRO.baseSize.x % 2 == 0)
            {
                offset.x += gridConfigSingleton.cellSize / 2f;
            }
            if (buildingPreview.ValueRO.baseSize.y % 2 == 0)
            {
                offset.y += gridConfigSingleton.cellSize / 2f;
            }
            modelPreviewPosition += new float3(offset.x,0,offset.y);
            localTransform.ValueRW.Position = modelPreviewPosition;

            
            //좌표가 그리드 밖에 있으면 불가
            int gridIndex = gridConfigSingleton.WorldToGridIndex(localTransform.ValueRO.Position);
            if (gridIndex < 0)
            {
                urp.ValueRW.Value = new float4(1f, 0f, 0f, 0.3f);
                buildingPreview.ValueRW.canBuildHere = false;
                return;
            }

            // 해당 그리드의 베이스가 isOccuppied가 아닐 시에만 건설 가능
            bool canBuild = true;
            int2 baseSize = buildingPreview.ValueRO.baseSize;
            float cellSize = gridConfigSingleton.cellSize;

            //베이스가 차지하는 정점
            //왼쪽 아래
            //baseSize.y축 = modelPreviewPosition.z축
            float3 baseAreaVertexMin= new float3(modelPreviewPosition.x - (baseSize.x * cellSize/2), 0f, modelPreviewPosition.z - (baseSize.y * cellSize/2));

            int2 baseAreaVertexMinGrid = new int2(gridConfigSingleton.WorldToGrid(baseAreaVertexMin));

            //if (!occupyingGrid[entity].IsEmpty)
            //{
            //    occupyingGrid[entity].Clear();
            //}
            //정점들을 그리드 좌표로 변환하여 isoccupied 체크
            for (int i = 0; i < baseSize.x; i++)
            {
                for (int j = 0; j < baseSize.y; j++)
                {
                    
                    int2 currentGrid = new int2(baseAreaVertexMinGrid.x + i, baseAreaVertexMinGrid.y + j);
                    //occupyingGrid[entity].Add(new OccupyingGrid { occupyingGrid = currentGrid });
                    if (!gridConfigSingleton.IsInGrid(currentGrid))
                    {

                        canBuild = false;
                        break;
                    }
                    if (gridElementBufferSingleton[gridConfigSingleton.GetGridIndex(currentGrid)].isOccupied)
                    {

                        canBuild = false;
                        break;
                    }
                }
            }


            if (canBuild)
            {
                urp.ValueRW.Value = new float4(0f, 1f, 0f, 0.3f);
                buildingPreview.ValueRW.canBuildHere = true;
            }
            else
            {
                urp.ValueRW.Value = new float4(1f, 0f, 0f, 0.3f);
                buildingPreview.ValueRW.canBuildHere = false;
                //if (!occupyingGrid[entity].IsEmpty)
                //{
                //    occupyingGrid[entity].Clear();
                //}
            }
            
        }
    }

}
