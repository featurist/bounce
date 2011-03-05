﻿using System;
using System.IO;
using Bounce.Framework;
using Bounce.TestHelpers;
using ICSharpCode.SharpZipLib.Zip;
using Moq;
using NUnit.Framework;

namespace Bounce.Console.Tests {
    [TestFixture]
    public class RunsBatchFileBeforeRunningTargetsFeature {
        private void UnzipSolution() {
            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature");
            new FastZip().ExtractZip("BeforeBounceFeature.zip", "BeforeBounceFeature", null);
        }

        [Test]
        public void ShouldRunBatchFileBeforeRunningTargets() {
            UnzipSolution();

            FileSystemTestHelper.RecreateDirectory(@"BeforeBounceFeature\BeforeBounceFeature\bounce");
            File.WriteAllText(@"BeforeBounceFeature\BeforeBounceFeature\bounce\beforebounce.bat", @"msbuild BeforeBounceFeature.sln");

            var shell = new ShellCommandExecutor(() => new FakeLog());

            ProcessOutput output = null;

            Pushd(@"BeforeBounceFeature\BeforeBounceFeature", () => {
                output = shell.Execute("bounce.exe", "BeforeBounceFeature");
                System.Console.WriteLine(output.ErrorAndOutput);
            });

            Assert.That(output, Is.Not.Null);
            Assert.That(output.ExitCode, Is.EqualTo(0));
            Assert.That(output.Error.Trim(), Is.EqualTo(""));
            Assert.That(output.Output, Is.StringContaining("building before bounce feature"));
        }

        public void Pushd(string dir, Action inDirectory) {
            string cwd = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(dir);
            try {
                inDirectory();
            } finally {
                Directory.SetCurrentDirectory(cwd);
            }
        }
    }
}