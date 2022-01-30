using Unity.Entities;
using Unity.Jobs;
using Unity.Collections;
using Unity.Physics.Systems;
using Unity.Physics;

public class ShotEnemySystem : JobComponentSystem
{
    private BeginInitializationEntityCommandBufferSystem bufferSystem;
    private BuildPhysicsWorld buildPhysicsWorld;
    private StepPhysicsWorld stepPhysicsWorld;
    protected override void OnCreate()
    {
        bufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        buildPhysicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
        stepPhysicsWorld = World.GetOrCreateSystem<StepPhysicsWorld>();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        
        PrimeForDeletion triggerJob = new PrimeForDeletion
        {
            enemyEntities = GetComponentDataFromEntity<EnemyData>(),
            bulletEntities = GetComponentDataFromEntity<BulletSpeedData>(),
            entitiesToDelete = GetComponentDataFromEntity<DeleteTag>(),
            commandBuffer = bufferSystem.CreateCommandBuffer()
            
        };
        return triggerJob.Schedule(stepPhysicsWorld.Simulation, ref buildPhysicsWorld.PhysicsWorld, inputDeps);
    }
    private struct PrimeForDeletion : ITriggerEventsJob
    {

        public ComponentDataFromEntity<EnemyData> enemyEntities;
        public ComponentDataFromEntity<BulletSpeedData> bulletEntities;
        [ReadOnly] public ComponentDataFromEntity<DeleteTag> entitiesToDelete;
        public EntityCommandBuffer commandBuffer;
        public void Execute(TriggerEvent triggerEvent)
        {
            TestEntityTrigger(triggerEvent.EntityA, triggerEvent.EntityB);
            TestEntityTrigger(triggerEvent.EntityB, triggerEvent.EntityA);
        }

        private void TestEntityTrigger(Entity entityA, Entity entityB)
        {
            if (enemyEntities.HasComponent(entityA))
            {
                if (entitiesToDelete.HasComponent(entityA)) { return; }

                commandBuffer.AddComponent(entityA, new DeleteTag());
                commandBuffer.AddComponent(entityB, new DeleteTag());
            }

        }
    }
}