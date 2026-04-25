public interface IGatherable
{
    ResourceType ResourceType { get; }
    bool         IsDepleted   { get; }
    int          Gather(int amount); // returns amount actually gathered
}
