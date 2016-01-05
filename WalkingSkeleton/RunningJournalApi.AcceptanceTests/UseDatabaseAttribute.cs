﻿using System;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace RunningJournalApi.AcceptanceTests
{
    class UseDatabaseAttribute : BeforeAfterTestAttribute
    {
        public override void Before(MethodInfo methodUnderTest)
        {
            new Bootstrap().InstallDatabase();
            base.Before(methodUnderTest);
        }

        public override void After(MethodInfo methodUnderTest)
        {
            base.After(methodUnderTest);
            new Bootstrap().UninstallDatabase();
        }
    }
}