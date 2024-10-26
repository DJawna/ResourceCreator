namespace Resourcecreator.Services.Api;

public interface IPersistence {
    string? CurrentlySelectedResource { get; set; }

    public string CurrentWorkFolder { get; set; }
}