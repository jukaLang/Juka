using LiteDB;

namespace JukaNetwork
{
    public class Transaction
    {
        public DateTime TimeStamp { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public int Amount { get; set; }


        public Transaction(DateTime timestamp, string from, string to, int amount)
        {
            TimeStamp = timestamp;
            From = from;
            To = to;
            Amount = amount;
        }


        //Temporary
        public static void AddToPool(Transaction transaction)
        {
            var trxPool = GetPool();
            trxPool.Insert(transaction);
        }

        public static void Add(Transaction transaction)
        {
            var transactions = GetAll();
            transactions.Insert(transaction);
        }

        public static ILiteCollection<Transaction> GetPool()
        {
            var coll = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTION_POOL);
            return coll;
        }

        public static ILiteCollection<Transaction> GetAll()
        {
            var coll = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTIONS);
            return coll;
        }

        /**
        * get transaction list by name
        */
        public static IEnumerable<Transaction> GetHistory(string name)
        {
            var coll = DbAccess.DB.GetCollection<Transaction>(DbAccess.TBL_TRANSACTIONS);
            var transactions = coll.Find(x => x.From == name || x.To == name);
            return transactions;
        }


        /**
        * Get balance by name
        */
        public static double GetBalance(string name)
        {
            double balance = 0;
            double spending = 0;
            double income = 0;

            var collection = GetAll();
            var transactions = collection.Find(x => x.Sender == name || x.Recipient == name);

            foreach (Transaction trx in transactions)
            {
                var sender = trx.Sender;
                var recipient = trx.Recipient;

                if (name.ToLower().Equals(sender.ToLower()))
                {
                    spending += trx.Amount + trx.Fee;
                }

                if (name.ToLower().Equals(recipient.ToLower()))
                {
                    income += trx.Amount;
                }

                balance = income - spending;
            }

            return balance;
        }
    }
}