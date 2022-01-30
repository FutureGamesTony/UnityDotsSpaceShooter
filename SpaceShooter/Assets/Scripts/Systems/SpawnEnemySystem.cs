using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
public class SpawnEnemySystem : JobComponentSystem
{
    int spawnedShips = 0;

    int wave = 0;
    bool shouldSpawn = true;
    float3 cameraPos;
    float2 cameraExtent;
    float3 spawnpoint;

    GameManager manager = GameManager.instance;
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        manager = GameManager.instance;
        cameraPos = manager.GetCameraPosition();
        cameraExtent = manager.GetCameraExtent();
        spawnpoint = manager.GetEnemySpawnOrigin();
    }

    protected override JobHandle OnUpdate(JobHandle indepts)
    {
        if (manager.GetDisposedEnemies(spawnedShips)) 
        {
            SetShouldSpawn(true);
        }
        if (!shouldSpawn) return default;
  
        manager = GameManager.instance;
        float3 playerPos = manager.GetPlayerPosition();
        float3 spawnpointOrigin = manager.GetEnemySpawnOrigin();
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        
        Entities
            .WithoutBurst()
            .ForEach((EnemySpawnData enemySpawnData) =>
            {
                spawnpoint.x += enemySpawnData.distanceBetweenEnemies;
                if (spawnpoint.x > cameraPos.x + cameraExtent.x)
                {
                    spawnpoint.x = spawnpointOrigin.x;
                    spawnpoint.y += enemySpawnData.distanceBetweenEnemies;
                }
                enemySpawnData.originSpawnPoint = spawnpoint;

                Translation spawnTranslation = new Translation
                {
                    Value = spawnpoint
                };
                Entity spawnedShip = GameManager.instance.enemyEntityPrefab;
                entityCommandBuffer.SetComponent(spawnedShip, spawnTranslation);

                entityCommandBuffer.Instantiate(spawnedShip);
                entityCommandBuffer.AddComponent(spawnedShip, new EnemyData
                {
                    speed = manager.enemySpeed,
                });
                entityCommandBuffer.AddComponent(spawnedShip, new EnemyTag { });
                spawnedShips++;

                shouldSpawn = (enemySpawnData.shipsToSpawn > spawnedShips);
                
                if (!shouldSpawn) enemySpawnData.shipsToSpawn+=wave;
            }).Run();

        entityCommandBuffer.Playback(manager.manager);
        entityCommandBuffer.Dispose();
        return default;
    }
    public int GetSpawnedEnemies() 
    {
        return spawnedShips;
    }
    public void SetShouldSpawn(bool spawn)
    {
        shouldSpawn = spawn;
        spawnedShips = 0;
        spawnpoint = manager.enemySpawnOrigin;
        wave++;
    }
}