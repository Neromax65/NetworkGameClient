using System.Collections.Generic;
using MessagePack;

namespace Network.NetworkData
{
    /// <summary>
    /// Network data packet class, that represents couple units of Network Data
    /// </summary>
    [MessagePackObject()]
    public class DataPacket
    {
        /// <summary>
        /// Collection of Network Data that will be send in this packet
        /// </summary>
        [Key(0)] public List<INetworkData> DataList;

        public DataPacket()
        {
            DataList = new List<INetworkData>();
        }
        
        [SerializationConstructor]
        public DataPacket(List<INetworkData> dataList)
        {
            DataList = dataList;
        }

        /// <summary>
        /// Add network data to packet
        /// </summary>
        /// <param name="data">Data to add</param>
        public void Add(INetworkData data)
        {
            DataList.Add(data);
        }

        /// <summary>
        /// Remove network data from packet
        /// </summary>
        /// <param name="data">Data to remove</param>
        public void Remove(INetworkData data)
        {
            DataList.Remove(data);
        }

        /// <summary>
        /// Clear current list of Network Data
        /// </summary>
        public void Clear()
        {
            DataList.Clear();
        }
    }
}