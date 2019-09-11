﻿using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PWApplication.MobileShared.Models.User;

namespace PWApplication.MobileShared.Services.UserInfo
{
    public class MockUserInfoService : IUserInfoService
    {
        public Task<ObservableCollection<UserInfoSimple>> FindUser(string authToken, string username)
        {
            Task.Delay(500);
            var list = new ObservableCollection<UserInfoSimple>();
            for (int i = 1; i < 21; i++)
            {
                list.Add(new UserInfoSimple()
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = $"demouser{i}@microsoft.com",
                    FullName = "John Wick",
                    UserName = $"demouser{i}@microsoft.com"                    
                });
            }

            return Task.FromResult(list);
        }

        public Task<UserInfoSimple> GetUserInfo(string authToken, string userId)
        {
            return Task.FromResult(new UserInfoSimple()
            {
                UserId = userId,
                Email = "demouser@microsoft.com",
                FullName = "John Wick",
                UserName = "demouser@microsoft.com"
            });
        }
    }
}
