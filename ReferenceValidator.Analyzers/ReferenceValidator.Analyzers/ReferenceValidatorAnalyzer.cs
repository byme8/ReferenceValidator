using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReferenceValidator.Analyzers
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ReferenceValidatorAnalyzer : DiagnosticAnalyzer
    {
        public const string ReferenceValidatorAnalyzerDescriptionId = "ARVF";

        public static readonly DiagnosticDescriptor ReferenceForbidDescription
            = new DiagnosticDescriptor(
                ReferenceValidatorAnalyzerDescriptionId,
                "Reference not allowed",
                "Reference to '{0}' not allowed",
                "ReferenceValidator",
                DiagnosticSeverity.Error,
                true);


        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze |
                                                   GeneratedCodeAnalysisFlags.ReportDiagnostics);

            context.RegisterSymbolAction(Execute, SymbolKind.Method);
        }

        private void Execute(SymbolAnalysisContext context)
        {
            if (context.Symbol is IMethodSymbol method)
            {
                var attribute = context.Compilation.GetTypeByMetadataName("ReferenceValidator.ForbidAttribute");
                if (!(method.GetAttributes().FirstOrDefault(o => o.AttributeClass?.Equals(attribute) ?? false) is AttributeData data))
                {
                    return;
                }

                var assemblyName = data.ConstructorArguments.FirstOrDefault().Value?.ToString();
                if (string.IsNullOrEmpty(assemblyName))
                {
                    return;
                }

                if (context.Compilation.ReferencedAssemblyNames.Any(o => o.Name == assemblyName))
                {
                    var location = data.ApplicationSyntaxReference.SyntaxTree.GetLocation(data.ApplicationSyntaxReference.Span);
                    context.ReportDiagnostic(Diagnostic.Create(ReferenceForbidDescription, location, assemblyName));
                }
            }
        }


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
            => ImmutableArray.Create(ReferenceForbidDescription);
    }
}