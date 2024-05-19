using ReferenceValidator.Analyzers;
using ReferenceValidator.Test.Data;
using ReferenceValidator.Tests.Utils;

namespace ReferenceValidator.Tests;

public class ReferenceValidatorAnalyzerTest
{
    [Fact]
    public async Task NotAllowedReferenceFound()
    {
        var project = TestProject.Project;

        var diagnostics = await project.ApplyAnalyzer(new ReferenceValidatorAnalyzer());
        var errors = diagnostics
            .Where(o => o.Severity == Microsoft.CodeAnalysis.DiagnosticSeverity.Error)
            .Select(o => o.GetMessage())
            .ToArray();

        await Verify(errors);
    }
}