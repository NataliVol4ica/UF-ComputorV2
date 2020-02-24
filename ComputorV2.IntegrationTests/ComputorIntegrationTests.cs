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
        [TestCase("func(x)=2", "func(X) = 2")]
        [TestCase("func(BB)=BB^2+2BB+1", "func(X) = X ^ 2 + 2 X + 1")]
        public void StartReading_WhenDeclaringFunc_PrintsResult(string cmd, string expectedOutput)
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns(cmd)
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine($"> {expectedOutput}"));
        }

        [Test]
        public void StartReading_WhenDeclaringFuncWithExistingVarNameParam_PrintsResult()
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("a=3")
                .Returns("f(a)=7")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            StringAssert.Contains("Error", _consoleOutputLines[1]);
        }

        [Test]
        [TestCase("f(c) = 2", "2")]
        [TestCase("f(c) = 2*c", "6")]
        [TestCase("f(c) = 2*c^2+2", "20")]
        public void StartReading_WhenVarDeclarationContainsFuncInRightPart_PrintsResult(string funcDeclaration,
            string expectedOutput)
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns(funcDeclaration)
                .Returns("varA = f(3)")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine($"> {expectedOutput}"));
        }

        [Test]
        public void StartReading_EvaluatingFunction_ComplexTest()
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("f(c) = c + c * c")
                .Returns("b=9")
                .Returns("a = f(0) + f(1) +   f(b ^ 3 + 1) + f(f(2))")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine("> 533674"));
        }

        [Test]
        public void StartReading_SolvingSimplestEquation_PrintsSolution(string func, string solution)
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns("x = 0 ?")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine($"> {solution}"));
        }

        [Test]
        [TestCase("x", "0")]
        public void StartReading_SolvingEquationEqualsZero_PrintsSolution(string func, string solution)
        {
            _consoleProcessor
                .SetupSequence(cp => cp.ReadLine())
                .Returns($"f(x) = {func}")
                .Returns("f(x) = 0 ?")
                .Returns("Exit");

            var testedComputor = new Computor(_consoleProcessor.Object);
            testedComputor.StartReading();

            _consoleProcessor.Verify(cp => cp.WriteLine($"> {solution}"));
        }
    }
}