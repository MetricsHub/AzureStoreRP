/*
* Copyright 2013 MetricsHub, Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*   http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*
*/

namespace AzureStoreRP.Website.Controllers
{
    using AzureStoreRP.Data.Authorization;
    using AzureStoreRP.Data.Extensions;
    using AzureStoreRP.Data.Providers.Account;
    using log4net;
    using System;
    using System.Net;
    using System.Web.Mvc;
    using System.Web.Security;

    public class HomeController : Controller
    {
        #region Logger

        private static readonly ILog Logger = LogManager.GetLogger(typeof(HomeController));

        #endregion

        [AllowAnonymous]
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return this.Content("Welcome, " + User.Identity.Name);
            }
            else
            {
                return this.Content("You are not authenticated. Please login via Azure Store.");
            }
        }
    }
}