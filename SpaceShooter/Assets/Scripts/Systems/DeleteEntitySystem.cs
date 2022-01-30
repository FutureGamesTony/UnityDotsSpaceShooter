using Unity.Jobs;
using Unity.Entities;
using Unity.Collections;
[UpdateAfter(typeof(FireBulletSystem))]
public class DeleteEntitySystem : JobComponentSystem
{
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        EntityCommandBuffer commandBuffer = new EntityCommandBuffer(Allocator.TempJob);

        Entities
            .WithoutBurst()
            .WithAll<DeleteTag>()
            .ForEach((Entity entity) =>
            {
                commandBuffer.DestroyEntity(entity);
                if (GameManager.instance.manager.HasComponent<EnemyData>(entity)) GameManager.instance.EnemyKilled();
            }).Run();
        commandBuffer.Playback(EntityManager);
        commandBuffer.Dispose();
        return default;
    }

}
