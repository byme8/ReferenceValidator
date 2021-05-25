using System;
using System.Collections;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;

namespace ReferenceValidator.Tests.Utils
{
    public static class TestExtensions
    {
        public static async Task<Project> ReplacePartOfDocumentAsync(this Project project, string documentName, string textToReplace, string newText)
        {
            var document = project.Documents.First(o => o.Name == documentName);
            var text = await document.GetTextAsync();
            return document
                .WithText(SourceText.From(text.ToString().Replace(textToReplace, newText)))
                .Project;
        }
      
        public static object ReflectionGetValue(this object @object, string name)
        {
            var nonPublic = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var member = @object.GetType().GetField(name, nonPublic);
            if (member is null)
            {
                return @object
                    .GetType()
                    .GetProperty(name, nonPublic)
                    ?.GetValue(@object);
            }

            return member.GetValue(@object);
        }

        public static object ReflectionCall(this object @object, string name)
        {
            var nonPublic = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var member = @object.GetType().GetField(name, nonPublic);
            if (member is null)
            {
                return @object
                    .GetType()
                    .GetProperty(name, nonPublic)
                    ?.GetValue(@object);
            }

            return member.GetValue(@object);
        }
    }
}