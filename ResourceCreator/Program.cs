using Resourcecreator;
using Resourcecreator.Services.Impl;

internal class Program
{
    private static void Main(string[] args)
    {
        string? resourceFile = null;
        if (args.Length > 0) {
            resourceFile= args[0];
        }
        App.Start(new HomeFolderPersistence(), resourceFile);
    }
}