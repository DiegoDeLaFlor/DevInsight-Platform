using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using System.Text.RegularExpressions;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Services;

public sealed class FileSystemGitHubService : IGitHubService
{
    private static readonly Regex BranchNameRegex = new("^[A-Za-z0-9._/-]+$", RegexOptions.Compiled);

    public async Task<string> CloneRepositoryAsync(string repositoryUrl, string? branch, CancellationToken cancellationToken)
    {
        var repositoryUri = new Uri(repositoryUrl);
        var targetPath = Path.Combine(Path.GetTempPath(), "devinsight", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(targetPath);

        if (!string.IsNullOrWhiteSpace(branch) && !BranchNameRegex.IsMatch(branch))
        {
            throw new InvalidOperationException("Invalid branch name format.");
        }

        var args = string.IsNullOrWhiteSpace(branch)
            ? $"clone --depth 1 {repositoryUri} \"{targetPath}\""
            : $"clone --depth 1 --branch {branch} {repositoryUri} \"{targetPath}\"";

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        process.Start();
        await process.WaitForExitAsync(cancellationToken);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync(cancellationToken);
            throw new InvalidOperationException($"Repository clone failed: {error}");
        }

        return targetPath;
    }
}
