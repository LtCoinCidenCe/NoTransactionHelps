using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.CodeAnalysis;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
[assembly: ExcludeFromCodeCoverage]
