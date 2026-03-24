using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace STS2RunsMod
{
    public static class Uploader
    {
        private static readonly HttpClient _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(10) };

        public static async Task Upload(string runJson, int profile)
        {
            byte[] jsonBytes = Encoding.UTF8.GetBytes(runJson);
            string hash = BitConverter.ToString(SHA256.HashData(jsonBytes)).Replace("-", "").ToLowerInvariant();
            string data = Convert.ToBase64String(jsonBytes);

            string body = System.Text.Json.JsonSerializer.Serialize(new { hash, data, profile });

            async Task<HttpResponseMessage> SendRequest()
            {
                var request = new HttpRequestMessage(HttpMethod.Post, $"{Plugin.Config.ServerUrl}/api/runs/upload");
                request.Content = new StringContent(body, Encoding.UTF8, "application/json");

                if (SteamAuth.AuthToken != null)
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", SteamAuth.AuthToken);

                return await _httpClient.SendAsync(request);
            }

            HttpResponseMessage response = await SendRequest();

            // 401 → re-authenticate and retry once
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Godot.GD.Print("[STS2RunsMod] Auth expired, re-authenticating...");
                await SteamAuth.Authenticate(Plugin.Config);

                response = await SendRequest();
            }

            if (response.StatusCode == (System.Net.HttpStatusCode)429)
            {
                Godot.GD.Print("[STS2RunsMod] Rate limited, skipping upload");
                return;
            }

            response.EnsureSuccessStatusCode();

            string responseBody = await response.Content.ReadAsStringAsync();
            Godot.GD.Print($"[STS2RunsMod] Upload response: {responseBody}");
        }
    }
}
