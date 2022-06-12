using LiteDB;
using Newtonsoft.Json;

namespace JukaNetwork
{
    public class Blockchain
    {
        IList<Transaction> PendingTransactions = new List<Transaction>();
        public IList<Block> Chain { set; get; } //Temporary Database to hold all the blocks... replace with database
        public int Difficulty { set; get; } = 3;
        public int Reward = 1; 

        public Blockchain()
        {
            Initialize();
        }

        public void Initialize() //Check to see that there is no block, and create one if it doesn't exist
        {
            var blocks = GetBlocks();
            if (blocks.Count() < 1)
            {
                // Create Genesis Block
                var gnsBlock = Block.Genesis();
                blocks.Insert(gnsBlock);

                //Create Genesis Transaction
                CreateGenesisBlockTransaction();
            }
            Chain = new List<Block>();
        }

        public static void CreatGenesisBlockTransaction()
        {
            var genesisTrx = new Transaction(DateTime.Now, "JukaGenesis", "JukaNetwork", 1000);
            Transaction.AddToPool(genesisTrx);

            var trxPool = Transaction.GetPool();
            var transactions = trxPool.FindAll();
            string tempTransactions = JsonConvert.SerializeObject(transactions);
            var lastBlock = GetLastBlock(); //Last Block Cache
            var block = new Block(lastBlock, tempTransactions);

            // move all record from trx pool to transactions table
            foreach (Transaction trx in transactions)
            {
                Transaction.Add(trx);
            }

            // clear mempool
            trxPool.DeleteAll();

        }

        public static ILiteCollection<Block> GetBlocks()
        {
            var coll = DbAccess.DB.GetCollection<Block>(DbAccess.TBL_BLOCKS);
            coll.EnsureIndex(x => x.Index);
            return coll;
        }

        public static Block GetGenesisBlock()
        {
            var blockchain = GetBlocks();
            var block = blockchain.FindOne(Query.All(Query.Ascending));
            return block;
        }

        public static Block GetLastBlock()
        {
            var blockchain = GetBlocks();
            var block = blockchain.FindOne(Query.All(Query.Descending));
            return block;
        }

        //Add new block to the blockchain
        public void AddBlock(Block block)
        {
            Block latestBlock = GetLastBlock(); //Get the last block
            block.Index = latestBlock.Index + 1; //New Block Index is +1
            block.PreviousHash = latestBlock.Hash; //Previous Hash of block
            //block.Hash = block.CalculateHash();
            block.Mine(this.Difficulty);  //Mine new block to generate appropriate hash!
            Chain.Add(block);
        }




        public Block GetLastBlock()
        {
            return Chain[Chain.Count - 1];
        }

        public Block GetGenesisBlock()
        {
            return Chain[0];
        }

        public void PrintGenesisBlock()
        {
            Console.WriteLine("==== Genesis Block ====");
            PrintBlock(GetGenesisBlock());
        }

        public void PrintLastBlock()
        {
            Console.WriteLine("==== Last Block ====");
            PrintBlock(GetLastBlock());
        }



        public void CreateTransaction(Transaction transaction)
        {
            PendingTransactions.Add(transaction);
        }
        public void ProcessPendingTransactions(string minerAddress)
        {
            Block block = new(DateTime.Now, GetLastBlock().Hash, PendingTransactions,minerAddress);
            AddBlock(block);

            PendingTransactions = new List<Transaction>();
            CreateTransaction(new Transaction(DateTime.Now,"JukaRewardSystem", minerAddress, Reward));
        }




        public void PrintBlocks()
        {
            Console.WriteLine("====== All Blocks on Blockchain =====");
            foreach (Block block in Chain)
            {
                PrintBlock(block);
            }
        }

        public void PrintBlock(Block block)
        {
            Console.WriteLine("--------------");
            Console.WriteLine("Index      :{0}", block.Index);
            Console.WriteLine("Timestamp   :{0}", block.TimeStamp);
            Console.WriteLine("Prev. Hash  :{0}", block.PreviousHash);
            Console.WriteLine("Hash        :{0}", block.Hash);
            Console.WriteLine("Transactions :{0}", block.Transactions);
            Console.WriteLine("Creator     :{0}", block.Creator);
            Console.WriteLine("Nonce     :{0}", block.Nonce);
            Console.WriteLine("--------------");
        }


        public void PrintTransactionHistory(string name)
        {
                Console.WriteLine("==== Transaction History for { 0} === ", name);
                foreach (Block block in Chain)
                {
                    var transactions = block.Transactions;
                    foreach (var transaction in transactions)
                    {
                        var sender = transaction.To;
                        var recipient = transaction.From;
                        if (name.ToLower().Equals(sender.ToLower())
                         || name.ToLower().Equals(recipient.ToLower()))
                        {
                            Console.WriteLine("Timestamp :{0}", transaction.TimeStamp);
                            Console.WriteLine("Sender   :{0}", transaction.To);
                            Console.WriteLine("Recipient :{0}", transaction.From);
                            Console.WriteLine("Amount    :{0}", transaction.Amount);
                            Console.WriteLine("--------------");
                        }
                    }
            }
        }


        public bool IsValid()
        {
            for (int i = 1; i < Chain.Count; i++)
            {
                Block currentBlock = Chain[i];
                Block previousBlock = Chain[i - 1];

                if (currentBlock.Hash != currentBlock.CalculateHash())
                {
                    return false;
                }

                if (currentBlock.PreviousHash != previousBlock.Hash)
                {
                    return false;
                }
            }
            return true;
        }

        public int PrintBalance(string address)
        {
            int balance = 0;

            for (int i = 0; i < Chain.Count; i++)
            {
                for (int j = 0; j < Chain[i].Transactions.Count; j++)
                {
                    var transaction = Chain[i].Transactions[j];

                    if (transaction.From == address)
                    {
                        balance -= transaction.Amount;
                    }

                    if (transaction.To == address)
                    {
                        balance += transaction.Amount;
                    }
                }
            }

            return balance;
        }
    }
}