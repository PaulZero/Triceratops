using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using Triceratops.Api.Models.StackConfiguration;
using Triceratops.Api.Services.DbService;

namespace Triceratops.Api.Models.Persistence.Stacks
{
    public class ContainerStack : IDbEntity
    {
        public uint Id { get; set; }

        /// <summary>
        /// The type of IStack to be created from this row.
        /// </summary>
        public Type StackType { get; set; }

        public string ConfigBase64 { get; private set; }

        public object Config { get; private set; }

        public async Task<Container[]> GetContainersAsync(IDbContainerRepo containerRepo)
        {
            return await containerRepo.FetchByStackId(Id);
        }

        public bool IsForStack(IStack stack)
        {
            return stack.GetType().Equals(StackType);
        }

        public void SetConfig(object config)
        {
            Config = config;

            RefreshConfigBase64();
        }

        public T GetConfig<T>()
        {
            var json = Encoding.UTF8.GetString(Convert.FromBase64String(ConfigBase64));

            return JsonConvert.DeserializeObject<T>(json);
        }

        private void RefreshConfigBase64()
        {
            if (Config == null)
            {
                ConfigBase64 = "";

                return;
            }

            var json = JsonConvert.SerializeObject(Config);

            ConfigBase64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }
    }
}
