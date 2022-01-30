using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Collections;
using Unity.Transforms;

[AlwaysSynchronizeSystem]
public class EnemyMovementSystem : JobComponentSystem
{

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        GameManager manager = GameManager.instance;

        float deltaTime = Time.DeltaTime;
        EntityCommandBuffer entityCommandBuffer = new EntityCommandBuffer(Allocator.TempJob);


        Entities
            .WithoutBurst()
            .WithAll<EnemyTag>()
            .ForEach((ref PhysicsVelocity vel, in EnemyData speedData) =>
            {
                vel.Linear.y = -speedData.speed * deltaTime;
                vel.Linear.zx = 0;

            }).Run();
        entityCommandBuffer.Playback(EntityManager);
        entityCommandBuffer.Dispose();
        inputDeps.Complete();

        return default;
    }


}