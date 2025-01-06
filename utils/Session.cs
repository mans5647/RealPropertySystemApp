using Newtonsoft.Json;
using RealPropertySystemApp.bodies;
using RealPropertySystemApp.codes;
using RealPropertySystemApp.models;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RealPropertySystemApp.utils
{
    internal class TimeCamparer : IComparer<Session>
    {
        public int Compare(Session x, Session y)
        {
            DateTime f = x.LastLoginTime;
            DateTime g = y.LastLoginTime;
            if (f >= g) return -1;
            else if (f <= g) return 1;
            return 0;
        }
    }
    public class Session
    {
        private JwtResponse _jwtResponse;
        private UserModel _userModel;
        [JsonIgnore]
        private TimeSpan _timeLeft;
        private DateTime _sessionExpireTime;
        private DateTime _lastLoginTime;
        private bool _last;

        [JsonIgnore] private static Session? current;
        [JsonIgnore] private static List<Session> sessions;
        [JsonIgnore] private const string SaveFileName = "sessions.dat";
        [JsonIgnore] private const string SaveDirName = "data";
        [JsonIgnore] private const string EncryptKey = "n_/>|W}Q*3qTNxU*gt{AiTfy[:XxoM*;";
        [JsonIgnore] private const string IVKey = "tR7nR6wZHGjYMCuV";
        


        public JwtResponse Jwt
        {
            get { return _jwtResponse; }
            set { _jwtResponse = value; }
        }

        public UserModel CurrentUser
        {
            get { return _userModel; }
            set { _userModel = value; }
        }

        public TimeSpan TimeLeft
        {
            get => _timeLeft;
            set { _timeLeft = value; }
        }

        public DateTime SessionExpireTime
        {
            get => _sessionExpireTime;
            set { _sessionExpireTime = value; }
        }

        public DateTime LastLoginTime
        {
            get => _lastLoginTime;
            set { _lastLoginTime = value; }
        }

        public bool Last
        {
            get => _last;
            set { _last = value; }
        }

        public static void SetCurrent(Session value)
        {
            current = value;
        }


        public static Session GetCurrent() => current;

        
        public bool isSessionExpired()
        {
            return TimeLeft == TimeSpan.Zero || DateTime.Now >= SessionExpireTime;
        }

        public string GetLogin()
        {
            return CurrentUser.login;
        }
        
        public string GetPassword()
        {
            return CurrentUser.password;
        }

        public string GetFirstname()
        {
            return CurrentUser.firstName;
        }
        public string GetRoleSuffix()
        {
            return CurrentUser.userRole.Suffix;
        }

        public Session()
        {

        }

        private Session(UserModel u, JwtResponse r)
        {
            CurrentUser = u;
            Jwt = r;
            TimeLeft = TimeSpan.MinValue;
        }

        public static Session FromData(UserModel u, JwtResponse r)
        {
            return new Session(u, r);
        }

        public static void LoadAllFromFile(ref ContentStatus stat)
        {
            stat = ReloadAll();
        }

        public static void ClearAll()
        {
            sessions.Clear();
            File.WriteAllText(FullyPathToFile(), string.Empty);
        }

        public static Session findByMaxLastLogin()
        {
            var tc = new TimeCamparer();
            sessions.Sort(tc);

            return sessions.First();
        }

        public static void SaveAll()
        {
            if (!isSessionDirExists())
            {
                Directory.CreateDirectory(FullyPathToDir());
            }

            if (!isSessionFileExists())
            {
                File.Create(FullyPathToFile());
            }

            string jData = JsonConvert.SerializeObject(sessions);

            byte[] jDataRaw = Encoding.UTF8.GetBytes(jData);

            byte[] ivRaw = Encoding.UTF8.GetBytes(IVKey);
            byte[] keyRaw = Encoding.UTF8.GetBytes(EncryptKey);
            byte[] encrypted = AesManager.Encrypt(jData, ivRaw, keyRaw);

            File.WriteAllBytes(FullyPathToFile(), encrypted);
        }

        public static void Add(Session session)
        {
            sessions.Add(session);
        }

        public static int indexOf(Session session)
        {
            return sessions.FindIndex((s) =>
            {
                return session.GetLogin().Equals(s.GetLogin());
            });
        }

        public static void RewriteAt(Session session, int index)
        {
            sessions[index] = session;
        }

        private static ContentStatus ReloadAll()
        {
            if (sessions != null)
            {
                sessions.Clear();
            }
            
            if (isSessionFileExists())
            {
                if (isFileEmpty())
                {
                    sessions = new List<Session>();
                    return ContentStatus.Empty;
                }
                
                else
                {
                    var encData = File.ReadAllBytes(FullyPathToFile());
                    var bytesIV = Encoding.UTF8.GetBytes(IVKey);
                    var bytesKey = Encoding.UTF8.GetBytes(EncryptKey);

                    string jData = AesManager.Decrypt(encData, bytesIV, bytesKey);
                    
                    try
                    {
                        sessions = JsonConvert.DeserializeObject<List<Session>>(jData);
                        return ContentStatus.OK;
                    }

                    catch (JsonException)
                    {
                        return ContentStatus.InvalidFormat;
                    }

                    catch (Exception)
                    {
                        return ContentStatus.Unknown;
                    }
                }
                
            }
            else
            {
                sessions = new List<Session>();
            }

            return ContentStatus.NotExist;
        }

        public static bool IsSuchLoginExists(string login)
        {
            return sessions.Exists((s1) =>
            {
                return s1.CurrentUser.login.Equals(login);
            });
        }

        private static bool isSessionDirExists()
        {
            string path = FullyPathToDir();
            return Directory.Exists(path);
        }

        public static bool isSessionFileExists()
        {
            string path = FullyPathToFile();
            return File.Exists(path);
        }

        private static string FullyPathToDir()
        {
            return $"{Environment.CurrentDirectory}\\{SaveDirName}";
        }

        private static string FullyPathToFile()
        {
            return $"{Environment.CurrentDirectory}\\{SaveDirName}\\{SaveFileName}";
        }
        public static bool isFileEmpty()
        {
            return string.IsNullOrEmpty(File.ReadAllText(FullyPathToFile()));
        }

        public static List<Session> Source()
        {
            return sessions;
        }

        
    }
}
