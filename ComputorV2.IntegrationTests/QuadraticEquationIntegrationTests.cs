using ComputorV2.ExternalResources;
using Moq;
using NUnit.Framework;

namespace ComputorV2.IntegrationTests
{
    public class QuadraticEquationIntegrationTests
    {
        private readonly Mock<IConsoleProcessor> _consoleProcessor = new Mock<IConsoleProcessor>();

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
            AssertWrite("X = 0");
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

        private void AssertWrite(string expected)
        {
            _consoleProcessor.Verify(cp => cp.Write(
                It.Is<string>(s => s.Contains(expected))));
        }
    }
}