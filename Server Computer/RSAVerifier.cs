using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
public static class RSAVerifier
{
    public static bool Verify(string data, string signature, string publicKeyPem)
    {
        using var rsa = RSA.Create();
        rsa.ImportFromPem(publicKeyPem);
        var dataBytes = Encoding.UTF8.GetBytes(data);
        var sigBytes = Convert.FromBase64String(signature);
        return rsa.VerifyData(dataBytes, sigBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
}
