using System.Linq;
using System.Threading.Tasks;
using ReferenceValidator.Analyzers.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ReferenceValidator.Analyzers;
using ReferenceValidator.Test.Data;

namespace ReferenceValidator.Tests
{
    [TestClass]
    public class BindingExpressionAnalyzerTest
    {
        [TestMethod]
        public async Task NotAllowedReferenceFound()
        {
            var project = TestProject.Project;

            var diagnosticts = await project.ApplyAnalyzer(new ReferenceValidatorAnalyzer());
            
            Assert.IsTrue(diagnosticts
                .Any(o => o.Id == ReferenceValidatorAnalyzer.ReferenceValidatorAnalyzerDescriptionId));
        }
    }
}