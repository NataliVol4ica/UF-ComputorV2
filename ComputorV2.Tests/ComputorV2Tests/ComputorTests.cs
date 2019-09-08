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
        Mock<IVariableStorage> _variableStorage;
        Mock<IExpressionProcessor> _expressionProcessor;

        private readonly Expression _emptyExpression = new Expression(new List<RPNToken>(), false);

        [SetUp]
        public void Setup()
        {
            _consoleProcessor = new Mock<IConsoleProcessor>();
            _variableStorage = new Mock<IVariableStorage>();
            _expressionProcessor = new Mock<IExpressionProcessor>();
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
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   \t\n")]
        public void ExecuteAssignVarCommand_WhenCalledWithNullString_ThrowsArgumentNullException(string varname)
        {
            var computor = new Computor(_consoleProcessor.Object, _variableStorage.Object);

            Assert.That(() => computor.ExecuteAssignVarCommand(varname),
                Throws.TypeOf<ArgumentNullException>());
        }

        [Test]
        [TestCase("")]
        [TestCase("i")]
        [TestCase("23")]
        [TestCase("varA1")]
        public void ExecuteAssignVarCommand_WhenCalledWithInvalidVarName_ThrowsArgumentException(string varName)
        {
            var cmd = varName + " = 3";
            var computor = new Computor(_consoleProcessor.Object, _variableStorage.Object, _expressionProcessor.Object);

            Assert.That(() => computor.ExecuteAssignVarCommand(cmd),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ExecuteAssignVarCommand_CreatesExpressionWithException_WritesAnErrorToConsole()
        {
            string cpStringParameter = default;
            _expressionProcessor
                .Setup(ep => ep.CreateExpression(It.IsAny<string>(), It.IsAny<IVariableStorage>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<Exception>();
            _consoleProcessor
                .Setup(cp => cp.WriteLine(It.IsAny<string>()))
                .Callback<string>(str=>cpStringParameter=str);

            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object, _expressionProcessor.Object);
            testedComputor.ExecuteAssignVarCommand("vara = 2 / 0");

            _consoleProcessor.Verify(cp => cp.WriteLine(It.IsAny<string>()));
            StringAssert.Contains("Error", cpStringParameter);
        }

        [Test]
        public void ExecuteAssignVarCommand_WhenAllOk_CallsVariableStorageAddOrUpdateAndWritesOutput()
        {
            _expressionProcessor
                .Setup(ep => ep.CreateExpression(It.IsAny<string>(), It.IsAny<IVariableStorage>(), It.IsAny<bool>(), It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(_emptyExpression);
            _variableStorage
                .Setup(vs => vs.AddOrUpdateVariableValue(It.IsAny<string>(), It.IsAny<Expression>()))
                .Returns("AddOrUpdateReturnValue");
            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object, _expressionProcessor.Object);

            testedComputor.ExecuteAssignVarCommand("vara = 2 + 2");

            _variableStorage.Verify(vs => vs.AddOrUpdateVariableValue("vara", _emptyExpression));
            _consoleProcessor.Verify(cp => cp.WriteLine("> AddOrUpdateReturnValue"));
        }
    }
}
