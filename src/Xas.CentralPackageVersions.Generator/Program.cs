// See https://aka.ms/new-console-template for more information
using Microsoft.Build.Construction;

string path = "";
var projectFiles = Directory.GetFiles(path, "*.csproj", SearchOption.AllDirectories);
Dictionary<string, ProjectItemElement> packages = new Dictionary<string, ProjectItemElement>();
foreach (string project in projectFiles)
{
    ProjectRootElement projectRootElement = ProjectRootElement.Open(project);
    foreach (ProjectItemGroupElement item in projectRootElement.ItemGroups)
    {
        foreach (ProjectItemElement element in item.Children.OfType<ProjectItemElement>())
        {
            if (element.ElementName == "PackageReference" && !packages.ContainsKey(element.Include))
            {
                packages.Add(element.Include, element);
            }
        }
    }
}
Console.WriteLine($"Found {packages.Count} packages");
ProjectRootElement props = ProjectRootElement.Create();
ProjectItemGroupElement itemGroup = props.AddItemGroup();
foreach (KeyValuePair<string, ProjectItemElement> outerKvp in packages.OrderBy(x => x.Key))
{
    ProjectMetadataElement[] temp = new ProjectMetadataElement[outerKvp.Value.Metadata.Count];
    outerKvp.Value.Metadata.CopyTo(temp, 0);
    ProjectItemElement item = itemGroup.AddItem("PackageReference", outerKvp.Key);
    foreach (ProjectMetadataElement metadata in temp)
    {
        item.AddMetadata(metadata.Name, metadata.Value, metadata.ExpressedAsAttribute);
    }
    item.Include = null;
    item.Update = outerKvp.Key;
}
props.Save(Path.Combine(path, "Packages.props"));

foreach (string project in projectFiles)
{
    ProjectRootElement projectRootElement = ProjectRootElement.Open(project);
    foreach (ProjectItemGroupElement item in projectRootElement.ItemGroups)
    {
        foreach (ProjectItemElement element in item.Children.OfType<ProjectItemElement>())
        {
            if (element.ElementName == "PackageReference")
            {
                var metadataVersion = element.Metadata.Where(x => x.Name == "Version").FirstOrDefault();
                if (metadataVersion != null)
                {
                    metadataVersion.Value = string.Empty;
                }
            }
        }
    }
    projectRootElement.Save();
}
