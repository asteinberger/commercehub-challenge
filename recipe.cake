#load nuget:?package=Cake.Recipe&version=1.0.0

Environment.SetVariableNames();

BuildParameters.SetParameters(context: Context,
                            buildSystem: BuildSystem,
                            sourceDirectoryPath: "./src",
                            testDirectoryPath: "./tests",
                            title: "net-interview",
                            repositoryOwner: "CommerceHub",
                            repositoryName: "net-interview",                            
                            shouldRunGitVersion: false,
                            nuGetSources: new string[]{
                                "https://api.nuget.org/v3/index.json",
                                Context.MakeAbsolute(Context.Directory("./src/.packages")).FullPath
                            });

BuildParameters.PrintParameters(Context);

ToolSettings.SetToolSettings(context: Context,
                            dupFinderExcludePattern: new string[] { BuildParameters.RootDirectoryPath + "/tests/**/*.cs" },
                            testCoverageFilter: "+[*]* -[xunit.*]* -[*.Tests]* -[FluentAssertions*]* -[Moq*]* ",
                            testCoverageExcludeByAttribute: "*.ExcludeFromCodeCoverage*",
                            testCoverageExcludeByFile: "*/*Designer.cs;*/*.g.cs;*/*.g.i.cs",
                            dupFinderDiscardCost: 150,
                            dupFinderThrowExceptionOnFindingDuplicates: false);

Teardown(ctx => {    
    var output = $"./BuildArtifacts/Submission_{DateTime.Now:yyyyMMdd-HHmmss}.zip";
    Information("Creating submission archive from build at '{0}'", output);
    try
    {        
        var files = GetFiles("./**/*.*", new GlobberSettings {
                Predicate = (IDirectory dir) => {
                    var directoryName = new DirectoryPath(dir.Path.FullPath).GetDirectoryName();
                    var fullPath = dir.Path.FullPath;
                    var exclusions = new[] { ".git", ".vs", "tools", "bin", "obj", "BuildArtifacts", "TestResults" };

                    if(exclusions.Any(x => directoryName.Equals(x, StringComparison.OrdinalIgnoreCase) || fullPath.IndexOf($"/{x}/", StringComparison.OrdinalIgnoreCase) > -1)) return false;

                    return true;
                }
            });

        Zip("./", output, files);   
    }
    catch (Exception)
    {
        Warning("Unable to automatically create submission archive.  Please manually package the working directory for submission.");
    }
});

Build.RunDotNetCore();
