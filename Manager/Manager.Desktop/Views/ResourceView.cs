using Manager.Resource;

namespace Manager.Desktop.Views;

public record ResourceView(string Name, string Location, IResourceIdentifier Identifier);