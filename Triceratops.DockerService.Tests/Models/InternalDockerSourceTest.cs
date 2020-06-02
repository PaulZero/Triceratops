using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions.TestingHelpers;
using Triceratops.DockerService.Models;
using Triceratops.DockerService.Structs;

namespace Triceratops.DockerService.Tests.Models
{
    [TestClass]
    public class InternalDockerSourceTest
    {
        [TestMethod]
        public void TestThatMatchesMethodCorrectlyMatchesId()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
            {
                [@"/temp/some-directory"] = new MockDirectoryData()
            });

            var source = new InternalDockerSource(fileSystem.DirectoryInfo.FromDirectoryName("/temp/some-directory"), new ImageConfig
            {
                Name = "Steve",
                Versions = new[]
                {
                    new ImageVersion
                    {
                        Tag = "1.0"
                    },
                    new ImageVersion
                    {
                        Tag = "2.0"
                    }
                }
            });

            var steveOneId = new DockerImageId("Steve", "1.0");
            var steveTwoId = new DockerImageId("Steve", "2.0");
            var steveThreeId = new DockerImageId("Steve", "3.0");

            Assert.IsTrue(source.Matches(steveOneId));
            Assert.IsTrue(source.Matches(steveTwoId));
            Assert.IsFalse(source.Matches(steveThreeId));
        }
    }
}
