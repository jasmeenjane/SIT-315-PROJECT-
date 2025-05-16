using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
public class Block
{
    public string Data;
    public string Signature;
    public DateTime Timestamp;
    public string PreviousHash;
    public string Hash;

    public Block(DateTime time, string data, string signature)
    {
        Timestamp = time;
        Data = data;
        Signature = signature;
    }

    public void ComputeHash(string previous)
    {
        PreviousHash = previous;
        string content = $"{Timestamp}-{Data}-{Signature}-{PreviousHash}";
        Hash = Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(content)));
    }
}

public class Blockchain
{
    private List<Block> _chain = new List<Block>();

    public Blockchain()
    {
        var genesis = new Block(DateTime.Now, "Genesis Block", "");
        genesis.ComputeHash("");
        _chain.Add(genesis);
    }

    public void AddBlock(Block block)
    {
        block.ComputeHash(_chain.Last().Hash);
        _chain.Add(block);
    }
    public List<Block> GetBlocks()
    {
        return _chain;
    }
}
