namespace Steamworks
{
    public struct CSteamID
    {
        public ulong m_SteamID;

        public static readonly CSteamID Nil;
    }

    public struct HAuthTicket
    {
    }

    public struct SteamNetworkingIdentity
    {
        public void SetSteamID(CSteamID steamID)
        {
        }
    }

    public static class SteamUser
    {
        public static CSteamID GetSteamID()
        {
            return default;
        }

        public static HAuthTicket GetAuthSessionTicket(byte[] pTicket, int cbMaxTicket, out uint pcbTicket, ref SteamNetworkingIdentity pIdentity)
        {
            pcbTicket = 0;
            return default;
        }

        public static void CancelAuthTicket(HAuthTicket hAuthTicket)
        {
        }
    }
}
