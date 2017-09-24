﻿#region Disclaimer / License

// Copyright (C) 2010, Jackie Ng
// https://github.com/jumpinjackie/mapguide-maestro
//
// This library is free software; you can redistribute it and/or
// modify it under the terms of the GNU Lesser General Public
// License as published by the Free Software Foundation; either
// version 2.1 of the License, or (at your option) any later version.
//
// This library is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
// Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public
// License along with this library; if not, write to the Free Software
// Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
//

#endregion Disclaimer / License

using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.MaestroAPI.Services;
using OSGeo.MapGuide.MaestroAPI.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Xunit;

namespace MaestroAPITests
{
    public class HttpConnectionFixture : ConnectionTestBaseFixture
    {
        protected override bool ShouldIgnore(out string reason)
        {
            reason = string.Empty;
            if (TestControl.IgnoreHttpConnectionTests)
                reason = "Skipping HttpConnectionTests because TestControl.IgnoreHttpConnectionTests = true";

            return TestControl.IgnoreHttpConnectionTests;
        }

        public override IServerConnection CreateTestConnection()
        {
            return ConnectionUtil.CreateTestHttpConnection();
        }
    }

    public class HttpConnectionTests : ConnectionTestBase<HttpConnectionFixture>
    {
        public HttpConnectionTests(HttpConnectionFixture fixture) 
            : base(fixture)
        {
        }

        protected override string GetTestPrefix()
        {
            return "Http";
        }

        //[Fact]
        public override void TestEncryptedFeatureSourceCredentials()
        {
            base.TestEncryptedFeatureSourceCredentials();
        }

        [SkippableFact]
        public override void TestResourceExists()
        {
            base.TestResourceExists();
        }

        [SkippableFact]
        public void TestFeatureSourceCaching()
        {
            base.TestFeatureSourceCaching("HttpCaching");
        }

        [SkippableFact]
        public void TestClassDefinitionCaching()
        {
            base.TestClassDefinitionCaching("HttpCaching");
        }

        [SkippableFact]
        public override void TestTouch()
        {
            base.TestTouch();
        }

        [SkippableFact]
        public override void TestAnyStreamInput()
        {
            base.TestAnyStreamInput();
        }

        [SkippableFact]
        public override void TestCreateFromExistingSession()
        {
            base.TestCreateFromExistingSession();
        }

        [SkippableFact]
        public override void TestSchemaMapping()
        {
            base.TestSchemaMapping();
        }

        [SkippableFact]
        public override void TestCreateRuntimeMapWithInvalidLayersErrorsDisabled()
        {
            base.TestCreateRuntimeMapWithInvalidLayersErrorsDisabled();
        }

        [SkippableFact]
        public override void TestCreateRuntimeMapWithInvalidLayersErrorsEnabled()
        {
            base.TestCreateRuntimeMapWithInvalidLayersErrorsEnabled();
        }

        [SkippableFact]
        public override void TestQueryLimits()
        {
            base.TestQueryLimits();
        }

        [SkippableFact]
        public void TestCase2432()
        {
            Skip.If(_fixture.Skip, _fixture.SkipReason);

            var conn = _fixture.CreateTestConnection();

            //Problematic method: Cause of original ticket
            for (int i = 0; i < 10; i++)
            {
                using (var rdr1 = conn.FeatureService.QueryFeatureSource("Library://UnitTests/Data/Parcels.FeatureSource", "SHP_Schema:Parcels", "Autogenerated_SDF_ID < 20"))
                {
                    var count = 0;
                    while (count < 5 && rdr1.ReadNext())
                    {
                        count++;
                    }
                    rdr1.Close();
                }
            }

            //Normal sequential method
            for (int i = 0; i < 10; i++)
            {
                using (var rdr1 = conn.FeatureService.QueryFeatureSource("Library://UnitTests/Data/Parcels.FeatureSource", "SHP_Schema:Parcels", "Autogenerated_SDF_ID < 20"))
                {
                    while (rdr1.ReadNext()) { }
                    rdr1.Close();
                }
            }

            //Multi-threaded method
            var events = new List<ManualResetEvent>();
            for (int i = 0; i < 10; i++)
            {
                var resetEvent = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem((args) =>
                {
                    using (var rdr1 = conn.FeatureService.QueryFeatureSource("Library://UnitTests/Data/Parcels.FeatureSource", "SHP_Schema:Parcels", "Autogenerated_SDF_ID < 20"))
                    {
                        while (rdr1.ReadNext()) { }
                        rdr1.Close();
                        resetEvent.Set();
                    }
                });
                events.Add(resetEvent);
            }
            WaitHandle.WaitAll(events.ToArray());
        }

        public override IServerConnection CreateFromExistingSession(IServerConnection orig)
        {
            return ConnectionProviderRegistry.CreateConnection("Maestro.Http",
                HttpServerConnectionParams.PARAM_URL, orig.GetCustomProperty(HttpServerConnectionProperties.PROP_BASE_URL).ToString(),
                HttpServerConnectionParams.PARAM_SESSION, orig.SessionID);
        }

        [SkippableFact]
        public void TestSheboyganCsToPseudoMercator()
        {
            Skip.If(_fixture.Skip, _fixture.SkipReason);
            //Purpose: Unit test to guard against regression as a result of updating/replacing NTS

            var conn = _fixture.CreateTestConnection();
            var srcWkt = "GEOGCS[\"WGS84 Lat/Long's, Degrees, -180 ==> +180\",DATUM[\"D_WGS_1984\",SPHEROID[\"World_Geodetic_System_of_1984\",6378137,298.257222932867]],PRIMEM[\"Greenwich\",0],UNIT[\"Degree\",0.017453292519943295]]";
            var dstCs = conn.CoordinateSystemCatalog.FindCoordSys("WGS84.PseudoMercator");
            var dstWkt = dstCs.WKT;

            var trans = conn.CoordinateSystemCatalog.CreateTransform(srcWkt, dstWkt);

            double tx1;
            double ty1;
            trans.Transform(-87.7649869909628, 43.6913981287878, out tx1, out ty1);
            Assert.Equal(-9769953.66131227, tx1, 7); // 0.0000001);
            Assert.Equal(5417808.88017179, ty1, 7); // 0.0000001);

            double tx2;
            double ty2;

            trans.Transform(-87.6955215108997, 43.7975200004803, out tx2, out ty2);
            Assert.Equal(-9762220.79944393, tx2, 7); // 0.0000001);
            Assert.Equal(5434161.22418638, ty2, 7); // 0.0000001);
        }

        [SkippableFact]
        public void TestCustomProperties()
        {
            Skip.If(_fixture.Skip, _fixture.SkipReason);

            var conn = CreateHttpConnection("http://tempuri.org", "en");

            //Work through the interface
            IServerConnection isvc = (IServerConnection)conn;

            //UserAgent is exposed as a custom property
            var props = isvc.GetCustomPropertyNames();

            Assert.NotNull(props);
            Assert.Equal(props.Length, 2);
            Assert.True(Array.IndexOf<string>(props, HttpServerConnectionProperties.PROP_USER_AGENT) >= 0);
            Assert.True(Array.IndexOf<string>(props, HttpServerConnectionProperties.PROP_BASE_URL) >= 0);

            //It is of type string
            var type = isvc.GetCustomPropertyType(HttpServerConnectionProperties.PROP_USER_AGENT);
            Assert.Equal(type, typeof(string));
            type = isvc.GetCustomPropertyType(HttpServerConnectionProperties.PROP_BASE_URL);
            Assert.Equal(type, typeof(string));

            //We can set and get it
            isvc.SetCustomProperty(HttpServerConnectionProperties.PROP_USER_AGENT, "MapGuide Maestro API Unit Test Fixture");
            var agent = (string)isvc.GetCustomProperty(HttpServerConnectionProperties.PROP_USER_AGENT);
            Assert.Equal(agent, "MapGuide Maestro API Unit Test Fixture");

            //BaseUrl is read-only
            try
            {
                isvc.SetCustomProperty(HttpServerConnectionProperties.PROP_BASE_URL, "http://mylocalhost/mapguide");
                Assert.True(false, "Should've thrown exception");
            }
            catch { }
        }

        static IServerConnection CreateHttpConnection(string uri, string locale, Version ver = null)
        {
            var asm = typeof(IServerConnection).Assembly;
            var reqBuilderType = asm.GetTypes().FirstOrDefault(t => t.Name == "RequestBuilder");

            var reqBuilderCtor = reqBuilderType.GetInternalConstructor(new[] { typeof(Uri), typeof(string) });
            var reqBuilder = reqBuilderCtor.Invoke(new object[] { new Uri(uri), locale });

            var httpConnType = asm.GetTypes().FirstOrDefault(t => t.Name == "HttpServerConnection");

            var httpConnCtor = httpConnType.GetInternalConstructor(new[] { reqBuilderType });
            var conn = httpConnCtor.Invoke(new[] { reqBuilder }) as IServerConnection;
            
            if (ver != null)
            {
                var method = conn.GetType().GetMethod("SetSiteVersion", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                method.Invoke(conn, new[] { ver });
            }

            return conn;
        }

        [SkippableFact]
        public void TestServiceCapabilities()
        {
            Skip.If(_fixture.Skip, _fixture.SkipReason);

            var conn = CreateHttpConnection("http://tempuri.org", "en", new Version(1, 2, 0));

            //Work through the interface
            IServerConnection isvc = (IServerConnection)conn;
            int[] stypes = isvc.Capabilities.SupportedServices;
            foreach (int st in stypes)
            {
                try
                {
                    IService svc = isvc.GetService(st);
                }
                catch
                {
                    Assert.True(false, "Supported service type mismatch");
                }
            }

            var method = conn.GetType().GetMethod("SetSiteVersion", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.Invoke(conn, new[] { new Version(2, 0, 0) });

            stypes = isvc.Capabilities.SupportedServices;
            foreach (int st in stypes)
            {
                try
                {
                    IService svc = isvc.GetService(st);
                }
                catch
                {
                    Assert.True(false, "Supported service type mismatch");
                }
            }
        }
    }
}