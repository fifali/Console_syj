using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ConsoleHydee
{
    public static class Enum
    {
        #region 枚举
        public enum addstatus
        {
            未关注 = 0,
            已关注 = 1
        }
        public enum userstatus
        {
            启用 = 1,
            停用 = 2,
            注销 = 3,
            禁用 = 4
        }
        public enum addtype
        {
            个人 = 1,
            企业 = 2
        }
        public enum companystatus
        {
            启用 = 1,
            停用 = 2,
            注销 = 3,
            禁用 = 4
        }
        public enum userautstatus
        {
            未认证 = 1,
            认证通过 = 2,
            认证未通过 = 3
        }
        public enum companyautstatus
        {
            未认证 = 1,
            认证通过 = 2,
            认证未通过 = 3
        }
        public enum usercardstatus
        {
            未认证 = 0,
            已认证 = 1
        }
        #endregion
    }
}