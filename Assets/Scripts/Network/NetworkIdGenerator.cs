using UnityEngine;

namespace Network
{
    /// <summary>
    /// Class to generate global network identity
    /// </summary>
    public static class NetworkIdGenerator
    {
        /// <summary>
        /// Id that was set to last network object
        /// </summary>
        private static int _lastId;

        /// <summary>
        /// Generate network Id
        /// </summary>
        /// <returns></returns>
        public static int Generate()
        {
            return _lastId++;
        }

        /// <summary>
        /// Setter for last id
        /// </summary>
        /// <param name="value">Last network Id value</param>
        public static void SetLastId(int value)
        {
            _lastId = value;
        }
    }
}
