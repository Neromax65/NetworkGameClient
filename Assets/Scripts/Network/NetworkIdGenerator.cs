using UnityEngine;

namespace Network
{
    public static class NetworkIdGenerator
    {
        private static int _lastId;

        public static int Generate()
        {
            return _lastId++;
        }


        public static void SetLastId(int value)
        {
            _lastId = value;
        }
    }
}
