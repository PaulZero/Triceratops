using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.VisualBasic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text;
using Triceratops.DockerService.Managers;
using Triceratops.DockerService.Models;
using Triceratops.DockerService.Structs;
using Triceratops.Libraries.Helpers;

namespace Triceratops.DockerService.Tests.Managers
{
    [TestClass]
    public class ImageSourceManagerTest
    {
        [TestMethod]
        public void TestThatIsInternalImageWorks()
        {
            var fileSystem = new MockFileSystem();

            AddImageConfigToFileSystem(fileSystem, "Testing");

            var manager = new ImageSourceManager(new NullLogger<ImageSourceManager>(), fileSystem);

            Assert.IsTrue(manager.IsInternalImage(new DockerImageId("Testing", "1.0")));
        }

        [TestMethod]
        public void TestThatRefreshSourcesUpdatesCorrectlyWhenUsedAtRuntime()
        {
            var fileSystem = new MockFileSystem();

            AddImageConfigToFileSystem(fileSystem, "Testing");

            var manager = new ImageSourceManager(new NullLogger<ImageSourceManager>(), fileSystem);

            AddImageConfigToFileSystem(fileSystem, "Steve");

            Assert.IsTrue(manager.IsInternalImage(new DockerImageId("Testing", "1.0")));
            Assert.IsFalse(manager.IsInternalImage(new DockerImageId("Steve", "1.0")));

            manager.RefreshInternalSources();

            Assert.IsTrue(manager.IsInternalImage(new DockerImageId("Testing", "1.0")));
            Assert.IsTrue(manager.IsInternalImage(new DockerImageId("Steve", "1.0")));

            RemoveImageConfigFromFileSystem(fileSystem, "Testing");

            manager.RefreshInternalSources();

            Assert.IsFalse(manager.IsInternalImage(new DockerImageId("Testing", "1.0")));
            Assert.IsTrue(manager.IsInternalImage(new DockerImageId("Steve", "1.0")));
        }

        private void RemoveImageConfigFromFileSystem(MockFileSystem fileSystem, string name)
        {
            var directory = $"{ImageSourceManager.InternalSourcesPath}/{name.ToLower()}";

            fileSystem.RemoveFile($"{directory}/ImageConfig.json");
            fileSystem.RemoveFile($"{directory}/Dockerfile.json");
        }

        private void AddImageConfigToFileSystem(MockFileSystem fileSystem, string name, string tag = "1.0")
        {
            var directory = $"{ImageSourceManager.InternalSourcesPath}/{name.ToLower()}";
            var imageConfig = new ImageConfig
            {
                Name = name,
                Versions = new[]
                {
                    new ImageVersion
                    {
                        Tag = tag
                    }
                }
            };

            fileSystem.AddFile($"{directory}/ImageConfig.json", JsonHelper.Serialise(imageConfig));
            fileSystem.AddFile($"{directory}/Dockerfile", "NOT even a real Dockerfile");
        }

        private MockFileSystem CreateFileSystem()
        {
            var configOne = new ImageConfig
            {
                Name = "Steve",
                Versions = new[]
                {
                    new ImageVersion
                    {
                        Tag = "1.0"
                    }
                }
            };

            return new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [$"{ImageSourceManager.InternalSourcesPath}/testing/ImageConfig.json"] = new MockFileData(JsonHelper.Serialise(configOne)),
                [$"{ImageSourceManager.InternalSourcesPath}/testing/Dockerfile"] = new MockFileData("LABEL this is a real Dockerfile")
            });
        }
    }
}
