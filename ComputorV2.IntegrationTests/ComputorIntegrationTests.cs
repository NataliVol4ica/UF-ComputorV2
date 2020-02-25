using System.Collections.Generic;
using ComputorV2.ExternalResources;
using Moq;
using NUnit.Framework;

namespace ComputorV2.IntegrationTests
{
    public class ComputorIntegrationTests
    {
        private readonly List<string> _consoleOutputLines = new List<string>();
        private readonly Mock<IConsoleProcessor> _consoleProcessor = new Mock<IConsoleProcessor>();
        private readonly Expression _emptyExpression = new Expression(new List<RPNToken>(), false);

        [SetUp]
        public void Setup()
        {
            _consoleProcessor
                .Setup(cp => cp.WriteLine(It.IsAny<string>()))
                .Callback<string>(str => _consoleOutputLines.Add(str));
        }

        [Test]
        public void StartReading_EvaluatingFunction_ComplexTest()
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("f(c) = c + c * c")
                .Returns("b=9")
                .Returns("a = f(0) + f(1) +   f(b ^ 3 + 1) + f(f(2))")
                .Returns("Exit");
            var testedComputor = new Computor(_consoleProcessor.Object);

            //Act
            testedComputor.StartReading();

            //Assert
            AssertWriteLine("> 533674");
        }

        [Test]
        [TestCase("func(x)=2", "func(X) = 2")]
        [TestCase("func(BB)=BB^2+2*BB+1", "func(X) = X ^ 2 + 2 * X + 1")]
        public void StartReading_WhenDeclaringFunc_PrintsResult(string cmd, string expectedOutput)
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns(cmd)
                .Returns("Exit");
            var testedComputor = new Computor(_consoleProcessor.Object);
            
            //Act
            testedComputor.StartReading();

            //Assert
            AssertWriteLine($"> {expectedOutput}");
        }

        [Test]
        public void StartReading_WhenDeclaringFuncWithExistingVarNameParam_PrintsResult()
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("a=3")
                .Returns("f(a)=7")
                .Returns("Exit");
            var testedComputor = new Computor(_consoleProcessor.Object);
            
            //Act
            testedComputor.StartReading();

            //Assert
            AssertWriteLine("Error");
        }

        [Test]
        [TestCase("f(c) = 2", "2")]
        [TestCase("f(c) = 2*c", "6")]
        [TestCase("f(c) = 2*c^2+2", "20")]
        public void StartReading_WhenVarDeclarationContainsFuncInRightPart_PrintsResult(string funcDeclaration,
            string expectedOutput)
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns(funcDeclaration)
                .Returns("varA = f(3)")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            
            //Act
            testedComputor.StartReading();

            //Assert
            AssertWriteLine($"> {expectedOutput}");
        }

        [Test]
        public void StartReading_WhenSolvingSimplestExpression_ShouldPrintsSolution()
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("x = 7 + 2")
                .Returns("x= ?")
                .Returns("Exit");

            //Act
            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            //Assert
            AssertWriteLine("> 9");
        }


        #region Solve f(x) = ? Equations

        [Test]
        public void StartReading_WhenSolvingSimplestEquation_ShouldPrintsSolution()
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("f(x) = 0")
                .Returns("F(x)= ?")
                .Returns("Exit");

            //Act
            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            //Assert
            AssertWrite($"X = 0");
        }

        [Test]
        [TestCase("x", "0")]
        public void StartReading_SolvingEquationEqualsZero_PrintsSolution(string func, string solution)
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns($"f(x) = {func}")
                .Returns("f(x) = 0 ?")
                .Returns("Exit");

            //Act
            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            //Assert
            AssertWrite($"X = {solution}");
        }

        #endregion Solve f(x) = ? Equations

        private void AssertWrite(string expected)
        {
            _consoleProcessor.Verify(cp => cp.Write(
                It.Is<string>(s => s.Contains(expected))));
        }

        private void AssertWriteLine(string expected)
        {
            _consoleProcessor.Verify(cp => cp.WriteLine(
                It.Is<string>(s => s.Contains(expected))));
        }
    }
}