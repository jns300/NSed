using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArgumentHelper.Arguments.General;
using ArgumentHelper.Arguments.General.Attributes;
using System.Collections.Generic;
using System.Reflection;
using ArgumentHelper.Arguments.General.Logging;
using ArgumentHelper.Arguments;

namespace ArgumentHelperTest
{
    class DummyAllowedArgsBaseTest : AbstractAllowedArgsBase
    {
        public DummyAllowedArgsBaseTest()
            : base(Assembly.GetExecutingAssembly())
        {
        }

        public override IEnumerable<ExampleGroup> GetExamples(String appName)
        {
            return new List<ExampleGroup>(0);
        }

        protected override IEnumerable<HelpArgument> GetHelpArguments()
        {
            return new HelpArgument[0];
        }

        protected override IEnumerable<RawHelpArgument> GetRawHelpArguments()
        {
            return new RawHelpArgument[0];
        }

        protected override void InstantiateArguments()
        {
            Help1 = new Argument("-h", "Displays this help", false, 1, false);
            Help2 = new Argument("-help", "Displays this help", false, 1, false);
            Input = new Argument("-in", "Input files", true, 1, true);
        }
        public Argument Help1 { get; private set; }

        [NonArgument]
        public Argument Help2 { get; private set; }

        public Argument Input { get; private set; }
    }

    class DummyAllowedArgsTest2 : AbstractAllowedArgsBase
    {
        public DummyAllowedArgsTest2()
            : base(Assembly.GetExecutingAssembly())
        {
        }

        public override IEnumerable<ExampleGroup> GetExamples(String appName)
        {
            return new List<ExampleGroup>(0);
        }


        protected override IEnumerable<HelpArgument> GetHelpArguments()
        {
            return new HelpArgument[0];
        }

        protected override IEnumerable<RawHelpArgument> GetRawHelpArguments()
        {
            return new RawHelpArgument[0];
        }

        protected override void InstantiateArguments()
        {
            Help1 = new Argument(new string[] { "-h", "-help" }, "Displays this help", false, 1, false);
            Input = new Argument(new string[] { "-in", "-input" }, "Input files", true, 1, true);
        }
        public Argument Help1 { get; private set; }

        public Argument Input { get; private set; }
    }

    class DummyArgumentsChecker : AbstractArgumentsChecker
    {
        public DummyArgumentsChecker(string[] args, DummyAllowedArgsTest2 allowedArgs)
            : base(args, allowedArgs, new ConsoleLogProvider())
        {
        }
    }


    [TestClass]
    public class AbstractAllowedArgsBaseTest
    {
        [TestMethod]
        public void GetArguments()
        {
            DummyAllowedArgsBaseTest dummy = GetAllowedArgsTest();
            Assert.AreEqual(2, dummy.GetAllowedMap().Count);
            CollectionAssert.AreEquivalent(new String[] { "-h", "-in" }, dummy.GetAllowedMap().Keys.ToArray());
        }

        private static DummyAllowedArgsBaseTest GetAllowedArgsTest()
        {
            var args = new DummyAllowedArgsBaseTest();
            args.Initialize();
            return args;
        }

        [TestMethod]
        public void AreAllowed()
        {
            DummyAllowedArgsTest2 dummy = GetAllowedArgsTest2();
            Assert.AreEqual(4, dummy.GetAllowedMap().Count);
            Assert.IsTrue(dummy.IsArgAllowed("-input"));
            Assert.IsTrue(dummy.IsArgAllowed("-in"));
            Assert.IsTrue(dummy.IsArgAllowed("-h"));
            Assert.IsTrue(dummy.IsArgAllowed("-help"));
            Assert.IsFalse(dummy.IsArgAllowed("-input1"));


            // no mandator arg
            DummyArgumentsChecker checker = new DummyArgumentsChecker(new string[] { "" }, dummy);
            var result = checker.PerformAllChecksWResult();
            Assert.AreEqual(ArgCheckResult.ErrorFound, result);

            // arg has not value
            checker = new DummyArgumentsChecker(new string[] { "-in", }, dummy);
            result = checker.PerformAllChecksWResult();
            Assert.AreEqual(ArgCheckResult.ErrorFound, result);

            //valid arg
            checker = new DummyArgumentsChecker(new string[] { "-in", "value" }, dummy);
            result = checker.PerformAllChecksWResult();
            Assert.AreEqual(ArgCheckResult.Continue, result);

            // two times the same
            checker = new DummyArgumentsChecker(new string[] { "-in", "value", "-input", "value2" }, dummy);
            result = checker.PerformAllChecksWResult();
            Assert.AreEqual(2, checker.GetArgCount(dummy.Input));
            Assert.AreEqual(ArgCheckResult.ErrorFound, result);
        }

        private DummyAllowedArgsTest2 GetAllowedArgsTest2()
        {
            var args = new DummyAllowedArgsTest2();
            args.Initialize();
            return args;
        }
    }
}
