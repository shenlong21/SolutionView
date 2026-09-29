namespace SolutionView.Cli;

using SolutionView.Core;

using Spectre.Console;

public class Render
{
    public void Display(SolutionViewItem svi, bool isSolution)
    {
        // Implementation for displaying the solution view
        AnsiConsole.MarkupLine($"Schema: [green]{svi.SchemaVersion}[/]");
        AnsiConsole.MarkupLine($"Generated: [green]{svi.Generated}[/]");
        AnsiConsole.MarkupLine($"Solution: [green]{svi.Solution.Name}[/]");
        AnsiConsole.MarkupLine($"Solution Path: [green]{svi.Solution.Path}[/]");
        AnsiConsole.MarkupLine($"Projects: [green]{svi.Projects.Count}[/]");
        AnsiConsole.MarkupLine($"Packages: [green]{svi.Packages.Count}[/]");
        AnsiConsole.MarkupLine($"Dependencies: [green]{svi.Dependencies.Count}[/]");

        var projectTable = new Table()
            .AddColumn("Name")
            .AddColumn("Path");

        foreach (var project in svi.Projects)
        {
            projectTable.AddRow($"{project.Id} {project.Name}", $"{project.Path}");
        }

        AnsiConsole.Write(projectTable);

        var packageTable = new Table()
            .AddColumn("Name")
            .AddColumn("Version");

        foreach (var package in svi.Packages)
        {
            packageTable.AddRow($"{package.Id} {package.Name}", $"{package.Version}");
        }

        AnsiConsole.Write(packageTable);

        var tree = new Tree("Dependencies");
        foreach (var dependency in svi.Dependencies)
        {
            var projectNode = tree.AddNode(dependency.ProjectId.ToString());
            foreach (var projectDependency in dependency.ProjectDependency)
            {
                projectNode.AddNode(projectDependency.ToString());
            }

            foreach (var packageDependency in dependency.PackageDependency)
            {
                projectNode.AddNode(packageDependency.ToString());
            }
        }
        AnsiConsole.Write(tree);

    }
}