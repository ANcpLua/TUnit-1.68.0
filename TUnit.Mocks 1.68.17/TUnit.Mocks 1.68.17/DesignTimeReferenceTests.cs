using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace TUnit.Mocks_1._68._17;

// Both tests run MSBuild on this very project, and each run rewrites the publicized VendorSdk copy in obj/.
[NotInParallel]
public class DesignTimeReferenceTests
{
    // Fixed in 1.68.17 (#6836, #6837): an editor loading this project saw VendorSdk twice — the publicized copy
    // and the live ProjectReference — and the project's own compilation won, so InternalsAccessTests was red with
    // CS0122 while `dotnet build` was clean. In design-time builds only, the targets now set
    // ReferenceOutputAssembly=false on that ProjectReference, leaving the publicized copy as the one reference.
    [Test]
    public async Task Design_time_build_detaches_the_publicized_project_reference()
    {
        var referenceOutputAssembly = await VendorSdkReferenceOutputAssemblyAsync(designTimeBuild: true);

        await Assert.That(referenceOutputAssembly).IsEqualTo("false");
    }

    // A real build must keep the reference, or VendorSdk.dll would be missing from the output and deps.json.
    [Test]
    public async Task Real_build_keeps_the_project_reference()
    {
        var referenceOutputAssembly = await VendorSdkReferenceOutputAssemblyAsync(designTimeBuild: false);

        await Assert.That(referenceOutputAssembly).IsEmpty();
    }

    private static async Task<string> VendorSdkReferenceOutputAssemblyAsync(
        bool designTimeBuild,
        [CallerFilePath] string sourceFile = "")
    {
        var project = Directory.GetFiles(Path.GetDirectoryName(sourceFile)!, "*.csproj").Single();

        var startInfo = new ProcessStartInfo("dotnet")
        {
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            // A reused MSBuild node or server keeps the publicizer task loaded after the test ends.
            Environment = { ["MSBUILDUSESERVER"] = "0" },
        };
        foreach (var argument in new[]
                 {
                     "msbuild", project, "-nologo", "-nodeReuse:false",
                     "-t:Compile", "-p:SkipCompilerExecution=true", "-p:ProvideCommandLineArgs=true",
                     "-p:BuildProjectReferences=false", $"-p:DesignTimeBuild={designTimeBuild}",
                     "-getItem:ProjectReference",
                 })
        {
            startInfo.ArgumentList.Add(argument);
        }

        using var msbuild = Process.Start(startInfo)!;
        var stdout = msbuild.StandardOutput.ReadToEndAsync();
        var stderr = msbuild.StandardError.ReadToEndAsync();
        await msbuild.WaitForExitAsync();

        await Assert.That(msbuild.ExitCode).IsEqualTo(0).Because(await stderr);

        using var json = JsonDocument.Parse(await stdout);
        var vendorSdk = json.RootElement.GetProperty("Items").GetProperty("ProjectReference").EnumerateArray()
            .Single(item => item.GetProperty("Identity").GetString()!.EndsWith("VendorSdk.csproj"));

        return vendorSdk.TryGetProperty("ReferenceOutputAssembly", out var value) ? value.GetString()! : "";
    }
}
