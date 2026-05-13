using UnityEditor;
using UnityEngine;

public enum TestMovementType
{
    RandomNonPhysics,
    RandomPhysics,
    Manual,
}

[CreateAssetMenu(fileName = "TestFishNetSettings", menuName = "Create TestFishNetSettings SO")]
public class TestFishNetSettings : ScriptableObject
{
    public TestMovementType MovementType;
    public bool IsButtonDespawnEnabled;
}
