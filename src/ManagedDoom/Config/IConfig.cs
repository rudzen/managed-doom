namespace ManagedDoom.Config;

public interface IConfig
{
    ConfigValues Values { get; }
    bool IsRestoredFromFile { get; }
    void Save(string path);
}