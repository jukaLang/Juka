using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JukaNetwork
{
    internal class Menu
    {
        public static void DisplayMenu()
        {

            MenuScreen();
            GetInputFromUser();

        }

        private static void MenuScreen()
        {
            Console.Clear();
            Console.WriteLine("JUKA NETWORK MENU ");
            Console.WriteLine("=========================");
            Console.WriteLine("1. Get Genesis Block");
            Console.WriteLine("2. Get Last Block");
            Console.WriteLine("3. Send Coin");
            Console.WriteLine("4. Create Block (mining)");
            Console.WriteLine("5. Check Balance");
            Console.WriteLine("6. Transaction History");
            Console.WriteLine("7. Blockchain Explorer");
            Console.WriteLine("8. Exit");
            Console.WriteLine("=========================");
        }

        private static void GetInputFromUser()
        {
            int selection = 0;
            while (selection != 20)
            {
                switch (selection)
                {
                    case 1:
                        MenuGenesisBlock();
                        break;
                    case 2:
                        MenuLastBlock();
                        break;
                    case 3:
                        MenuSendCoin();
                        break;
                    case 4:
                        MenuCreateBlock();
                        break;
                    case 5:
                        MenuGetBalance();
                        break;
                    case 6:
                        MenuGetTransactionHistory();
                        break;
                    case 7:
                        MenuShowBlockchain();
                        break;

                    case 8:
                        MenuExit();
                        break;
                }


            }
        }

        private static void MenuExit()
        {
            Console.WriteLine("=Exited Juka Network=");
            Environment.Exit(0);
        }

        private static void MenuShowBlockchain()
        {
            Console.Clear();
            Console.WriteLine("=Blockchain Explorer=");
            Console.WriteLine("Time: {0}", DateTime.Now);
            Console.WriteLine("======================");
            var blockchain = Blockchain.GetBlocks();
            var blocks = blockchain.FindAll();

            foreach (Block block in blocks)
            {
                Console.WriteLine("Index      : {0}", block.Index);
                Console.WriteLine("Timestamp   : {0}", block.TimeStamp);
                Console.WriteLine("Prev. Hash  : {0}", block.PreviousHash);
                Console.WriteLine("Hash        : {0}", block.Hash);
                Console.WriteLine("Creator        : {0}", block.Creator);
                Console.WriteLine("Nonce        : {0}", block.Nonce);



                if (block.Index == 1)
                {
                    Console.WriteLine("Transactions : {0}", block.Transactions);
                }
                else
                {
                    var transactions = JsonConvert.DeserializeObject<List<Transaction>>(block.Transactions);
                    Console.WriteLine("Transactions:");
                    foreach (Transaction trx in transactions)
                    {
                        Console.WriteLine("   Timestamp   : {0}", trx.TimeStamp);
                        Console.WriteLine("   Sender      : {0}", trx.From);
                        Console.WriteLine("   Recipient   : {0}", trx.To);
                        Console.WriteLine("   Amount      : {0}", trx.Amount);
                        Console.WriteLine("   - - - - - - ");

                    }
                }


                Console.WriteLine("--------------\n");

            }


        }

        private static void MenuGetTransactionHistory()
        {
            Console.Clear();
            Console.WriteLine("Get Transaction History");
            Console.WriteLine("Please enter name:");
            var name = Console.ReadLine();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("\n\nError, Please input name!\n");
                return;
            }

            Console.Clear();
            Console.WriteLine("Transaction History for {0}", name);
            Console.WriteLine("Time: {0}", DateTime.Now);
            Console.WriteLine("======================");
            var trxs = Transaction.GetHistory(name);

            if (trxs != null && trxs.Any())
            {
                foreach (Transaction trx in trxs)
                {
                    Console.WriteLine("Timestamp   : {0}", trx.TimeStamp);
                    Console.WriteLine("Sender      : {0}", trx.From);
                    Console.WriteLine("Recipient   : {0}", trx.To);
                    Console.WriteLine("Amount      : {0}", trx.Amount);
                    Console.WriteLine("--------------\n");

                }
            }
            else
            {
                Console.WriteLine("\n---- no record found! ---");
            }


        }

        private static void MenuGetBalance()
        {
            Console.Clear();
            Console.WriteLine("Get Balance Account");
            Console.WriteLine("Please enter name:");
            string name = Console.ReadLine();

            if (string.IsNullOrEmpty(name))
            {
                Console.WriteLine("Error, Please input name!");
                return;
            }

            Console.Clear();
            Console.WriteLine("Balance for {0}", name);
            Console.WriteLine("Time: {0}", DateTime.Now);
            Console.WriteLine("======================");
            var balance = Transaction.GetBalance(name);
            Console.WriteLine("Balance: {0}", balance.ToString("N", CultureInfo.InvariantCulture));

        }

        private static void MenuLastBlock()
        {
            Console.Clear();
            Console.WriteLine("Last Block");
            Console.WriteLine("Time: {0}", DateTime.Now);
            Console.WriteLine("======================");
            var block = Blockchain.GetLastBlock();

            Console.WriteLine("Index      : {0}", block.Index);
            Console.WriteLine("Timestamp   : {0}", block.TimeStamp);
            Console.WriteLine("Prev. Hash  : {0}", block.PreviousHash);
            Console.WriteLine("Hash        : {0}", block.Hash);
            Console.WriteLine("Creator        : {0}", block.Creator);
            Console.WriteLine("Nonce        : {0}", block.Nonce);


            var transactions = JsonConvert.DeserializeObject<List<Transaction>>(block.Transactions);
            Console.WriteLine("Transactions:");
            foreach (Transaction trx in transactions)
            {
                Console.WriteLine("Timestamp   : {0}", trx.TimeStamp);
                Console.WriteLine("Sender      : {0}", trx.From);
                Console.WriteLine("Recipient   : {0}", trx.To);
                Console.WriteLine("Amount      : {0}", trx.Amount);
                Console.WriteLine("--------------\n");

            }




        }

        private static void MenuGenesisBlock()
        {
            Console.Clear();
            Console.WriteLine("Genesis Block");
            Console.WriteLine("Time: {0}", DateTime.Now);
            Console.WriteLine("======================");
            var genesisBlock = Blockchain.GetGenesisBlock();
            Console.WriteLine(JsonConvert.SerializeObject(genesisBlock, Formatting.Indented));
        }

        private static void MenuSendCoin()
        {
            Console.Clear();
            Console.WriteLine("Send Money");
            Console.WriteLine("Time: {0}", DateTime.Now);
            Console.WriteLine("======================");


            Console.WriteLine("Please enter the sender name!:");
            Console.WriteLine("(type 'ga1' or 'ga2' for first time)");
            string sender = Console.ReadLine();

            Console.WriteLine("Please enter the recipient name!:");
            string recipient = Console.ReadLine();

            Console.WriteLine("Please enter the amount (number)!:");
            string strAmount = Console.ReadLine();

            Console.WriteLine("Please enter fee (number)!:");
            string strFee = Console.ReadLine();
            double amount;

            // validate input
            if (string.IsNullOrEmpty(sender) ||
                string.IsNullOrEmpty(recipient) ||
                string.IsNullOrEmpty(strAmount) ||
                string.IsNullOrEmpty(strFee))
            {

                Console.WriteLine("Error, Please input all data: sender, recipient, amount and fee!\n");
                return;
            }

            // validate amount
            try
            {
                amount = double.Parse(strAmount);
            }
            catch
            {
                Console.WriteLine("Error! You have inputted the wrong value for  the amount!");
                return;
            }

            double fee;
            // validate fee
            try
            {
                fee = float.Parse(strFee);
            }
            catch
            {
                Console.WriteLine("Error! You have inputted the wrong value for the fee!");
                return;
            }

            // validating fee
            // asume max fee is 50% of amount
            if (fee > (0.5 * amount))
            {
                Console.WriteLine("Error! You have inputted the fee to high, max fee 50% of amount!");
                return;
            }

            //get sender balance
            var senderBalance = Transaction.GetBalance(sender);

            // validate amount and fee
            if ((amount + fee) > senderBalance)
            {
                Console.WriteLine("Error! Sender ({0}) don't have enough balance!", sender);
                Console.WriteLine("Sender ({0}) balance is {1}", sender, senderBalance);
                return;
            }


            //Create transaction
            var newTrx = new Transaction()
            {
                TimeStamp = DateTime.Now.Ticks,
                Sender = sender,
                Recipient = recipient,
                Amount = amount,
                Fee = fee
            };

            Transaction.AddToPool(newTrx);
            Console.Clear();
            Console.WriteLine("\n\n\n\nHoree, transaction added to transaction pool!.");
            Console.WriteLine("Sender: {0}", sender);
            Console.WriteLine("Recipient {0}", recipient);
            Console.WriteLine("Amount: {0}", amount);
            Console.WriteLine("Fee: {0}", fee);


        }

        private static void MenuCreateBlock()
        {
            Console.Clear();
            Console.WriteLine("\n\n\nCreate Block");
            Console.WriteLine("======================");
            var trxPool = Transaction.GetPool();
            var transactions = trxPool.FindAll();
            var numOfTrxInPool = trxPool.Count();
            if (numOfTrxInPool <= 0)
            {
                Console.WriteLine("No transaction in pool, please create transaction first!");
            }
            else
            {
                var lastBlock = Blockchain.GetLastBlock();

                // create block from transaction pool
                string tempTransactions = JsonConvert.SerializeObject(transactions);

                var block = new Block(lastBlock, tempTransactions);
                Console.WriteLine("Block created and added to Blockchain");

                Blockchain.AddBlock(block);

                // move all record in trx pool to transactions table
                foreach (Transaction trx in transactions)
                {
                    Transaction.Add(trx);
                }

                // clear mempool
                trxPool.DeleteAll();
            }
        }

    }
}
