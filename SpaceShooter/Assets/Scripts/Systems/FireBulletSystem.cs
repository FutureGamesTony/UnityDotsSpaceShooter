using System.Diagnostics;
using Unity.Entities;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;
using UnityEngine;
using Unity.Burst;

[AlwaysSynchronizeSystem]
public class FireBulletSystem : JobComponentSystem
{
    float3 spawnpoint;

    GameManager manager = GameManager.instance;
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        manager = GameManager.instance;
        spawnpoint = manager.GetPlayerPosition();
        
    }

    protected override JobHandle OnUpdate(JobHandle indepts)
    {
        manager = GameManager.instance;
        
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        bool fire = Input.GetKey(KeyCode.Space);

        Entities
            .WithoutBurst()
            .ForEach((ref BulletSpawnData bulletSpawnData) =>
            {
                if (fire)
                {
                    if (manager.CanFire())
                    {
                        Translation spawnTranslation = new Translation      
                        {
                            Value = manager.GetPlayerPosition()
                        };
                        manager.Fire();
                        bulletSpawnData.bullet = manager.bulletEntityPrefab;

                        entityCommandBuffer.SetComponent(bulletSpawnData.bullet, spawnTranslation);
                        entityCommandBuffer.Instantiate(manager.bulletEntityPrefab);
                        entityCommandBuffer.AddComponent(bulletSpawnData.bullet, new BulletSpeedData
                        {
                            speed = manager.bulletSpeed
                        }); 
                    }
                }
            }).Run();
        entityCommandBuffer.Playback(manager.manager);
        entityCommandBuffer.Dispose();
        return default;
    }

}