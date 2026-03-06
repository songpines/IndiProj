using System;
using System.Collections;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public class DoomCounter : MonoBehaviour
{
    public static DoomCounter Instance { get; private set; }
    [SerializeField] TextMeshProUGUI DoomTimer;
    private float doomTime;

    public event EventHandler AnEnd;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        doomTime = 1111f;
        StartCoroutine(doomCounter());
        AnEnd += DoomCounter_AnEnd;
    }

    private void DoomCounter_AnEnd(object sender, EventArgs e)
    {
        AnEndScenarioMundane();
    }

    IEnumerator doomCounter()
    {
        while(doomTime > 0)
        {
            doomTime -= Time.deltaTime;
            DoomTimer.text = $"Your Doom is Coming in...{((int)doomTime)}";
            yield return null;
        }
        DoomTimer.text = "An End has come.";
        AnEnd?.Invoke(this, EventArgs.Empty);
    }

    public void AnEndScenarioMundane()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<EnemySpawner>().Build(entityManager);
        NativeArray<EnemySpawner> enemySpawnerdArray = entityQuery.ToComponentDataArray<EnemySpawner>(Allocator.Temp);

        for (int i = 0; i < enemySpawnerdArray.Length; i++)
        {
            EnemySpawner en = enemySpawnerdArray[i];
            en.timerMax = 0.1f;
            en.randomWalkingDistanceMin = 30f;
            en.randomWalkingDistanceMin = 100f;
            enemySpawnerdArray[i] = en;
        }
        entityQuery.CopyFromComponentDataArray<EnemySpawner>(enemySpawnerdArray);
    }
}
