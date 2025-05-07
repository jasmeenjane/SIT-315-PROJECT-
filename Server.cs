using Grpc.Core;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;

namespace MedicalRecordSystem
{
    public class Program
    {
        static Dictionary<string, string> registeredHospitals = new()
        {
            { "HospitalA", "<HospitalA_Public_Key_Base64>" }
        };

        static List<Block> blockchain = new() { new Block("Genesis Block", DateTime.Now, "0") };

        public static void Main()
        {
            var server = new Server
            {
                Services = { MedicalRecordService.BindService(new MedicalRecordServiceImpl()) },
                Ports = { new ServerPort("192.168.56.1", 5001, ServerCredentials.Insecure) }
            };
            server.Start();
            Console.WriteLine("Server started on port 5001...");
            Console.ReadKey();
        }

        public class MedicalRecordServiceImpl : MedicalRecordService.MedicalRecordServiceBase
        {
            public override Task<AddRecordResponse> AddRecord(AddRecordRequest request, ServerCallContext context)
            {
                if (registeredHospitals.TryGetValue(request.HospitalId, out var pubKeyBase64))
                {
                    using var rsa = new RSACryptoServiceProvider();
                    rsa.ImportRSAPublicKey(Convert.FromBase64String(pubKeyBase64), out _);
                    byte[] data = Encoding.UTF8.GetBytes(request.Record);
                    byte[] signature = Convert.FromBase64String(request.Signature);

                    bool isValid = rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    if (isValid)
                    {
                        AddToBlockchain(request.Record);
                        return Task.FromResult(new AddRecordResponse { Message = "Record added to blockchain." });
                    }
                }

                return Task.FromResult(new AddRecordResponse { Message = "Invalid signature or hospital." });
            }

            private void AddToBlockchain(string data)
            {
                var prev = blockchain[^1];
                var newBlock = new Block(data, DateTime.Now, prev.Hash);
                blockchain.Add(newBlock);
                Console.WriteLine($"New Block Added: {data}");
            }
        }

        public class Block
        {
            public string Data { get; }
            public DateTime Timestamp { get; }
            public string PreviousHash { get; }
            public string Hash { get; }

            public Block(string data, DateTime time, string prevHash)
            {
                Data = data;
                Timestamp = time;
                PreviousHash = prevHash;
                Hash = ComputeHash();
            }

            private string ComputeHash()
            {
                using var sha = SHA256.Create();
                var bytes = Encoding.UTF8.GetBytes($"{Data}{Timestamp}{PreviousHash}");
                return Convert.ToBase64String(sha.ComputeHash(bytes));
            }
        }
    }
}
