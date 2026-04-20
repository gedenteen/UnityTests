using Unity.Entities;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Transforms;

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
        _timer += SystemAPI.Time.DeltaTime;
        if (_timer >= _maxTime)
        {
            _direction *= -1;
            _timer -= _maxTime;
        }

        // Вычисляем скорость один раз на главном потоке
        float currentSpeed = _direction * SystemAPI.Time.DeltaTime * 300f;
        float deltaTime = SystemAPI.Time.DeltaTime;

        // Job автоматически распараллеливается по всем сущностям с нужными компонентами
        new MoveCubeJob
        {
            CurrentSpeed = currentSpeed,
            DeltaTime = deltaTime
        }.ScheduleParallel(); // <-- параллельное выполнение на worker threads
    }
}

[BurstCompile]
public partial struct MoveCubeJob : IJobEntity
{
    public float CurrentSpeed;
    public float DeltaTime;

    // ECS автоматически делает query: только сущности с CubeComponent + LocalTransform
    public void Execute(ref LocalTransform transform, ref CubeComponent cube)
    {
        // Обновляем скорость
        cube.moveSpeed = CurrentSpeed;

        // Двигаем сущность
        float3 moveDirection = cube.moveDirection * DeltaTime * cube.moveSpeed;
        transform.Position += moveDirection;
    }
}