using Resourcecreator.Services.Api;
using Resourcecreator.Services.Impl.HomeFolderModels;

namespace Resourcecreator.Services.Impl;

internal class HomeFolderPersistence : IPersistence
{
    private readonly string HomeFolder;
    private readonly string ProfileFile;
    private Profile CurrentProfile;

    private string DefaultWorkFolder;
    public HomeFolderPersistence() {
        HomeFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        DefaultWorkFolder= HomeFolder;
        var ProfileFolder = System.IO.Path.Combine(HomeFolder, "ResourceCreator");
        Directory.CreateDirectory(ProfileFolder);
        ProfileFile = System.IO.Path.Combine(ProfileFolder, "profile.json");
        CurrentProfile = LoadProfile(ProfileFile);
    }

    public string? CurrentlySelectedResource { 
        get => CurrentProfile.CurrentResourceFile; 
        set {
            CurrentProfile.CurrentResourceFile = value;
            CurrentProfile.LastWorkFolder = System.IO.Path.GetDirectoryName(CurrentProfile.CurrentResourceFile);
            SaveProfile(CurrentProfile, ProfileFile);
        }
    }

    public string CurrentWorkFolder {
        get => CurrentProfile.LastWorkFolder ?? DefaultWorkFolder;
        set {
            CurrentProfile.LastWorkFolder = value;
            SaveProfile(CurrentProfile, ProfileFile);
        }
    }


    private static Profile LoadProfile(string profileFile) {
        var newProfile= new Profile{ };
        if (!File.Exists(profileFile)) {
            return newProfile;
        }
        string? profileJson = File.ReadAllText(profileFile);
        
        if (profileJson == null) {
            return newProfile;
        }

        Profile? result = System.Text.Json.JsonSerializer.Deserialize<Profile>(profileJson);
        if (result == null) {
            return newProfile;
        }
        return result;
    }

    private static void SaveProfile(Profile profile, string profileFile) {
        string serializedProfile= System.Text.Json.JsonSerializer.Serialize<Profile>(profile);
        File.WriteAllText(profileFile, serializedProfile);

    }

}
