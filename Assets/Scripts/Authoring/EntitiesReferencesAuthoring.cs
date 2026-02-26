using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject bulletPrefabGameObject;
    public GameObject enemyPrefab;

    //건물
    public GameObject skullBuilding;
    public GameObject skullBuildingPreview;

    //건물
    public GameObject wall;
    public GameObject wallPreview;

    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                bulletPrefabEntity = GetEntity(authoring.bulletPrefabGameObject, TransformUsageFlags.Dynamic),
                enemyPrefab = GetEntity(authoring.enemyPrefab, TransformUsageFlags.Dynamic),
                skullBuilding = GetEntity(authoring.skullBuilding, TransformUsageFlags.Dynamic),
                skullBuildingPreview = GetEntity(authoring.skullBuildingPreview, TransformUsageFlags.Dynamic),
                wall = GetEntity(authoring.wall, TransformUsageFlags.Dynamic),
                wallPreview = GetEntity(authoring.wallPreview, TransformUsageFlags.Dynamic),
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity bulletPrefabEntity;
    public Entity enemyPrefab;


    //건물
    public Entity skullBuilding;
    public Entity skullBuildingPreview;

    public Entity wall;
    public Entity wallPreview;

    public Entity GetBuildingPreview(BuildingType buildingType)
    {
        if(buildingType == BuildingType.BaseBuilding)
        {
            return skullBuildingPreview;
        }
        else if(buildingType == BuildingType.Wall)
        {
            return wallPreview;
        }
            return Entity.Null;
    }

    public Entity GetBuilding(BuildingType buildingType)
    {
        if (buildingType == BuildingType.BaseBuilding)
        {
            return skullBuilding;
        }
        else if (buildingType == BuildingType.Wall)
        {
            return wall;
        }
        return Entity.Null;
    }

}


public enum BuildingType
{
    BaseBuilding,
    Wall,
}
