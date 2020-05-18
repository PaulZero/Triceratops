using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using Triceratops.Libraries.Http.Api.Interfaces.Client;
using Triceratops.Libraries.Http.Api.Interfaces.Server;

namespace Triceratops.Libraries.Tests.Http.Api.Interfaces
{
    [TestClass]
    public class ApiInterfaceConsistencyTest
    {
        [TestMethod]
        public void EnsureClientAndServerInterfacesContainMatchingMethods()
        {
            var serverMethods = typeof(ITriceratopsServerApi).GetMethods();
            var clientMethods = typeof(ITriceratopsServersApiClient).GetMethods();

            var serverMethodNames = serverMethods.Select(m => m.Name).ToArray();
            var clientMethodNames = clientMethods.Select(m => m.Name).ToArray();

            CollectionAssert.AreEqual(serverMethodNames, clientMethodNames, "Client and server interfaces must contain the same named methods.");

            foreach (var clientMethod in clientMethods)
            {
                var serverMethod = serverMethods.First(m => m.Name == clientMethod.Name);
                var serverReturnType = serverMethod.ReturnType;
                var clientReturnType = clientMethod.ReturnType;

                Assert.AreEqual(serverReturnType, clientReturnType, "Server endpoints must have the same return type as the client method.");
            }
        }
    }
}
