using Unity.Entities;
using Unity.Jobs;
using Unity.Physics;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[AlwaysSynchronizeSystem, 
    UpdateAfter(typeof(ShotEnemySystem))]
public class PlayerMovementSystem : JobComponentSystem
{
    GameManager manager;
    float3 cameraPos;
    float2 cameraExtent;
    float playerDiameter = 0.5f;
    protected override void OnStartRunning()
    {
        base.OnStartRunning();
        manager = GameManager.instance;
        cameraPos = manager.GetCameraPosition();
        cameraExtent = manager.GetCameraExtent();
    }
    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        float deltaTime = Time.DeltaTime;
        float2 curInput = new float2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
     


        Entities
            .WithAll<PlayerData>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((ref PhysicsVelocity vel, ref Translation pos, in PlayerData playerData) =>
            {
                if (math.abs(pos.Value.x) > math.abs( cameraPos.x) + math.abs(cameraExtent.x) - playerDiameter)
                {
                    pos.Value.x = pos.Value.x > cameraPos.x ? 
                    cameraPos.x + cameraExtent.x - playerDiameter : cameraPos.x - cameraExtent.x + playerDiameter;
                    vel.Linear.xz = 0;
                }

                if (math.abs(pos.Value.y) > math.abs(cameraPos.y) + math.abs(cameraExtent.y) - playerDiameter)
                {
                    pos.Value.y = pos.Value.y > cameraPos.y ? 
                    cameraPos.y + cameraExtent.y - playerDiameter : -cameraPos.y - cameraExtent.y + playerDiameter;

                    vel.Linear.yz = 0;
                }

                float2 newVel = vel.Linear.xy;

                newVel += curInput * playerData.speed * deltaTime;
                manager.SetPlayerPosition(pos.Value);

                vel.Linear.xy = newVel;
                vel.Linear.z = 0;

            }).Run();

        return default;
    }

}
