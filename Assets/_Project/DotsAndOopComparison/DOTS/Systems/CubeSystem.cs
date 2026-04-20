using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
public partial struct CubeSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;

        NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            if (entityManager.HasComponent<CubeComponent>(entity))
            {
                CubeComponent cubeComponent = entityManager.GetComponentData<CubeComponent>(entity);
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

                float3 moveDirection = cubeComponent.moveDirection * SystemAPI.Time.DeltaTime * cubeComponent.moveSpeed;

                localTransform.Position = localTransform.Position + moveDirection;
                entityManager.SetComponentData<LocalTransform>(entity, localTransform);

                if (cubeComponent.moveSpeed > 0)
                {
                    cubeComponent.moveSpeed -= 5 * SystemAPI.Time.DeltaTime;
                }
                else
                {
                    cubeComponent.moveSpeed = 0;
                }

                entityManager.SetComponentData<CubeComponent>(entity, cubeComponent);
            }
        }
    }
}
