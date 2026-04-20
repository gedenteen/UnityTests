using Unity.Entities;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

[BurstCompile]
public partial struct CubeSystem : ISystem
{
    private double _timer;
    private double _maxTime;
    private float _direction;

    [BurstCompile]
    public void OnCreate(ref SystemState state) // инициализация здесь
    {
        _timer = 0;
        _maxTime = 5f;
        _direction = 1f;
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityManager entityManager = state.EntityManager;

        NativeArray<Entity> entities = entityManager.GetAllEntities(Allocator.Temp);

        _timer += SystemAPI.Time.DeltaTime;
        if (_timer >= _maxTime)
        {
            _direction *= -1;
            _timer -= _maxTime;
        }

        foreach (Entity entity in entities)
        {
            if (entityManager.HasComponent<CubeComponent>(entity))
            {
                CubeComponent cubeComponent = entityManager.GetComponentData<CubeComponent>(entity);
                LocalTransform localTransform = entityManager.GetComponentData<LocalTransform>(entity);

                float3 moveDirection = cubeComponent.moveDirection * SystemAPI.Time.DeltaTime * cubeComponent.moveSpeed;

                localTransform.Position = localTransform.Position + moveDirection;
                entityManager.SetComponentData<LocalTransform>(entity, localTransform);
                cubeComponent.moveSpeed = _direction * 100 * SystemAPI.Time.DeltaTime;

                entityManager.SetComponentData<CubeComponent>(entity, cubeComponent);
            }
        }
    }
}
