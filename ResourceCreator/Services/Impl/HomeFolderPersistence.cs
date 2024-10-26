using Resourcecreator.Services.Api;
using Resourcecreator.Services.Impl.HomeFolderModels;

namespace Resourcecreator.Services.Impl;

internal class HomeFolderPersistence : IPersistence
{
    private readonly string HomeFolder;
    private Profile CurrentProfile;
    public HomeFolderPersistence() {
        HomeFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        CurrentProfile = LoadProfile();
    }

    public string? CurrentlySelectedResource { 
        get => CurrentProfile.CurrentResourceFile; 
        set {
            CurrentProfile.CurrentResourceFile = value;

        }
    }

    private static Profile LoadProfile() {

        return new Profile{ };
    }

    private static void SaveProfile(Profile profile) {

    }

}
