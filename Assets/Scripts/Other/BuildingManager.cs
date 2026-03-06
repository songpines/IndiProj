using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Transforms;
using Unity.VisualScripting;
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

    //
    private bool isBuidlingMode;

    private BuildingType buildingType;

    //빌딩 단계
    public enum BuildingModeProgress {
        BuildingModeFalse,
        PreviewInitialization,
        PreviewSetting,
        Confirm,
        Destroy,
    }
    BuildingModeProgress buildingModeProgress;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        isBuidlingMode = false;

        
    }
    // Update is called once per frame
    void Update()
    {
        if (isBuidlingMode)
        {
            BuildingModeHandling();
        }
    }


    //buildingmode enter
    public void EnterBuildingMode(BuildingType buildingType)
    {
        OnBuildingModeEnter?.Invoke(this, EventArgs.Empty);
        //유닛들 deselect
        Deselect();
        isBuidlingMode = true;
        this.buildingType = buildingType;
        buildingModeProgress = BuildingModeProgress.BuildingModeFalse;
    }

    public void ExitBuidlingMode()
    {
        OnBuildingModeExit?.Invoke(this, EventArgs.Empty);
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        if (entityManager.Exists(buildingPreviewEntity))
        {
            entityManager.DestroyEntity(buildingPreviewEntity);
        }
        isBuidlingMode = false;
    }

    public void EnterDeconstructionMode()
    {
        OnBuildingModeEnter?.Invoke(this, EventArgs.Empty);
        Deselect();
        isBuidlingMode = true;
        buildingModeProgress = BuildingModeProgress.Destroy;
    }

    //철거모드
    public void DeconstructionMode()
    {

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            entityQuery = entityManager.CreateEntityQuery(typeof(PhysicsWorldSingleton));
            PhysicsWorldSingleton physicsWorldSingleton = entityQuery.GetSingleton<PhysicsWorldSingleton>();
            CollisionWorld collisionWorld = physicsWorldSingleton.CollisionWorld;
            UnityEngine.Ray cameraRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastInput raycastInput = new RaycastInput
            {
                Start = cameraRay.GetPoint(0f),
                End = cameraRay.GetPoint(9999f),
                Filter = new CollisionFilter
                {
                    BelongsTo = ~0u,
                    CollidesWith = 1u << GameAssets.BUILDING_LAYER,
                    GroupIndex = 0
                }
            };
            if (collisionWorld.CastRay(raycastInput, out Unity.Physics.RaycastHit raycastHit))
            {
                //파괴는 resetsystem 업데이트에서
                //baseareaoccupying 컴포넌트 끄기
                if (entityManager.HasComponent<BaseAreaOccupying>(raycastHit.Entity)){
                    BaseAreaOccupying baseAO = entityManager.GetComponentData<BaseAreaOccupying>(raycastHit.Entity);
                    baseAO.isOccupying = false;
                    entityManager.SetComponentData(raycastHit.Entity, baseAO);
                }
            }
        }
    }

    public void BuildingModeHandling()
    {
        //마우스 우클릭 취소
        if ((Input.GetMouseButton(1)) || Input.GetKeyDown(KeyCode.Escape))
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

            case BuildingModeProgress.Destroy:
                DeconstructionMode();
                break;
        }
    }
    //프리뷰 생성
    public void BuildingModePreviewInitialization()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        EntitiesReferences entitiesReferencecs = entityQuery.GetSingleton<EntitiesReferences>();
        

        //preview 생성
        buildingPreviewEntity = entityManager.Instantiate(entitiesReferencecs.GetBuildingPreview(buildingType));
        //LocalTransform buildingPreviewTransform = entityManager.GetComponentData<LocalTransform>(buildingPreviewEntity);
        //buildingPreviewTransform.Position  =
        //{
        //    Position = Vector3.zero,//MouseWorldPosition.Instance.GetPosition(),
        //    Rotation = Quaternion.identity,
        //};
        //entityManager.SetComponentData<LocalTransform>(buildingPreviewEntity, new LocalTransform
        //{
        //    Position = Vector3.zero,//MouseWorldPosition.Instance.GetPosition(),
        //    Rotation = Quaternion.identity,
        //    Scale = 10f
        //});
    }

    public void BuildingModelPreviewSetLocation()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            if(Input.mousePosition.y > Screen.height * 0.3f)
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
           
    }

    public void BuildingConfirmCheck()
    {
        
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        entityManager.DestroyEntity(buildingPreviewEntity);

        //빌딩 설치
        entityQuery = entityManager.CreateEntityQuery(typeof(EntitiesReferences));
        EntitiesReferences entitiesReferencecs = entityQuery.GetSingleton<EntitiesReferences>();

        Building building = entityManager.GetComponentData<Building>(entitiesReferencecs.GetBuilding(buildingType));
        //프리뷰 삭제
        if(ResourceManager.Instance.Coral >= building.coralPrice && ResourceManager.Instance.Stone >= building.stonePrice)
        {
            Entity buildingEntity = entityManager.Instantiate(entitiesReferencecs.GetBuilding(buildingType));
            LocalTransform buildingLocalTransform = entityManager.GetComponentData<LocalTransform>(buildingEntity);

            entityQuery = entityManager.CreateEntityQuery(typeof(GridConfig));
            var gridConfigSingleton = entityQuery.GetSingleton<GridConfig>();
            int2 pos = gridConfigSingleton.WorldToGrid(MouseWorldPosition.Instance.GetPosition());
            float3 buildingPos = gridConfigSingleton.GridToWorld(pos);
            buildingLocalTransform.Position = buildingPos;

            entityManager.SetComponentData<LocalTransform>(buildingEntity, buildingLocalTransform);

            //빌딩 설치 true

            building.hasBuilt = true;
            entityManager.SetComponentData<Building>(buildingEntity, building);
        }
        else
        {
            //경고
            SoundManager.Instance.PlayResourceLackSound();
        }
            //빌딩 모드 종료?
            //OnBuildingModeExit?.Invoke(this, EventArgs.Empty);
            buildingModeProgress = BuildingModeProgress.PreviewInitialization;
        //isBuidlingMode = false;
    }





    public void Deselect()
    {
        //선택된 entity들 deselect
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<Selected>().Build(entityManager);
        if (!entityQuery.IsEmpty)
        {
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
}
