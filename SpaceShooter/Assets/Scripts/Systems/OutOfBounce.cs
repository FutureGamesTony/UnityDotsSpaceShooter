using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Collections;

[AlwaysSynchronizeSystem, UpdateAfter(typeof(SpawnEnemySystem))]
public class OutOfBounce : JobComponentSystem
{
    GameManager manager;

    float2 cameraExtent;
    float2 cameraOrigin;

    float2 enemyOrigin;
    float2 offset;
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        manager = GameManager.instance;

        cameraOrigin = manager.GetCameraPosition().xy;
        cameraExtent = manager.GetCameraExtent();
        
        enemyOrigin = manager.GetEnemySpawnOrigin().xy;
        offset = new float2(10, 10);
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithoutBurst()
            .ForEach((ref Entity entity, in Translation pos) =>
            {
                if (math.abs(pos.Value.y - cameraOrigin.y) > math.abs(cameraExtent.y + enemyOrigin.y) + offset.y) 
                {
                    commandBuffer.AddComponent(entity, new DeleteTag());
                }
        }).Run();
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();

        return default;
    }
}