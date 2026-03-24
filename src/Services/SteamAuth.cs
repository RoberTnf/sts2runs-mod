using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Steamworks;

#nullable enable

namespace STS2RunsMod
{
    public static class SteamAuth
    {
        internal static string? AuthToken;

        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public static async Task Authenticate(ModConfig config)
        {
            AuthToken = null;

            string steamId;
            string ticketHex;
            HAuthTicket ticket = default;
            bool hasAuthTicket = false;

            try
            {
                try
                {
                    CSteamID cSteamId = SteamUser.GetSteamID();
                    steamId = cSteamId.m_SteamID.ToString();

                    byte[] ticketData = new byte[1024];
                    uint ticketLength;
                    SteamNetworkingIdentity identity = new SteamNetworkingIdentity();
                    identity.SetSteamID(CSteamID.Nil);
                    ticket = SteamUser.GetAuthSessionTicket(ticketData, 1024, out ticketLength, ref identity);
                    hasAuthTicket = true;

                    if (ticketLength == 0)
                        throw new InvalidOperationException("Steamworks returned an empty auth ticket");

                    ticketHex = BitConverter.ToString(ticketData, 0, (int)ticketLength).Replace("-", "").ToLowerInvariant();
                }
                catch (Exception ex)
                {
                    Godot.GD.Print($"[STS2RunsMod] Steam auth unavailable: could not get Steamworks ticket ({ex.Message})");
                    return;
                }

                try
                {
                    string requestBody = JsonSerializer.Serialize(new { steamId = steamId, ticket = ticketHex });
                    StringContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    using HttpResponseMessage response = await _httpClient.PostAsync($"{config.ServerUrl}/api/auth/mod", content);
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        Godot.GD.Print($"[STS2RunsMod] Auth request rejected: {(int)response.StatusCode} {response.ReasonPhrase}");
                        return;
                    }

                    using JsonDocument doc = JsonDocument.Parse(responseBody);
                    if (!doc.RootElement.TryGetProperty("token", out JsonElement tokenElement) || string.IsNullOrWhiteSpace(tokenElement.GetString()))
                    {
                        Godot.GD.Print("[STS2RunsMod] Auth response missing token");
                        return;
                    }

                    AuthToken = tokenElement.GetString();
                    Godot.GD.Print($"[STS2RunsMod] Authenticated as Steam user {steamId}");
                }
                catch (Exception ex)
                {
                    Godot.GD.Print($"[STS2RunsMod] Auth request failed: {ex.Message}");
                }
            }
            finally
            {
                if (hasAuthTicket)
                    SteamUser.CancelAuthTicket(ticket);
            }
        }
    }
}
