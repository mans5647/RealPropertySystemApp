using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
using RealPropertySystemApp.models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net;

namespace RealPropertySystemApp.utils
{
    public class RPClient
    {
        private static RPClient ? instance;

        


        private HttpClient httpClient;
        
        public static RPClient GetClient()
        {
            if (instance == null)
            {
                instance = new RPClient();
            }
            return instance;
        }

        private RPClient()
        {
            httpClient = new HttpClient();

            var vars = EnvManager.LoadAll();
            string host = vars["host"];
            string port = vars["port"];
            httpClient.BaseAddress = new Uri($"http://{host}:{port}");
        }

        public async Task<Dictionary<string, object>> Authenticate(string username, string password)
        {
            JObject loginForm = new JObject();
            loginForm.Add("login", username);
            loginForm.Add("password", password);
            
            string json = loginForm.ToString();
            JwtResponse jwtResponse = new JwtResponse();
            int code = AuthCode.AuthErrorFailed;
            

            Dictionary<string, object> values = new Dictionary<string, object>();


            StringContent content = new StringContent(json);
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");
            
            try
            {
                var response = await httpClient.PostAsync("/api/public/login", content);
                string msg = await response.Content.ReadAsStringAsync();

                JObject authResponse = JObject.Parse(msg);

                code = (int)authResponse["code"];
                if (code == GenericCode.NoError)
                {
                    jwtResponse.AccessToken = authResponse["access"].ToObject<string>();
                    jwtResponse.RefreshToken = authResponse["refresh"].ToObject<string>();
                }
            }

            catch (HttpRequestException)
            {
                code = GenericCode.NetError;
            }

            values.Add("code", code);
            values.Add("jwt", jwtResponse);
            return values;
        }

        public async Task<RegistrationResult> CreateAccount(string login, string password, string code)
        {
            RegisterBody body = new RegisterBody();
            body.Login = login;
            body.Password = password;
            body.Code = code;

            string json = JsonConvert.SerializeObject(body);

            StringContent jsonContent = new StringContent(json, Encoding.UTF8, "application/json");
            
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"/api/public/create_account");
            request.Content = jsonContent;
            
            var response = await httpClient.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();

            RegistrationResult registrationResult = new RegistrationResult(true);


            switch (response.StatusCode)
            {
                case HttpStatusCode.BadRequest:
                {
                    JObject commonObject = JObject.Parse(responseContent);
                    if (commonObject.ContainsKey("meta"))
                    {   
                        registrationResult.Code = commonObject["meta"]["code"].ToObject<int>();
                        registrationResult.OptionalMessage = commonObject["meta"]["message"].ToString();
                        registrationResult.Errors = commonObject["errors"].ToObject<List<FieldError>>();
                    }
                    else
                    {
                        registrationResult.Code = commonObject["code"].ToObject<int>();
                        registrationResult.OptionalMessage = commonObject["message"].ToString();
                    }

                    break;
                }
                case HttpStatusCode.Conflict:
                {
                    var authResponse = JsonConvert.DeserializeObject<AuthResponse>(responseContent);
                    registrationResult.Code = authResponse.authCode;
                    registrationResult.OptionalMessage = authResponse.message;
                    break;
                }
                case HttpStatusCode.Created:
                {
                    var userResponseBack = JsonConvert.DeserializeObject<UserModel>(responseContent);
                    registrationResult.Code = GenericCode.NoError;
                    registrationResult.OptionalMessage = "Пользователь создан,\r\nпройдите процедуру авторизации";
                    break;
                }
                
                default:
                    registrationResult = RegistrationResult.Invalid();
                    break;
            }

            return registrationResult;
        }

        public async Task<JwtResponse> RefreshToken(string token)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Post;
            
            requestMessage.Headers.Add("RefreshToken", token);

            var response = await httpClient.SendAsync(requestMessage);

            var content = await response.Content.ReadAsStringAsync();

            var jwtResponse = JsonConvert.DeserializeObject<JwtResponse>(content);
            
            return jwtResponse;
        }

        public async Task <UserModel> GetUserByLogin(string login, string accessToken)
        {
            AuthenticationHeaderValue value = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"/api/resources/users/{login}");
            request.Headers.Authorization = value;

            var responseMessage = await httpClient.SendAsync(request);

            string content = await responseMessage.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<UserModel>(content);
        }

        public async Task<UserModel> UpdateUserByLogin(UserModel model, string accessToken)
        {
            AuthenticationHeaderValue value = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"/api/resources/users/{model.login}");
            request.Headers.Authorization = value;

            UserBody data = new UserBody();


            data.user = model;
            data.roleId = model.userRole.Id;

            var json = JsonConvert.SerializeObject(data);

            StringContent stringContent = new StringContent(json, MediaTypeHeaderValue.Parse("application/json; charset=utf-8"));
            request.Content = stringContent;
            var responseMessage = await httpClient.SendAsync(request);

            string content = await responseMessage.Content.ReadAsStringAsync();


            return JsonConvert.DeserializeObject<UserModel>(content);

        }

    }
}
