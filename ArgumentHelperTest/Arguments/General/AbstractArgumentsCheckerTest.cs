using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ArgumentHelper.Arguments.General;
using ArgumentHelper.Arguments.General.Logging;
using System.Collections.Generic;
using System.Reflection;

namespace ArgumentHelperTest.Arguments.General
{
    [TestClass]
    public class AbstractArgumentsCheckerTest
    {
        public class FakeAllowedArgs : AbstractAllowedArgsBase
        {
            public Argument Arg1 { get; private set; }

            public Argument Arg2 { get; private set; }

            public FakeAllowedArgs() : base(System.Reflection.Assembly.GetExecutingAssembly())
            {
            }

            public override IEnumerable<ExampleGroup> GetExamples(string appName)
            {
                return null;
            }

            protected override void InstantiateArguments()
            {
                Arg1 = new Argument("-arg1", "d", false, 1, false);
                Arg2 = new Argument("-arg2", "d", false, 1, false);

            }
        }
        public class FakeArgumentsChecker : AbstractArgumentsChecker
        {
            private FakeAllowedArgs allowedArgs;

            public FakeArgumentsChecker(string[] args, FakeAllowedArgs allowedArgs) : base(args, allowedArgs, new ConsoleLogProvider())
            {
                this.allowedArgs = allowedArgs;
            }
            public override void PerformCheck()
            {
                base.PerformCheck();
                if (GetArgCount(allowedArgs.Arg1) > 0)
                {
                    EnsureArgumentSet("argument is not allowed ", allowedArgs.Arg1);
                }
                if (GetArgCount(allowedArgs.Arg2) > 0)
                {
                    EnsureArgumentSet("argument is not allowed ", allowedArgs.Arg2);
                }
            }
        }

        [TestMethod]
        public void Arg1Valid()
        {
            FakeArgumentsChecker checker = new FakeArgumentsChecker(new String[] { "-arg1" }, new FakeAllowedArgs());
            Assert.IsTrue(checker.PerformAllChecks());
        }

        [TestMethod]
        public void Arg2Valid()
        {
            FakeArgumentsChecker checker = new FakeArgumentsChecker(new String[] { "-arg2" }, new FakeAllowedArgs());
             Assert.IsTrue(checker.PerformAllChecks());
        }
        [TestMethod]
        public void BothArgs()
        {
            FakeArgumentsChecker checker = new FakeArgumentsChecker(new String[] { "-arg1", "-arg2" }, new FakeAllowedArgs());
            Assert.IsFalse(checker.PerformAllChecks());
        }
    }
}
