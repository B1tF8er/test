using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using App.Wpf.Model;
using Newtonsoft.Json;

namespace AppWpf.Model
{
    public class DataService : IDataService
    {
        private static HttpClient _httpClient;

        public DataService()
        {
            InitializeHttpClient();
        }

        private void InitializeHttpClient() {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _httpClient.BaseAddress = new Uri("http://localhost:50933/api/");
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync("Users"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadAsAsync<IEnumerable<User>>();

                    return users;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            } 
        }

        public async Task<User> GetUser(int userId)
        {
            using (HttpResponseMessage response = await _httpClient.GetAsync($"Users/{userId}"))
            {
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadAsAsync<User>();

                    return user;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task<User> CreateUser(User user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            var userStringContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            using (HttpResponseMessage response = await _httpClient.PostAsync($"Users", userStringContent))
            {
                if (response.IsSuccessStatusCode)
                {
                    var users = await response.Content.ReadAsAsync<User>();

                    return users;
                }
                else
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task UpdateUser(User user)
        {
            var userJson = JsonConvert.SerializeObject(user);
            var userStringContent = new StringContent(userJson, UnicodeEncoding.UTF8, "application/json");

            using (HttpResponseMessage response = await _httpClient.PutAsync($"Users/{user.Id}", userStringContent))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }

        public async Task DeleteUser(int userId)
        {
            using (HttpResponseMessage response = await _httpClient.DeleteAsync($"Users/{userId}"))
            {
                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(response.ReasonPhrase);
                }
            }
        }
    }
}