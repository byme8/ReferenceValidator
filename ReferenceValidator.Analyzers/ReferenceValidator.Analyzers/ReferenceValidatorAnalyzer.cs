using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReferenceValidator.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class ReferenceValidatorAnalyzer : DiagnosticAnalyzer
{
    public const string ReferenceValidatorAnalyzerDescriptionId = "ARVF";

    public static readonly DiagnosticDescriptor ReferenceForbidDescription
        = new(
            ReferenceValidatorAnalyzerDescriptionId,
            "Reference not allowed",
            "Reference to '{0}' not allowed",
            "ReferenceValidator",
            DiagnosticSeverity.Error,
            true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
        => ImmutableArray.Create(ReferenceForbidDescription);

    public override void Initialize(AnalysisContext context)
    {
#if !DEBUG
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                                   GeneratedCodeAnalysisFlags.ReportDiagnostics);
#endif

        context.RegisterSymbolAction(ExecuteMethod, SymbolKind.Method);
        context.RegisterSyntaxNodeAction(ExecuteForClass, SyntaxKind.ClassDeclaration, SyntaxKind.RecordDeclaration);
    }

    private void ExecuteForClass(SyntaxNodeAnalysisContext context)
    {
        var symbol = GetSymbol(context);
        if (symbol is null)
        {
            return;
        }

        var specificAttribute = context.Compilation
            .GetTypeByMetadataName("ReferenceValidator.FailOnAssemblyReferenceAttribute")!;

        var diagnostics = Verify(context.Compilation, symbol, specificAttribute);
        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private void ExecuteMethod(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IMethodSymbol method)
        {
            return;
        }

        var specificAttribute = context.Compilation
            .GetTypeByMetadataName("ReferenceValidator.FailOnAssemblyReferenceAttribute")!;
        
        var diagnostics = Verify(context.Compilation, method, specificAttribute);
        foreach (var diagnostic in diagnostics)
        {
            context.ReportDiagnostic(diagnostic);
        }
    }

    private IEnumerable<Diagnostic> Verify(Compilation compilation, ISymbol symbol, INamedTypeSymbol specificAttribute)
    {
        var attributes = symbol.GetAttributes()
            .Where(o => SymbolEqualityComparer.Default.Equals(o.AttributeClass, specificAttribute))
            .Select(o => (
                AttributeData: o,
                Argument: o.ConstructorArguments
                    .Select(oo => oo.Value?.ToString())
                    .First(oo => oo != null)))
            .ToArray();

        foreach (var attribute in attributes)
        {
            if (compilation.ReferencedAssemblyNames.Any(o => o.Name == attribute.Argument))
            {
                var location = attribute.AttributeData.ApplicationSyntaxReference?.SyntaxTree
                    .GetLocation(attribute.AttributeData.ApplicationSyntaxReference.Span);

                yield return Diagnostic.Create(ReferenceForbidDescription, location, attribute.Argument);
            }
        }
    }

    private static INamedTypeSymbol? GetSymbol(SyntaxNodeAnalysisContext context)
    {
        return context.Node switch
        {
            ClassDeclarationSyntax classDeclaration => context.SemanticModel.GetDeclaredSymbol(classDeclaration),
            RecordDeclarationSyntax recordDeclaration => context.SemanticModel.GetDeclaredSymbol(recordDeclaration),
            _ => null
        };
    }
}