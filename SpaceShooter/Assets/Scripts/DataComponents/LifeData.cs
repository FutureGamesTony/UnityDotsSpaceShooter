using Unity.Entities;

[GenerateAuthoringComponent]
public class LifeData : IComponentData
{
    public bool isAlive;
    public int healthPoints;
    public int lives;
}
