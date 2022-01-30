using Unity.Entities;
using Unity.Mathematics;
[GenerateAuthoringComponent]
public class EnemySpawnData : IComponentData
{
    public Entity enemyPrefab;

    public float distanceBetweenEnemies;
    public float secondsBetweenSpawns;
    public float secondsToNextSpawn;

    public int shipsToSpawn;

    [UnityEngine.HideInInspector] public float3 originSpawnPoint;

}
