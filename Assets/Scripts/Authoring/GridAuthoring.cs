using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class GridAuthoring : MonoBehaviour
{
    public int Width;
    public int Height;
    public float cellSize;
    public float3 originPos;


    public class Baker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GridConfig
            {
                Width = authoring.Width,
                Height = authoring.Height,
                cellSize = authoring.cellSize,
                originPos = authoring.originPos,
            });

            DynamicBuffer<GridCellElement> buffer = AddBuffer<GridCellElement>(entity);
            buffer.ResizeUninitialized(authoring.Width * authoring.Height);
            for(int i = 0; i < buffer.Length; i++)
            {
                GridCellElement cell = buffer[i];
                cell.isOccupied = false;
                buffer[i] = cell;
            }

        }
    }
}


public struct GridConfig : IComponentData
{
    public int Width;
    public int Height;
    public float cellSize;
    public float3 originPos;

    //pos는 2d 그리드 좌표 -> 1d 배열로
    public int GetGridIndex(int2 pos) => pos.y * Width + pos.x;
    //1d -> 2d 그리드 좌표 반환
    public int2 GetIndexToGrid(int index) => new int2(index % Width, index / Width);

    //world에서 gridIndex로
    public int WorldToGridIndex(float3 worldPos)
    {
        int2 gridxy = (int2)math.floor(worldPos.xz);
        //축을 벗어나면
        if(gridxy.x < 0)
        {
            return -1;
        }
        return gridxy.y * Width + gridxy.x;
    }

    //worldTogrid 2d
    public int2 WorldToGrid(float3 pos) => (int2)math.floor(pos.xz/cellSize);

    //gridToWorld 2d
    public float3 GridToWorld(int2 gridPos) => new float3(gridPos.x * cellSize + cellSize * 0.5f, 0f, gridPos.y * cellSize + cellSize * 0.5f);
    //gridToWorld index
    public float3 GridIndexToWorld(int index) => new float3(index % Width, 0f, index / Width);

    //그리드 범위 안쪽인지 체크
    public bool IsInGrid(int2 pos) => 0 <= pos.x && pos.x < Width && pos.y >= 0 && pos.y < Height;


}

public struct GridCellElement : IBufferElementData
{
    //occupied와 겸
    public bool isWalkable;
    public bool isOccupied;

}
