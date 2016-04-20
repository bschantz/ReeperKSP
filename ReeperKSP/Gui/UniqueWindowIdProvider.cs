namespace ReeperKSP.Gui
{
    public class UniqueWindowIdProvider
    {
        private static int _id = 15000;

        public static void SetStartingId(int id)
        {
            _id = id;
        }


        public static int Get()
        {
            return _id++;
        }
    }
}
