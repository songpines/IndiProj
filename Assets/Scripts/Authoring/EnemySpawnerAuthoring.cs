using System.Linq.Expressions;
using Unity.Entities;
using UnityEngine;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    public GameObject enemyToSpawn;
    public int maxNumber;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
    public class Baker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemySpawner
            {
                timerMax = authoring.timerMax,
                randomWalkingDistanceMin = authoring.randomWalkingDistanceMin,
                randomWalkingDistanceMax = authoring.randomWalkingDistanceMax,
                maxNumber = authoring.maxNumber,
                enemyToSpawn = GetEntity(authoring.enemyToSpawn, TransformUsageFlags.Dynamic),
            });
        }
    }

}


public struct EnemySpawner : IComponentData, IEnableableComponent
{
    public Entity enemyToSpawn;
    public int currentNumber;
    public int maxNumber;
    public float timer;
    public float timerMax;
    public float randomWalkingDistanceMin;
    public float randomWalkingDistanceMax;
}