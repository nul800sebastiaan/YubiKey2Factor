//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using umbraco.businesslogic;
//using umbraco.interfaces;
//using Umbraco.Core.Services;

//[Application("2_Step_Verification", "TwoFactorAuthentication", "icon-firewall", 1)]
//    public class TwoFactorAuthenticationSection : IApplication
//{
//    private void AddSectionSevenSeven(string alias)
//    {
//        IUserService _service=null;
//        var adminGroup = _service.GetUserGroupByAlias("2_Step_Verification");
//        if (adminGroup != null)
//        {
//            if (!adminGroup.AllowedSections.Contains(alias))
//            {
//                adminGroup.AddAllowedSection(alias);
//                _service.Save(adminGroup);
//            }
//        }
//    }
//}
