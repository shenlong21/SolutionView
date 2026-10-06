namespace SolutionView.Core;

public class SolutionViewItem
{
    public string SchemaVersion { get; set; } = "1.0";
    public string Generated { get; set; } = string.Empty;
    public SolutionViewSolutionItem? Solution { get; set; }
    public List<SolutionViewProjectItem> Projects { get; set; } = new List<SolutionViewProjectItem>();
    public List<SolutionViewPackageItem> Packages { get; set; } = new List<SolutionViewPackageItem>();
    public List<SolutionViewDependencyItem> Dependencies { get; set; } = new List<SolutionViewDependencyItem>();
}

public class SolutionViewSolutionItem
{
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
}

public class SolutionViewProjectItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public List<string> TargetFrameworks { get; set; } = new List<string>();
    public bool IsTestProject { get; set; }
}

public class SolutionViewPackageItem
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
}

public class SolutionViewDependencyItem
{
    public int ProjectId { get; set; }
    public List<int> ProjectDependency { get; set; } = new List<int>();
    public List<int> PackageDependency { get; set; } = new List<int>();
}
