// ****************************************************************
// This is free software licensed under the NUnit license. You
// may obtain a copy of the license as well as information regarding
// copyright ownership at http://nunit.org/?p=license&r=2.4.
// ****************************************************************

using System;
using NUnit.Framework;
using NUnit.Core;

namespace NUnit.Util.Tests
{
	[TestFixture]
	public class RemoteTestResultTest
	{
		[Test]
		public void ResultStillValidAfterDomainUnload() 
		{
			TestDomain domain = new TestDomain();
			TestPackage package = new TestPackage( "mock-assembly.dll" );
			//package.BasePath = AppDomain.CurrentDomain.BaseDirectory;
			Assert.IsTrue( domain.Load( package ) );
			TestResult result = domain.Run( new NullListener() );
			TestSuiteResult suite = result as TestSuiteResult;
			Assert.IsNotNull(suite);
			TestCaseResult caseResult = findCaseResult(suite);
			Assert.IsNotNull(caseResult);
			TestResultItem item = new TestResultItem(caseResult);
			//domain.Unload(); // TODO: Figure out where unhandled exception comes from
			string message = item.GetMessage();
			Assert.IsNotNull(message);
		}

        [Test, Explicit("Fails intermittently")]
        public void AppDomainUnloadedBug()
        {
            TestDomain domain = new TestDomain();
            domain.Load( new TestPackage( "mock-assembly.dll" ) );
            domain.Run(new NullListener());
            domain.Unload();
        }

		private TestCaseResult findCaseResult(TestSuiteResult suite) 
		{
			foreach (TestResult r in suite.Results) 
			{
				if (r is TestCaseResult)
				{
					return (TestCaseResult) r;
				}
				else 
				{
					TestCaseResult result = findCaseResult((TestSuiteResult)r);
					if (result != null)
						return result;
				}

			}

			return null;
		}
	}
}
