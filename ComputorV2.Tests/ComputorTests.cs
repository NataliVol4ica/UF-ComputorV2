using System;
using System.Collections.Generic;
using ComputorV2.ExternalResources;
using Moq;
using NUnit.Framework;

namespace ComputorV2.UnitTests
{
    public class ComputorTests
    {
        private readonly List<string> _consoleOutputLines = new List<string>();
        private readonly Mock<IConsoleProcessor> _consoleProcessor = new Mock<IConsoleProcessor>();
        private readonly Expression _emptyExpression = new Expression(new List<RPNToken>(), false);
        private readonly Mock<IExpressionProcessor> _expressionProcessor = new Mock<IExpressionProcessor>();
        private readonly Mock<IVariableStorage> _variableStorage = new Mock<IVariableStorage>();

        [SetUp]
        public void Setup()
        {
            _consoleProcessor
                .Setup(cp => cp.WriteLine(It.IsAny<string>()))
                .Callback<string>(str => _consoleOutputLines.Add(str));
        }

        [Test]
        public void ExecuteVarsCommand_WhenCalled_PrintsVarsData()
        {
            //Arrange
            var variablesString = "a = 2\nb = 3";
            _variableStorage.Setup(vs => vs.GetVariablesString()).Returns(variablesString);
            SetupConsoleMockToReturnCommandAndExit("Vars");
            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object,
                _expressionProcessor.Object);
            //Act
            testedComputor.StartReading();
            //Assert
            _consoleProcessor.Verify(cp => cp.WriteLine(variablesString));
        }

        private void SetupConsoleMockToReturnCommandAndExit(string command)
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns(command)
                .Returns("Exit");
        }

        #region Assign Var

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   \t\n")]
        public void ExecuteAssignVarCommand_WhenCalledWithNullString_WritesAnErrorToConsole(string varName)
        {
            SetupConsoleMockToReturnCommandAndExit(varName);
            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object,
                _expressionProcessor.Object);

            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine(It.IsAny<string>()));
            StringAssert.Contains("Error", _consoleOutputLines[0]);
        }

        [Test]
        [TestCase("")]
        [TestCase("i")]
        [TestCase("23")]
        [TestCase("varA1")]
        public void ExecuteAssignVarCommand_WhenCalledWithInvalidVarName_WritesAnErrorToConsole(string varName)
        {
            var cmd = varName + " = 3";
            SetupConsoleMockToReturnCommandAndExit(cmd);
            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object,
                _expressionProcessor.Object);

            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine(It.IsAny<string>()));
            StringAssert.Contains("Error", _consoleOutputLines[0]);
        }

        [Test]
        public void ExecuteAssignVarCommand_CreatesExpressionWithException_WritesAnErrorToConsole()
        {
            _expressionProcessor
                .Setup(ep => ep.CreateExpression(It.IsAny<string>(), It.IsAny<bool>()))
                .Throws<Exception>();

            SetupConsoleMockToReturnCommandAndExit("vara = 2 / 0");
            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object,
                _expressionProcessor.Object);
            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine(It.IsAny<string>()));
            StringAssert.Contains("Error", _consoleOutputLines[0]);
        }

        [Test]
        public void ExecuteAssignVarCommand_WhenAllOk_CallsVariableStorageAddOrUpdateAndWritesOutput()
        {
            //Arrange
            _expressionProcessor
                .Setup(ep => ep.CreateExpression(It.IsAny<string>(), It.IsAny<bool>()))
                .Returns(_emptyExpression);
            _variableStorage
                .Setup(vs => vs.AddOrUpdateVariableValue(It.IsAny<string>(), It.IsAny<Expression>()))
                .Returns("AddOrUpdateReturnValue");
            SetupConsoleMockToReturnCommandAndExit("vara = 2 + 2");
            var testedComputor = new Computor(_consoleProcessor.Object, _variableStorage.Object,
                _expressionProcessor.Object);
            
            //Act
            testedComputor.StartReading();

            //Assert
            _variableStorage.Verify(vs => vs.AddOrUpdateVariableValue("vara", _emptyExpression));
            _consoleProcessor.Verify(cp => cp.WriteLine("> AddOrUpdateReturnValue"));
        }

        #endregion

        #region Functions

        #endregion
    }
}