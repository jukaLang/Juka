using LiteDB;

namespace JukaNetwork
{
    public class DbAccess
    {
        public static LiteDatabase DB { set; get; }
        public const string DB_NAME = @"node.db";
        public const string TBL_BLOCKS = "tbl_blocks"; //Store all created blocks
        public const string TBL_TRANSACTION_POOL = "tbl_transaction_pool"; //Store all unconfirmed transactions. (Contents deleted once the block is created)
        public const string TBL_TRANSACTIONS = "tbl_transactions"; //Store all confirmed transactions
        /**
        create db with name node.db
        **/
        public static void Initialize()
        {
            DB = new LiteDatabase(DB_NAME);
        }
        /**
        Delete all rows for all table
        **/
        public static void ClearDB()
        {
            var coll = DB.GetCollection<Block>(TBL_BLOCKS);
            coll.DeleteAll();
            var coll2 = DB.GetCollection <Transaction>(TBL_TRANSACTION_POOL);
            coll2.DeleteAll();
            var coll3 = DB.GetCollection <Transaction>(TBL_TRANSACTIONS);
            coll3.DeleteAll();
        }
        /**
         * Close database when app closed
         **/
        public static void CloseDB()
        {
            DB.Dispose();
        }
    }
}
