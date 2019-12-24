using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Tests;
using Universe;
using Universe.CpuUsage;
using Universe.CpuUsage.Tests;

namespace KernelManagementJam.Tests
{
    [TestFixture]
    public class PosixResourcesUsage_Tests : NUnitTestsBase
    {
        [Test]
        public void Is_Supported()
        {
            Console.WriteLine($"PosixResourceUsage.IsSupported: {PosixResourceUsage.IsSupported}");
        }
        
        [Test]
        [TestCase(CpuUsageScope.Thread)]
        [TestCase(CpuUsageScope.Process)]
        public void Smoke_Test(CpuUsageScope scope)
        {
            if (!PosixResourceUsage.IsSupported) return;
            Console.WriteLine($"PosixResourceUsage.GetByScope({scope}): {PosixResourceUsage.GetByScope(scope)}");
        }


        [Test]
        [TestCase(CpuUsageScope.Thread,1)]
        [TestCase(CpuUsageScope.Process,1)]
        [TestCase(CpuUsageScope.Thread,42)]
        [TestCase(CpuUsageScope.Process,42)]
        public void ContextSwitch_Test(CpuUsageScope scope, int switchCount)
        {
            if (!PosixResourceUsage.IsSupported) return;
            if (scope == CpuUsageScope.Thread && CrossInfo.ThePlatform != CrossInfo.Platform.Linux) return;
            
            PosixResourceUsage before = PosixResourceUsage.GetByScope(scope).Value;
            
            for (int i = 0; i < switchCount; i++)
            {
                // CpuLoader.Run(1, 0, true);
                Stopwatch sw = Stopwatch.StartNew();
                while (sw.ElapsedMilliseconds < 1) ;
                Thread.Sleep(1);
            }
            
            PosixResourceUsage after = PosixResourceUsage.GetByScope(scope).Value;
            var delta = PosixResourceUsage.Substruct(after, before);
            Console.WriteLine($"delta.InvoluntaryContextSwitches = {delta.InvoluntaryContextSwitches}");
            Console.WriteLine($"delta.VoluntaryContextSwitches = {delta.VoluntaryContextSwitches}");
            Assert.AreEqual(switchCount, delta.VoluntaryContextSwitches);
        }
    }
}