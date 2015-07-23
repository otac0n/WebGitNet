using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebGitNet.SharedLib;
using System.IO;
using System.Linq;

namespace WebGitNet.SharedLib.Tests
{
    [TestClass]
    public class FileManagerTests
    {
        private string _path = "";

        /// <summary>
        /// Get the fake repository path to test from 
        /// </summary>
        [TestInitialize]
        public void GetRepoPath()
        {
            string startupPath = System.AppDomain.CurrentDomain.BaseDirectory;
            var pathItems = startupPath.Split (Path.DirectorySeparatorChar);
            string projectPath = String.Join (Path.DirectorySeparatorChar.ToString (), pathItems.Take (pathItems.Length - 2));
            _path = Path.Combine (projectPath, "fake_repos");
        }

        /// <summary>
        /// Test that FileManager can locate a directory that has no spaces
        /// </summary>
        [TestMethod]
        public void GetResourceInfoTest_NoSpacesInUrl()
        {
            // Directory without spaces
            FileManager fm = new FileManager(_path);
            ResourceInfo info = fm.GetResourceInfo("mustard");

            Assert.AreEqual(ResourceType.Directory, info.Type);
        }

        /// <summary>
        /// Test that FileManager can find a directory that has URL encoded spaces
        /// </summary>
        [TestMethod]
        public void GetResourceInfoTest_SpacesInUrl()
        {
            // Directory with spaces
            FileManager fm = new FileManager(_path);
            ResourceInfo info = fm.GetResourceInfo("yellow%20pepper");

            Assert.AreEqual(ResourceType.Directory, info.Type);
        }

    }
}
