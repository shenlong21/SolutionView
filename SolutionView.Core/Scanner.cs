using Buildalyzer;
using Buildalyzer.IO;
using Buildalyzer.Environment;
using Buildalyzer.Construction;

namespace SolutionView.Core;

public class Scanner
{
    private readonly CancellationToken _cancellationToken;

    public Scanner(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;
    }

    public async Task<SolutionViewItem> ScanAsync(string solutionPath, CancellationToken cancellationToken = default)
    {
        var iOPath = new IOPath();

        SolutionViewItem svi = new SolutionViewItem();
        svi.Generated = DateTime.UtcNow.ToString("O");


        AnalyzerManager manager = new AnalyzerManager(iOPath.Combine(solutionPath));

        svi.Solution.Name = Path.GetFileName(manager.SolutionFilePath);
        svi.Solution.Path = manager.SolutionFilePath;
        // svi.Projects.

        var orderedProjects = manager.Projects.OrderBy(p => p.Key).ToList();
        var projectPackages = manager.Projects.Select(p => p.Value.ProjectFile.PackageReferences.Select(p => new { p.Name, p.Version })).ToList();
        var flattenedOrderedPackages = projectPackages.SelectMany(p => p).ToList();
        var uniqueOrderedPackages = flattenedOrderedPackages.DistinctBy(p => new { p.Name, p.Version }).ToList();
        var orderedPackages = uniqueOrderedPackages.OrderBy(p => p.Name).ThenBy(p => p.Version).ToList();

        for (var i = 1; i <= orderedPackages.Count; i++)
        {
            var package = orderedPackages.ElementAt(i - 1);

            SolutionViewPackageItem svp = new()
            {
                Id = i,
                Name = package.Name,
                Version = package.Version,
            };

            svi.Packages.Add(svp);
        }

        for (var i = 1; i <= orderedProjects.Count; i++)
        {
            var project = orderedProjects.ElementAt(i - 1);

            SolutionViewProjectItem svp = new()
            {
                Id = i,
                Name = Path.GetFileName(project.Key),
                Path = project.Value.ProjectFile.Path,
                TargetFrameworks = [.. project.Value.ProjectFile.TargetFrameworks],
                IsTestProject = Path.GetFileName(project.Key).Contains("Test"),
            };

            svi.Projects.Add(svp);
        }

        // dependencies now
        for (var i = 1; i <= orderedProjects.Count; i++)
        {
            var project = orderedProjects.ElementAt(i - 1);

            var svdi = new SolutionViewDependencyItem
            {
                ProjectId = i,
                ProjectDependency = [],
                PackageDependency = [],
            };

                IProjectAnalyzer projectAnalyser = manager.GetProject(project.Key);
                IAnalyzerResults analyserResult = projectAnalyser.Build();

                if (analyserResult.OverallSuccess)
                {
                    foreach (var result in analyserResult)
                    {
                        result.ProjectReferences.ToList().ForEach(r =>
                        {
                            int pid = svi.Projects.FirstOrDefault(op => op.Path == r.ToString())?.Id ?? 0;
                            if (pid != 0) svdi.ProjectDependency.Add(pid);
                        });
                    }
                }
                else {
                    throw new Exception("Build failed: " + project.Key + " " + analyserResult.OverallSuccess);
                }
            

            foreach (var packageReference in project.Value.ProjectFile.PackageReferences)
            {
                bool isExists = svi.Packages.Any(p => p.Name == packageReference.Name && p.Version == packageReference.Version);
                if (!isExists)
                {
                    throw new Exception($"Package {packageReference.Name} {packageReference.Version} not found");
                }

                var packageId = svi.Packages.First(p => p.Name == packageReference.Name && p.Version == packageReference.Version).Id;
                svdi.PackageDependency.Add(packageId);
            }

            svi.Dependencies.Add(svdi);
        }


        return svi;
        // IProjectAnalyzer projects = manager.GetProject();

        // foreach (var project in projects)
        // {
        //     var analyzer = manager.GetAnalyzer(project);
        //     var result = await analyzer.AnalyzeAsync(_cancellationToken);
        //     Console.WriteLine(result);
        // }
    }
}
