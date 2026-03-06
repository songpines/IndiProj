using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

partial class UISelectedPanelUpdateSystem : SystemBase
{
    UISelectedViewPanelData uISelectedViewPanelData;
    EntityQuery selectedUnitQuery;
    int queryNum;
    protected override void OnCreate()

    {
        //데이터 엔티티가 있을 때만 update
        RequireForUpdate<Selected>();
        RequireForUpdate<UISelectedViewPanelData>();

        selectedUnitQuery = GetEntityQuery(ComponentType.ReadOnly<Selected>(), ComponentType.ReadOnly<Unit>());
        queryNum = selectedUnitQuery.CalculateEntityCount();
    }


    protected override void OnUpdate()
    {
        uISelectedViewPanelData = SystemAPI.ManagedAPI.GetSingleton<UISelectedViewPanelData>();

        //선택된 유닛들 업데이트
        //선택된 상태에서만 업데이트 or 선택된 첫 프레임에만
        if (!selectedUnitQuery.IsEmpty && queryNum != selectedUnitQuery.CalculateEntityCount())
        {

            NativeArray<Unit> changedSelectedList = selectedUnitQuery.ToComponentDataArray<Unit>(Allocator.Temp);
            NativeArray<Entity> changedSelectedListEntity = selectedUnitQuery.ToEntityArray(Allocator.Temp);
            int[] changedSelectedUnitId = new int[uISelectedViewPanelData.Value.defaultUnitSelectionNumber];
            int[] changedSelectedUnitEntityIndex = new int[uISelectedViewPanelData.Value.defaultUnitSelectionNumber];
            for (int i = 0; i < changedSelectedList.Length; i++)
            {
                changedSelectedUnitId[i] = changedSelectedList[i].unitId;
                changedSelectedUnitEntityIndex[i] = changedSelectedListEntity[i].Index;
            }
            uISelectedViewPanelData.Value.SetSelectedPanel(changedSelectedUnitId);
        }

        selectedUnitQuery = GetEntityQuery(ComponentType.ReadOnly<Selected>(), ComponentType.ReadOnly<Unit>());
        queryNum = selectedUnitQuery.CalculateEntityCount();
        //selected가 없으면 clear
        if (selectedUnitQuery.IsEmpty)
        {
            if (uISelectedViewPanelData.Value.isOccupied)
            {
                uISelectedViewPanelData.Value.Clear();

            }
            
            return;
        }

        if (uISelectedViewPanelData.Value.isOccupied) return;

        //새로 선택 되었을 때
        NativeArray<Unit> selectedList = selectedUnitQuery.ToComponentDataArray<Unit>(Allocator.Temp);
        NativeArray<Entity> selectedListEntity = selectedUnitQuery.ToEntityArray(Allocator.Temp);
        int[] selectedUnitId = new int[selectedList.Length];
        int[] selectedUnitEntityIndex = new int[selectedList.Length];
        for (int i = 0; i < selectedList.Length; i++)
        {
            selectedUnitId[i] = selectedList[i].unitId;
            selectedUnitEntityIndex[i] = selectedListEntity[i].Index;
        }

        uISelectedViewPanelData.Value.SetSelectedPanel(selectedUnitId);
        

        //체력 업데이트
        //foreach ((RefRO<Selected> selected, RefRO<Health> health, Entity entity) in SystemAPI.Query<RefRO<Selected>, RefRO<Health>>().WithEntityAccess())
        //{
        //    if (health.ValueRO.onHealthChanged)
        //    {
        //        uISelectedViewPanelData.Value.
        //    }
        //}
    }
}
