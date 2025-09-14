using System.Collections.Concurrent;

namespace HelloWorld.Authentication.Services
{
    public class UserLoginService
    {
        private readonly ConcurrentDictionary<string, string> _loginData;
        public UserLoginService()
        {
            _loginData = new();
            _loginData.TryAdd("Vijay", "password@123");
        }

        public bool ValidateCredentials(string username, string password)
        {
            return _loginData.TryGetValue(username, out var pwd) && pwd == password;
        }
    }
}