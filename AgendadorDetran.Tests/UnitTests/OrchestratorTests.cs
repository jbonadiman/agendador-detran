using System.Collections.Generic;
using System.Diagnostics;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Interfaces;
using AgendadorDetran.Core.Utils;
using FluentAssertions;
using NUnit.Framework;

namespace AgendadorDetran.Tests.UnitTests
{
    public class OrchestratorTests
    {
        private IOrchestrator _orchestrator;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            this._orchestrator = new Orchestrator();
        }

        [Test]
        public void ShouldOpenAndCloseANewBrowser()
        {
            const BrowserType browserType = BrowserType.Chrome;
            
            using IBrowser browser = this._orchestrator.OpenBrowser(
                browserType,
                BrowserMode.Maximized
            );

            int pid = browser.Service().ProcessId;
            var processInfo = Process.GetProcessById(pid);
            
            processInfo.ProcessName.Should().ContainEquivalentOf(browserType.ToString());
        }
        
        
        
    }
}