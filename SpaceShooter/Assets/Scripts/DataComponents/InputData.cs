using Unity.Entities;

[GenerateAuthoringComponent]
public struct InputData : IComponentData
{
    public float horizontal;
    public float vertical;
}