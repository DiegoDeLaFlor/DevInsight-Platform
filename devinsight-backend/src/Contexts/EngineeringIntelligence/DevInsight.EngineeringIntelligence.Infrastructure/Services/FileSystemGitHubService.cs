using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using System.Text.RegularExpressions;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Services;

public sealed class FileSystemGitHubService : IGitHubService
{
    private static readonly Regex BranchNameRegex = new("^[A-Za-z0-9._/-]+$", RegexOptions.Compiled);

    public async Task<string> CloneRepositoryAsync(string repositoryUrl, string? branch, string? accessToken, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(repositoryUrl, UriKind.Absolute, out var repositoryUri))
        {
            throw new InvalidOperationException("Repository URL is invalid.");
        }

        if (!string.Equals(repositoryUri.Scheme, Uri.UriSchemeHttps, StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only HTTPS repositories are allowed.");
        }

        if (!repositoryUri.Host.EndsWith("github.com", StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidOperationException("Only github.com repositories are allowed.");
        }

        var targetPath = Path.Combine(Path.GetTempPath(), "devinsight", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(targetPath);

        if (!string.IsNullOrWhiteSpace(branch) && !BranchNameRegex.IsMatch(branch))
        {
            throw new InvalidOperationException("Invalid branch name format.");
        }

        var process = new System.Diagnostics.Process
        {
            StartInfo = new System.Diagnostics.ProcessStartInfo
            {
                FileName = "git",
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        if (!string.IsNullOrWhiteSpace(accessToken))
        {
            process.StartInfo.ArgumentList.Add("-c");
            process.StartInfo.ArgumentList.Add($"http.extraHeader=Authorization: Bearer {accessToken}");
        }

        process.StartInfo.ArgumentList.Add("clone");
        process.StartInfo.ArgumentList.Add("--depth");
        process.StartInfo.ArgumentList.Add("1");

        if (!string.IsNullOrWhiteSpace(branch))
        {
            process.StartInfo.ArgumentList.Add("--branch");
            process.StartInfo.ArgumentList.Add(branch);
        }

        process.StartInfo.ArgumentList.Add(repositoryUri.ToString());
        process.StartInfo.ArgumentList.Add(targetPath);

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
