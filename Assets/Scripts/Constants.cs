/// <summary>
/// Game constant values
/// </summary>
public static class Constants
{
    /// <summary>
    /// Maximum number of server ticks for not receiving any NetworkData
    /// </summary>
    public const int MAX_PING_FAILURE_COUNT = 50;
    
    /// <summary>
    /// Buffer size for one unit of NetworkData
    /// </summary>
    public const int BUFFER_SIZE = 1024;
}
