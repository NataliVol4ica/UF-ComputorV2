using ComputorV2.ExternalConnections;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace ComputorV2.Tests.ComputorV2Tests
{
    public class ComputorTests
    {
        Mock<IConsoleProcessor> _consoleProcessor;
        Mock<VariableStorage> _variableStorage;

        [SetUp]
        public void Setup()
        {
            _consoleProcessor = new Mock<IConsoleProcessor>();
            _variableStorage = new Mock<VariableStorage>();
        }

        [Test]
        public void ExecuteVarsCommand_WhenCalled_PrintsVarsData()
        {
            var variablesString = "a = 2\nb = 3";

            _variableStorage.Setup(vs => vs.GetVariablesString()).Returns(variablesString);
            var computor = new Computor(_consoleProcessor.Object, _variableStorage.Object);
            computor.ExecuteVarsCommand();

            _consoleProcessor.Verify(cp => cp.WriteLine(variablesString));

        }
    }
}
