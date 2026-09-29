using System.ComponentModel;

using SolutionView.Core;

using Spectre.Console;
using Spectre.Console.Cli;

namespace SolutionView.Cli;

public class CliCommandSettings : CommandSettings
{
    [CommandOption("--sln")]
    [Description("The path to the solution file")]
    public required string SolutionFilePath { get; init; } = string.Empty;

    [CommandOption("--project")]
    [Description("The project to view")]
    public string ProjectPath { get; init; } = string.Empty;
}

public class CliCommand : Command<CliCommandSettings>
{
    public override int Execute(CommandContext context, CliCommandSettings settings, CancellationToken cancellationToken)
    {
        var solutionFilePath = settings.SolutionFilePath;
        var projectPath = settings.ProjectPath;

        if (!string.IsNullOrWhiteSpace(solutionFilePath))
        {
            var scanner = new Scanner(cancellationToken);
            SolutionViewItem svi = scanner.ScanAsync(solutionFilePath, cancellationToken).GetAwaiter().GetResult();

            var render = new Render();
            render.Display(svi, isSolution: true);
        }
        else if (!string.IsNullOrWhiteSpace(projectPath))
        {
            var scanner = new Scanner(cancellationToken);
            SolutionViewItem svi = scanner.ScanAsync(projectPath, cancellationToken).GetAwaiter().GetResult();

            var render = new Render();
            render.Display(svi, isSolution: false);
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Error:[/] Please provide either a solution file path or a project path.");
            return 1;
        }

        return 0;
    }
}