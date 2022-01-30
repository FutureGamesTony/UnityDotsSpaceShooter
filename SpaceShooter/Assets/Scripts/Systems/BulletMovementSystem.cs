using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Collections;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class BulletMovmentSystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        GameManager manager = GameManager.instance;
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);
        Entities
            .WithoutBurst()
            .ForEach((ref PhysicsVelocity vel, in BulletSpeedData bulletSpeedData) =>
            {
                vel.Linear.y = bulletSpeedData.speed * deltaTime;
                vel.Linear.zx = 0;
            }).Run();
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        inputDeps.Complete();
        
        return default;
    }
}
