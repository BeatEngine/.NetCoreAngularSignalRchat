using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;
using Xunit.v3;

namespace AngularDemoChatSignalR.Test
{
    public class TestMethodOrderAlphabet : ITestCaseOrderer
    {
        public IReadOnlyCollection<TTestCase> OrderTestCases<TTestCase>(IReadOnlyCollection<TTestCase> testCases) where TTestCase : notnull, ITestCase
        {
            return testCases.OrderBy(tc => tc.TestMethodName).ToList();
        }
    }
}
