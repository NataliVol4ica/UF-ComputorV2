using ComputorV2.ExternalResources;
using Moq;
using NUnit.Framework;

namespace ComputorV2.IntegrationTests
{
    internal class FunctionDeclarationAndEvaluationIntegrationTests
    {
        private readonly Mock<IConsoleProcessor> _consoleProcessor = new Mock<IConsoleProcessor>();

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
        public void StartReading_WhenEvaluatingTwoNestedFunctions_PrintsResult()
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("fa(x) = x ^ 10")
                .Returns("fb (y)  = y * 3")
                .Returns("varA = fb(fa(2))")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);

            //Act
            testedComputor.StartReading();

            //Assert
            AssertWriteLine($"> 3072");
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

        [Test]
        public void StartReading_WhenFunctionContainsComplexItem_ShouldPrintsSolution()
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

        private void AssertWriteLine(string expected)
        {
            _consoleProcessor.Verify(cp => cp.WriteLine(
                It.Is<string>(s => s.Contains(expected))));
        }//98
    }
}