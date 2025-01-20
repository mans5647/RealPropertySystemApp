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
using Microsoft.VisualBasic;
using System.Collections.ObjectModel;

namespace RealPropertySystemApp.utils
{
    public class RPClient
    {
        private static RPClient ? instance;

        private static ILoggerService logger = LoggerServiceFactory
            .CreateFileLogger(typeof(RPClient));


        private HttpClient httpClient;
        
        public static RPClient GetClient()
        {
            if (instance == null)
            {
                instance = new RPClient();
                logger.info("API client initialized");
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
            
            RegistrationResult registrationResult = new RegistrationResult(true);
            try
            {
                var response = await httpClient.SendAsync(request);

                string responseContent = await response.Content.ReadAsStringAsync();



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
            }
            catch (HttpRequestException)
            {
                registrationResult = RegistrationResult.Invalid();
            }

            return registrationResult;
        }

        public async Task<JwtResponse> RefreshToken(string token)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage();
            requestMessage.Method = HttpMethod.Post;
            requestMessage.RequestUri = new Uri("/api/public/refresh_token", UriKind.Relative);
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
            data.passportId = (model.passport != null) ? model.passport.Id : -1;

            var json = JsonConvert.SerializeObject(data);

            StringContent stringContent = new StringContent(json, MediaTypeHeaderValue.Parse("application/json; charset=utf-8"));
            request.Content = stringContent;
            var responseMessage = await httpClient.SendAsync(request);
            var sc = Status(responseMessage);

            switch (sc)
            {
                case HttpStatusCode.BadRequest:
                    
                    string c = await Content(responseMessage);

                    ValidationResponse response = JsonConvert.DeserializeObject<ValidationResponse>(c);
                    throw new InvalidUserBodyException(response, "Ошибка в полях");

            }


            string content = await responseMessage.Content.ReadAsStringAsync();
            
            return JsonConvert.DeserializeObject<UserModel>(content);

        }

        public async Task<bool> CreateUser(UserModel model)
        {

            string uri = "/api/resources/users/add";

            string jdata = JsonConvert.SerializeObject(model);



            var req = BuildRequest(Session.GetCurrent().getAT(), HttpMethod.Post, BuildContent(jdata), uri);

            var resp = await httpClient.SendAsync(req);

            var sc = Status(resp);

            switch (sc)
            {
                case HttpStatusCode.OK: return true;
                case HttpStatusCode.Conflict :
                {
                    throw new InvalidOperationException($"Пользователь {model.login} уже существует");
                }
                case HttpStatusCode.BadRequest:
                {
                    string j = await Content(resp);
                        ValidationResponse validationResponse = JsonConvert.DeserializeObject<ValidationResponse>(j);
                        throw new InvalidUserBodyException(validationResponse, "Ошибка при создании пользователя");
                }
            }
            

            return false;
        }

        public async Task<bool> DeleteUser(long uid)
        {
            string uri = $"/api/resources/users/delete/{uid}";

            var req = BuildRequest(Session.GetCurrent().getAT(), HttpMethod.Delete, null, uri);

            var resp = await httpClient.SendAsync(req);

            var sc = Status(resp);

            switch (sc)
            {
                case HttpStatusCode.OK: return true;
                case HttpStatusCode.NotFound:
                    {
                        throw new InvalidOperationException($"Пользователь не существует");
                    }
            }


            return false;
        }

        public async Task<List<ServerLog>> GetLogs(string accessToken, int page, int size = 5)
        {
            string uri = $"/api/resources/logs/ch?page={page}&size={size}";

            var req = BuildRequest(accessToken, HttpMethod.Get, null, uri);


            try
            {
                var resp = await httpClient.SendAsync(req);

                string c = await Content(resp);

                return JsonConvert.DeserializeObject<List<ServerLog>>(c);
            }

            catch (Exception ex)
            {

            }

            return new List<ServerLog>();
        }
        
        public async Task<Passport> AddPassport(Passport value)
        {
            string uri = $"/api/resources/passports/add";
            HttpContent content = BuildContent(JsonConvert.SerializeObject(value));
            
            var req = BuildRequest(Session.GetCurrent().Jwt.AccessToken, HttpMethod.Post, content, uri);
            
            try
            {
                string c = await DoSendGetContent(req, httpClient);
                return JsonConvert.DeserializeObject<Passport>(c);
            }

            catch (Exception)
            {

            }

            return Passport.Invalid();
        }


        public async Task<Passport> SavePassport(Passport value)
        {
            string uri = $"/api/resources/passports/upd/{value.Id}";
            HttpContent content = BuildContent(JsonConvert.SerializeObject(value));

            var req = BuildRequest(Session.GetCurrent().Jwt.AccessToken, HttpMethod.Put, content, uri);

            try
            {
                string c = await DoSendGetContent(req, httpClient);
                return JsonConvert.DeserializeObject<Passport>(c);
            }

            catch (Exception)
            {

            }

            return Passport.Invalid();
        }

        public async Task<Passport> GetPassport(long id)
        {
            string uri = $"/api/resources/passports/get/{id}";
            
            var req = BuildRequest(Session.GetCurrent().Jwt.AccessToken, HttpMethod.Get, null, uri);

            try
            {
                string c = await DoSendGetContent(req, httpClient);
                return JsonConvert.DeserializeObject<Passport>(c);
            }

            catch (Exception)
            {

            }

            return Passport.Invalid();
        }



        public async Task<int> GetTotalPagesOfLogs(string token)
        {
            string uri = "/api/resources/logs/total";

            var req = BuildRequest(token, HttpMethod.Get,null, uri);

            try
            {
                var resp = await httpClient.SendAsync(req);

                string c = await Content(resp);

                JObject j = JObject.Parse(c);

                return j["value"].ToObject<int>();

            }

            catch (Exception ex)
            {

            }

            return 0;

        }

        public async Task<long> GetTotalCountOfUsers()
        {
            string uri = "/api/resources/users/total";
            var req = BuildRequest(Session.GetCurrent().getAT(), HttpMethod.Get, null, uri);

            try
            {
                string c = await DoSendGetContent(req, httpClient);
                JObject j = JObject.Parse(c);

                return j["value"].ToObject<long>();
            }
            catch (Exception)
            {

            }

            return -1;
        }

        public async Task<List<UserModel>> GetUsersChunked(long page, long size)
        {
            string uri = $"/api/resources/users/ch?page={page}&size={size}";
            
            var req = BuildRequest(Session.GetCurrent().getAT(), HttpMethod.Get, null, uri);

            try
            {
                string c = await DoSendGetContent(req, httpClient);

                return GetListableOfType<UserModel>(c);

            }
            catch (Exception)
            {

            }

            return new List<UserModel>();
        }

        public async Task<List<UserModel>> GetUsersChunked(long page, long size, List<long> ids)
        {
            string uri = $"/api/resources/users/ch?page={page}&size={size}";

            if (ids != null && ids.Count > 0)
            {
                uri += "&except_ids=";

                foreach (var i in ids)
                {
                    uri += $"{i},";
                }

                uri = uri.Substring(0, uri.Length - 1);
            }

            var req = BuildRequest(Session.GetCurrent().getAT(), HttpMethod.Get, null, uri);

            try
            {
                string c = await DoSendGetContent(req, httpClient);

                return GetListableOfType<UserModel>(c);

            }
            catch (Exception)
            {

            }

            return new List<UserModel>();
        }

        public async Task<List<UserModel>> GetUsersChunkedAndFiltered(long CurrentPage, long Count, List<long> ids
            , UserFilterBody filter)
        {

            string uri = $"/api/resources/users/filtered?page={CurrentPage}&size={Count}";

            if (ids != null && ids.Count > 0)
            {
                uri += "&except_ids=";

                foreach (var i in ids)
                {
                    uri += $"{i},";
                }

                uri = uri.Substring(0, uri.Length - 1);
            }

            if (filter.Firstname != null &&  filter.Firstname.Length > 0)
            {
                uri += $"&firstname={filter.Firstname}";
            }

            if (filter.Lastname != null && filter.Lastname .Length > 0)
            {
                uri += $"&lastname={filter.Lastname}";
            }

            if (filter.DateMin != DateTime.MinValue)
            {
                uri += $"&birthDateMin={filter.DateMin.ToShortDateString()}";
            }

            if (filter.DateMax != DateTime.MaxValue)
            {
                uri += $"&birthDateMax={filter.DateMax.ToShortDateString()}";
            }

            if (filter.Rolename != null && filter.Rolename.Length > 0)
            {
                uri += $"&role={filter.Rolename}";
            }

            var req = BuildRequest(Session.GetCurrent().getAT(), HttpMethod.Get, null, uri);

            try
            {
                string c = await DoSendGetContent(req, httpClient);

                return GetListableOfType<UserModel>(c);

            }
            catch (Exception)
            {

            }

            return new List<UserModel>();
        }

        private static HttpRequestMessage BuildRequest(string accessToken, HttpMethod method, HttpContent? content, string relativeUri)
        {
            var request = new HttpRequestMessage();
            request.Method = method;
            
            if (content != null)
            {
                request.Content = content;
            }

            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
            request.RequestUri = new Uri(relativeUri, UriKind.Relative);

            return request;
        }


        private static List<T> GetListableOfType<T>(string c)
        {
            return JsonConvert.DeserializeObject<List<T>>(c);
        }
        
        private static HttpContent BuildContent(string data)
        {
            return new StringContent(data, Encoding.UTF8, MediaTypeHeaderValue.Parse("application/json"));
        }

        private static async Task<string> DoSendGetContent(HttpRequestMessage msg, HttpClient c)
        {
            var res = await c.SendAsync(msg);
            return await Content(res);
        }

        private static async Task<string> Content(HttpResponseMessage message)
        {
            return await message.Content.ReadAsStringAsync();
        }

        private static HttpStatusCode Status(HttpResponseMessage message)
        {
            return message.StatusCode;
        }
    }
}
