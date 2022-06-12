using JukaNetwork;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

namespace JukaNetwork
{
    public class Block
    {
        public int Index { get; set; }   //Sequence Number Block
        public DateTime TimeStamp { get; set; } //Time when block was created
        public string PreviousHash { get; set; } //Previous Block hash
        public string Hash { get; set; } //Current block hash
        public IList<Transaction> Transactions { get; set; } // List of Transactions
        public string Creator { get; set; } //Track Creator of the block [needed?]
        public int Nonce { get; set; } = 0; //Nonce (to generate proper hash)

        public Block(DateTime timeStamp, string previousHash, IList<Transaction> transactions,string creator)
        {
            Index = 0;
            TimeStamp = timeStamp;
            PreviousHash = previousHash;
            Transactions = transactions;
            Creator = creator;
            Hash = CalculateHash();
        }

        public string CalculateHash()
        {
            SHA256 sha256 = SHA256.Create();

            byte[] inputBytes = Encoding.ASCII.GetBytes($"{TimeStamp}-{PreviousHash ?? ""}-{JsonConvert.SerializeObject(Transactions)}+{Creator}-{Nonce}");
            byte[] outputBytes = sha256.ComputeHash(inputBytes);

            return Convert.ToBase64String(outputBytes);
        }

        public void Mine(int difficulty = 4)
        {
            var leadingZeros = new string('0', difficulty);
            while (this.Hash == null || this.Hash.Substring(0, difficulty) != leadingZeros)
            {
                this.Nonce++;
                this.Hash = this.CalculateHash();
            }
        }
    }
}