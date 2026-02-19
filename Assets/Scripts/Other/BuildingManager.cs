using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using static BuildingManager;

public class BuildingManager : MonoBehaviour
{
    public static BuildingManager Instance { get; private set; }
    public event EventHandler OnBuildingModeEnter;
    public event EventHandler OnBuildingModeExit;

    //엔티티 관련
    EntityManager entityManager;
    EntityQuery entityQuery;
    Entity buildingPreviewEntity;

    private bool isBuidlingMode;

    //빌딩 단계
    public enum BuildingModeProgress {
        BuildingModeFalse,
        PreviewInitialization,
        PreviewSetting,
        Confirm
    }
    BuildingModeProgress buildingModeProgress;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isBuidlingMode = false;

        buildingModeProgress = BuildingModeProgress.BuildingModeFalse;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            OnBuildingModeEnter?.Invoke(this, EventArgs.Empty);
            isBuidlingMode = true;
        }

        if (isBuidlingMode)
        {
            BuildingModeHandling();
        }
    }

    public void BuildingModeHandling()
    {
        //esc누르면 취소
        if ((Input.GetKeyDown(KeyCode.Escape)))
        {
            OnBuildingModeExit?.Invoke(this, EventArgs.Empty);
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            if (entityManager.Exists(buildingPreviewEntity))
            {
                entityManager.DestroyEntity(buildingPreviewEntity);
            }
            //빌딩 모드 진행 초기화
            //TODO 바로 전 단계로
            buildingModeProgress = BuildingModeProgress.BuildingModeFalse;
            isBuidlingMode = false;
        }
        switch (buildingModeProgress)
        {
            case BuildingModeProgress.BuildingModeFalse:
                buildingModeProgress = BuildingModeProgress.PreviewInitialization;
                break;

            case BuildingModeProgress.PreviewInitialization:
                BuildingModePreviewInitialization();
                buildingModeProgress = BuildingModeProgress.PreviewSetting;
                break;

            case BuildingModeProgress.PreviewSetting:
                BuildingModelPreviewSetLocation();
                break;

            case BuildingModeProgress.Confirm:
                BuildingConfirmCheck();
                break;
        }
    }
    //프리뷰 생성
    public void BuildingModePreviewInitialization()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        EntitiesReferences entitiesReferencecs = entityQuery.GetSingleton<EntitiesReferences>();
        //유닛들 deselect
        Deselect();

        //preview 생성
        buildingPreviewEntity = entityManager.Instantiate(entitiesReferencecs.skullBuildingPreview);
        entityManager.SetComponentData<LocalTransform>(buildingPreviewEntity, new LocalTransform
        {
            Position = Vector3.zero,//MouseWorldPosition.Instance.GetPosition(),
            Rotation = Quaternion.identity,
            Scale = 10f
        });

        
        
    }

    public void BuildingModelPreviewSetLocation()
    {
        if (Input.GetMouseButtonDown(0))
        {
            
            //이동은 onbuildingmodelsystem에서 담당
            entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            bool canBuild = entityManager.GetComponentData<BuildingPreview>(buildingPreviewEntity).canBuildHere;

            //마우스 클릭시 지을 수 있는 위치면 confirm 단계로
            if (canBuild)
            {
                buildingModeProgress = BuildingModeProgress.Confirm;
            }
            
        }
    }

    public void BuildingConfirmCheck()
    {
        //프리뷰 삭제
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.DestroyEntity(buildingPreviewEntity);

        //빌딩 설치
        entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        EntitiesReferences entitiesReferencecs = entityQuery.GetSingleton<EntitiesReferences>();
        Entity buildingEntity = entityManager.Instantiate(entitiesReferencecs.skullBuilding);
        entityManager.SetComponentData<LocalTransform>(buildingEntity, new LocalTransform
        {
            Position = MouseWorldPosition.Instance.GetPosition(),
            Rotation = Quaternion.identity,
            Scale = 10f
        });

        //빌딩 모드 종료
        OnBuildingModeExit?.Invoke(this, EventArgs.Empty);
        buildingModeProgress = BuildingModeProgress.BuildingModeFalse;
        isBuidlingMode = false;
    }







    public void Deselect()
    {
        //선택된 entity들 deselect
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);

        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<Selected> selectedArray = entityQuery.ToComponentDataArray<Selected>(Allocator.Temp);

        for (int i = 0; i < entityArray.Length; i++)
        {
            entityManager.SetComponentEnabled<Selected>(entityArray[i], false);
            Selected selected = selectedArray[i];
            selected.onDeselected = true;
            entityManager.SetComponentData<Selected>(entityArray[i], selected);

        }
    }
}
