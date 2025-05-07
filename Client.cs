using Grpc.Net.Client;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using medical; // Namespace from generated proto

namespace ClientApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("http://192.168.56.1:5001"); // IP of Dell Server
            var client = new MedicalRecordService.MedicalRecordServiceClient(channel);

            Console.WriteLine("--- Enter Medical Record Details ---");
            Console.Write("Patient Name: ");
            string patientName = Console.ReadLine();
            Console.Write("Age: ");
            string age = Console.ReadLine();
            Console.Write("Gender: ");
            string gender = Console.ReadLine();
            Console.Write("Symptoms: ");
            string symptoms = Console.ReadLine();
            Console.Write("Diagnosis: ");
            string diagnosis = Console.ReadLine();
            Console.Write("Prescribed Medicine: ");
            string prescription = Console.ReadLine();
            Console.Write("Doctor Name: ");
            string doctor = Console.ReadLine();
            Console.Write("Date (YYYY-MM-DD): ");
            string date = Console.ReadLine();

            string medicalRecord = $@"
Patient Name: {patientName}
Age: {age}
Gender: {gender}
Symptoms: {symptoms}
Diagnosis: {diagnosis}
Prescribed Medicine: {prescription}
Doctor: {doctor}
Date: {date}";
  using var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKeyXml);

            byte[] data = Encoding.UTF8.GetBytes(medicalRecord);
            byte[] signature = rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

            var request = new AddRecordRequest
            {
                HospitalId = "HospitalA",
                Record = medicalRecord,
                Signature = Convert.ToBase64String(signature)
            };

            var response = await client.AddRecordAsync(request);
            Console.WriteLine("\n Server Response: " + response.Message);
        }
    }
}
