using System;
using System.Net;
using System.Threading.Tasks;

namespace SourceQuery.Sample
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var addresses = Dns.GetHostAddresses("172.20.32.1");
            if (addresses.Length < 1)
            {
                Console.WriteLine("Failed to find IP address.");
                return;
            }

            var client = new A2SClient(new A2SClientOptions
            {
                Address = addresses[0]
            });
            
            await client.Connect();
            client.Close();
        }
    }
}