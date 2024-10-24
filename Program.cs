using Resourcecreator;
using Resourcecreator.Services.Impl;

internal class Program
{
    private static void Main(string[] args)
    {

        App.Start(new HomeFolderPersistence());
    }
}