using Astro.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Astro.Winform.Classes
{
    internal static class HttpClientSingleton
    {
        private static readonly HttpClient client;
        private static readonly object lockObj = new object();
        private static bool isDisposed = false;
        static HttpClientSingleton()
        {
            client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(30)
            };
            client.DefaultRequestHeaders.Add("User-Agent", "astro.winform.app");
        }
        public static HttpClient Instance => client;
        private static Uri CreateUri(string url)
        {
            if (!Uri.TryCreate(My.Application.ApiUrl + url, UriKind.Absolute, out var endpoint))
                throw new ArgumentException($"Invalid URL: {url}");
            return endpoint;
        }
        internal static async Task<bool> SignInAsync(string username, string password)
        {
            Credential request = new Credential(username, password);
            Uri endpoint = CreateUri("/auth/login");
            HttpContent content = new StringContent(request.ToString(), Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await Instance.PostAsync(endpoint, content);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                AuthResponse? authResult = AuthResponse.Create(json);
                if (authResult != null)
                {
                    if (authResult.IsAuthenticated())
                    {
                        if (authResult.AccessToken is null) throw new ArgumentException("token is null");

                        My.Application.ApiToken = authResult.AccessToken;
                        My.Application.User = authResult.UserInfo;
                        return true;
                    }

                    if (!string.IsNullOrEmpty(authResult.Message)) MessageBox.Show(authResult.Message, "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    else MessageBox.Show("Invalid username or password", "Access denied", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show("Request Failed 1: " + ex.Message, "Request failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Request Failed 2: " + ex.Message, "Request failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Clipboard.SetText(ex.Message);
            }
            return false;
        }
        internal static async Task<bool> SignOutAsync()
        {
            if (My.Application.ApiToken.Trim() == "") return false;

            string response = await PostAsync("/auth/logout");
            return response.ToLower() == "true" ? true : false;
        }
        internal static async Task<Stream> PostStreamAsync(string url, string content)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Stream.Null;
            }

            var endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            if (content.Length > 0) request.Content = new StringContent(content, Encoding.UTF8, "application/json");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Stream.Null;
        }
        internal static async Task<Stream> GetStreamAsync(string url)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Stream.Null;
            }

            var endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStreamAsync();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Stream.Null;
        }
        internal static async Task<byte[]> GetByteArrayAsync(string url)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return Array.Empty<byte>();
            }

            var endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsByteArrayAsync();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Array.Empty<byte>();
        }
        internal static async Task<string> GetAsync(string url)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            Uri endpoint = CreateUri(url);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }
        internal static async Task<string> PostAsync(string url, string? jsonObject = null)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            Uri endpoint = CreateUri(url);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);
            request.Content = jsonObject is null ? null : new StringContent(jsonObject, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}");
            }
            return string.Empty;
        }
        internal static async Task<string> PutAsync(string url, string jsonObject)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            Uri endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);
            request.Content = new StringContent(jsonObject, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }
        internal static async Task<string> DeleteAsync(string url)
        {
            if (My.Application.ApiToken == "")
            {
                MessageBox.Show("Anda belum login silakan menutup aplikasi dan membukanya kembali untuk login", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return string.Empty;
            }

            Uri endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);

                response.EnsureSuccessStatusCode();
                return response.StatusCode == System.Net.HttpStatusCode.OK ? await response.Content.ReadAsStringAsync() : string.Empty;
            }
            catch (HttpRequestException ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Request failed: {ex.Message}", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }
        internal static void Dispose()
        {
            lock (lockObj)
            {
                if (!isDisposed)
                {
                    client.Dispose();
                    isDisposed = true;
                }
            }
        }
    }
}
