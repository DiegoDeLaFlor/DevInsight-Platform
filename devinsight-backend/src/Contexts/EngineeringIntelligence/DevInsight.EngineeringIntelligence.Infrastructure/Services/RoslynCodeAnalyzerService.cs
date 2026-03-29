using DevInsight.EngineeringIntelligence.Application.Contracts;
using DevInsight.EngineeringIntelligence.Application.OutboundServices;
using DevInsight.EngineeringIntelligence.Domain.Entities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DevInsight.EngineeringIntelligence.Infrastructure.Services;

public sealed class RoslynCodeAnalyzerService : ICodeAnalyzerService
{
    private const int LongMethodThreshold = 50;
    private const int DeepNestingThreshold = 3;
    private const int LargeFileThreshold = 300;

    public async Task<IReadOnlyCollection<CodeAnalysisFileResult>> AnalyzeAsync(string repositoryPath, CancellationToken cancellationToken)
    {
        var results = new List<CodeAnalysisFileResult>();
        var files = Directory.EnumerateFiles(repositoryPath, "*.cs", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var source = await File.ReadAllTextAsync(file, cancellationToken);
            var lines = source.Split('\n').Length;
            var issues = new List<CodeIssue>();

            if (lines > LargeFileThreshold)
            {
                issues.Add(new CodeIssue(
                    "LargeFile",
                    1,
                    "high",
                    $"File has {lines} lines and exceeds threshold {LargeFileThreshold}."));
            }

            var syntaxTree = CSharpSyntaxTree.ParseText(source, cancellationToken: cancellationToken);
            var root = await syntaxTree.GetRootAsync(cancellationToken);

            var methods = root.DescendantNodes().OfType<MethodDeclarationSyntax>();
            foreach (var method in methods)
            {
                var lineSpan = method.GetLocation().GetLineSpan();
                var methodLineCount = lineSpan.EndLinePosition.Line - lineSpan.StartLinePosition.Line + 1;
                var methodStartLine = lineSpan.StartLinePosition.Line + 1;

                if (methodLineCount > LongMethodThreshold)
                {
                    issues.Add(new CodeIssue(
                        "LongMethod",
                        methodStartLine,
                        "medium",
                        $"Method has {methodLineCount} lines and exceeds threshold {LongMethodThreshold}.",
                        method.Identifier.ValueText));
                }

                var maxDepth = CalculateMaxNesting(method);
                if (maxDepth > DeepNestingThreshold)
                {
                    issues.Add(new CodeIssue(
                        "DeepNesting",
                        methodStartLine,
                        "medium",
                        $"Method nesting depth is {maxDepth} and exceeds threshold {DeepNestingThreshold}.",
                        method.Identifier.ValueText));
                }
            }

            if (issues.Count > 0)
            {
                results.Add(new CodeAnalysisFileResult(file, issues));
            }
        }

        return results;
    }

    private static int CalculateMaxNesting(MethodDeclarationSyntax method)
    {
        if (method.Body is null)
        {
            return 0;
        }

        var walker = new NestingDepthWalker();
        walker.Visit(method.Body);
        return walker.MaxDepth;
    }

    private sealed class NestingDepthWalker : CSharpSyntaxWalker
    {
        private int _currentDepth;
        public int MaxDepth { get; private set; }

        public override void VisitIfStatement(IfStatementSyntax node)
        {
            VisitNested(node, base.VisitIfStatement);
        }

        public override void VisitForStatement(ForStatementSyntax node)
        {
            VisitNested(node, base.VisitForStatement);
        }

        public override void VisitForEachStatement(ForEachStatementSyntax node)
        {
            VisitNested(node, base.VisitForEachStatement);
        }

        public override void VisitWhileStatement(WhileStatementSyntax node)
        {
            VisitNested(node, base.VisitWhileStatement);
        }

        public override void VisitSwitchStatement(SwitchStatementSyntax node)
        {
            VisitNested(node, base.VisitSwitchStatement);
        }

        public override void VisitTryStatement(TryStatementSyntax node)
        {
            VisitNested(node, base.VisitTryStatement);
        }

        private void VisitNested<TNode>(TNode node, Action<TNode> visit)
            where TNode : SyntaxNode
        {
            _currentDepth++;
            MaxDepth = Math.Max(MaxDepth, _currentDepth);
            visit(node);
            _currentDepth--;
        }
    }
}
