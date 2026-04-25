public interface IConstructable
{
    float ConstructionProgress { get; } // 0 - 1
    bool  IsComplete           { get; }
    void  ApplyConstruction(float deltaTime);
}
