using ComputorV2.ExternalResources;
using Moq;
using NUnit.Framework;

namespace ComputorV2.IntegrationTests
{
    public class QuadraticEquationIntegrationTests
    {
        private readonly Mock<IConsoleProcessor> _consoleProcessor = new Mock<IConsoleProcessor>();

        [Test]
        [TestCase("x", "0", new[] { "X = 0" })]
        [TestCase("x", "3", new[] { "X = 3" })]
        [TestCase("x^20-x^20 + x", "3", new[] { "X = 3" })]
        [TestCase("2  * x + 2 * x = 0 ", "0", new[] { "X = 0" })]
        [TestCase("x^2", "4", new[] { "X1 = 2", "X2 = -2" })]
        [TestCase("x^2 + 4*x + 4", "0", new[] { "X = -2" })]
        [TestCase("2 + ----3 * x ^2", "0", new[] { "X = -2" })]
        [TestCase("0", "1", new[] { "None" })]
        [TestCase("0*x", "1", new[] { "None" })]
        [TestCase("2 + 3 + 1", "7", new[] { "None" })]
        [TestCase("x^2", "-1", new[] { "X1 = +i", "X2 = -i" })]
        [TestCase("4 + 1 * x^2", "0", new[] { "X1 = + 2i", "X2 = - 2i" })]
        [TestCase("x^2 - 6*x + 34", "0", new[] { "X1 = 3 + 5i", "X2 = 3 - 5i" })]
        [TestCase("1 * x ^0 + 0 * x^1 + 1 * x^2", "0", new[] { "X1 = +i", "X2 = -i" })]
        [TestCase("x", "1000000000000000000000000000000000", new[] { "X = 1000000000000000000000000000000000" })]
        public void StartReading_WhenSolvingEquation_ShouldPrintsSolution(string func, string rightPart, string[] solutions)
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns($"f(x) = {func}")
                .Returns($"f(x) = {rightPart} ?")
                .Returns("Exit");
            var testedComputor = new Computor(_consoleProcessor.Object);

            //Act
            testedComputor.StartReading();

            //Assert
            foreach (var solution in solutions)
            {
                AssertWrite(solution);
            }
        }


        [Test]
        [TestCase("x + 2i", "0", new[] { "Cannot solve" })]
       
        public void StartReading_WhenSolvingEquation_ShouldPrintsSolution2(string func, string rightPart, string[] solutions)
        {
            //Arrange
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns($"f(x) = {func}")
                .Returns($"f(x) = {rightPart} ?")
                .Returns("Exit");
            var testedComputor = new Computor(_consoleProcessor.Object);

            //Act
            testedComputor.StartReading();

            //Assert
            foreach (var solution in solutions)
            {
                AssertWriteLine(solution);
            }
        }

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