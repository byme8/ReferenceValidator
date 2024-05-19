using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace ReferenceValidator.Tests.Utils
{
    public static class RoslynAnalyzerExtensions
    {
        public static async Task<ImmutableArray<Diagnostic>> ApplyAnalyzer(this Project project, DiagnosticAnalyzer analyzer)
        {
            var compilation = await project.GetCompilationAsync();
            var newCompilation = compilation!.WithAnalyzers([analyzer]);
            var diagnostics = await newCompilation.GetAllDiagnosticsAsync();

            return diagnostics;
        }
    }
}
