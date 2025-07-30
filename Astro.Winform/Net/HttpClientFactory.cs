using Astro.Models;
using Astro.Winform.Net;
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
                if (response.IsSuccessStatusCode)
                {
                    using (var stream = await response.Content.ReadAsStreamAsync())
                    using (var reader = new IO.Reader(stream))
                    {
                        My.Application.User = new UserInfo(reader.ReadInt16(), reader.ReadString(), new Role() { Id = reader.ReadInt16(), Name = reader.ReadString() });
                        My.Application.ApiToken = reader.ReadString();
                        return true;
                    }
                }

                var statusCode = response.StatusCode;
                switch (statusCode)
                {
                    case System.Net.HttpStatusCode.Unauthorized:
                        MessageBox.Show("Invalid username or password", "Unauthorized", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case System.Net.HttpStatusCode.Forbidden:
                        MessageBox.Show("Your password has expired. Please contact your system administrator to reset it.", "Password expired", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null)
                        {
                            MessageBox.Show(problem.Detail, "Internal Server Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unknown error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStreamAsync();
                
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Stream.Null;
        }
        internal static async Task<string> UploadDocument(string url, byte[] file)
        {
            var endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            if (file.Length > 0)
            {
                request.Content = new ByteArrayContent(file);
            }
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return string.Empty;
        }
        internal static async Task<Stream> PostStreamAsync(string url, byte[] content)
        {
            var endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            if (content.Length > 0) request.Content = new ByteArrayContent(content);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                HttpResponseMessage response = await Instance.SendAsync(request);
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStreamAsync();

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Stream.Null;
        }
        internal static async Task<Stream> GetStreamAsync(string url)
        {
            var endpoint = CreateUri(url);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, endpoint);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", My.Application.ApiToken);

            try
            {
                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    HttpResponseMessage response = await Instance.SendAsync(request, cts.Token);
                    if (response.IsSuccessStatusCode) return await response.Content.ReadAsStreamAsync();

                    switch (response.StatusCode)
                    {
                        case System.Net.HttpStatusCode.InternalServerError:
                            var problem = await response.GetProbemDetails();
                            if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Unknown Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return Stream.Null;
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
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();

                var statusCode = response.StatusCode;
                if (statusCode == System.Net.HttpStatusCode.InternalServerError)
                {
                    var problem = await response.GetProbemDetails();
                    if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unkknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();
                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                if (response.IsSuccessStatusCode) return await response.Content.ReadAsStringAsync();

                switch (response.StatusCode)
                {
                    case System.Net.HttpStatusCode.InternalServerError:
                        var problem = await response.GetProbemDetails();
                        if (problem != null) MessageBox.Show(problem.Detail, "Internal server error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        break;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unknown error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
