namespace AgentsTheOdds.Data.Storage;

public static class DataRootResolver
{
    private const string SentinelFile = "AgentsTheOdds.sln";

    public static string Resolve()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            if (System.IO.File.Exists(System.IO.Path.Combine(dir.FullName, SentinelFile)))
                return System.IO.Path.Combine(dir.FullName, "data");
            dir = dir.Parent;
        }
        return System.IO.Path.Combine(AppContext.BaseDirectory, "data");
    }
}
