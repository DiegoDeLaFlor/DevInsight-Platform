using DevInsight.EngineeringIntelligence.Domain.Exceptions;

namespace DevInsight.EngineeringIntelligence.Domain.Aggregates;

public sealed class Repository
{
    private Repository(Guid id, string name, string sourceUrl, string? branch)
    {
        Id = id;
        Name = name;
        SourceUrl = sourceUrl;
        Branch = branch;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public Guid Id { get; }
    public string Name { get; }
    public string SourceUrl { get; }
    public string? Branch { get; }
    public DateTime CreatedAtUtc { get; }

    public static Repository Create(string name, string sourceUrl, string? branch)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            throw new DomainValidationException("Repository name is required.");
        }

        if (!Uri.TryCreate(sourceUrl, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp))
        {
            throw new DomainValidationException("Repository URL must be a valid absolute HTTP(S) URL.");
        }

        if (!uri.Host.EndsWith("github.com", StringComparison.OrdinalIgnoreCase))
        {
            throw new DomainValidationException("Only github.com repositories are allowed.");
        }

        return new Repository(Guid.NewGuid(), name.Trim(), sourceUrl.Trim(), string.IsNullOrWhiteSpace(branch) ? null : branch.Trim());
    }
}
