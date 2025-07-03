using Manager.Resource;

namespace Desktop.View;

public record ResourceView(string Name, string Location, IResourceIdentifier Identifier);