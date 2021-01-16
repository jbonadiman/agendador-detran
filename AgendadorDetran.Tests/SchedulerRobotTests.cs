using System.Diagnostics;
using AgendadorDetran.Core.Data.Enums;
using AgendadorDetran.Core.Robots;
using FluentAssertions;
using NUnit.Framework;

namespace AgendadorDetran.Tests
{
    public class SchedulerRobotTests
    {
        private SchedulerRobot robot;
        
        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            this.robot = new SchedulerRobot(BrowserType.Chrome);
        }

        [Test]
        public void ShouldOpenANewBrowser()
        {
            int pid = this.robot.OpenBrowser(BrowserMode.Maximized);

            var processInfo = Process.GetProcessById(pid);
            
            processInfo.ProcessName.Should().Be("chrome.exe");
        }
    }

    
}