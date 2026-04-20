using Unity.Entities;
using UnityEngine;

public class CubeSpawnerAuthoring : MonoBehaviour
{
    [SerializeField] public GameObject prefab;
    [SerializeField] public float spawnRate;
}

class CuberSpawnerBaker : Baker<CubeSpawnerAuthoring>
{
    public override void Bake(CubeSpawnerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.None);

        AddComponent(entity, new CubeSpawnerComponent
        {
            prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic),
            spawnPos = authoring.transform.position,
            nextSpawnTime = 0f,
            spawnRate = authoring.spawnRate,
        });
    }
}