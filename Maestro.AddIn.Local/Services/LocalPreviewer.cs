﻿#region Disclaimer / License

// Copyright (C) 2011, Jackie Ng
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

using Maestro.Editors;
using Maestro.Editors.Preview;
using OSGeo.MapGuide;
using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.LayerDefinition;
using OSGeo.MapGuide.ObjectModels.MapDefinition;
using OSGeo.MapGuide.ObjectModels.WatermarkDefinition;
using System;

namespace Maestro.AddIn.Local.Services
{
    public class LocalPreviewer : IResourcePreviewer
    {
        public bool IsPreviewable(IResource res, IServerConnection conn)
        {
            var rt = res.ResourceType;
            return (rt == ResourceTypes.LayerDefinition.ToString() ||
                    rt == ResourceTypes.MapDefinition.ToString() ||
                    rt == ResourceTypes.WatermarkDefinition.ToString());
        }

        /// <summary>
        /// Previews the specified resource
        /// </summary>
        /// <param name="res">The resource to be previewed</param>
        /// <param name="edSvc">The editor service</param>
        public void Preview(IResource res, IEditorService edSvc)
        {
            Preview(res, edSvc, edSvc.PreviewLocale);
        }

        public void Preview(IResource res, IEditorService edSvc, string locale)
        {
            IMapDefinition mapDef = null;
            var conn = edSvc.CurrentConnection;

            if (res.ResourceType == ResourceTypes.LayerDefinition.ToString())
            {
                var ldf = (ILayerDefinition)res;
                string wkt;
                var env = ldf.GetSpatialExtent(conn, true, out wkt);
                if (env == null)
                    throw new ApplicationException(Strings.CouldNotComputeExtentsForPreview);
                mapDef = Utility.CreateMapDefinition(conn, "Preview");
                mapDef.CoordinateSystem = wkt;
                mapDef.SetExtents(env.MinX, env.MinY, env.MaxX, env.MaxY);
                string resId = "Session:" + edSvc.SessionID + "//" + Guid.NewGuid() + "." + res.ResourceType.ToString();
                conn.ResourceService.SetResourceXmlData(resId, ObjectFactory.Serialize(res));
                mapDef.AddLayer(null, "PreviewLayer", resId);
            }
            else if (res.ResourceType == ResourceTypes.MapDefinition.ToString())
            {
                mapDef = (IMapDefinition)res;
            }
            else if (res.ResourceType == ResourceTypes.WatermarkDefinition.ToString())
            {
                string resId = "Session:" + edSvc.SessionID + "//" + Guid.NewGuid() + "." + res.ResourceType.ToString();
                conn.ResourceService.SetResourceXmlData(resId, ObjectFactory.Serialize(res));

                var csFact = new MgCoordinateSystemFactory();
                var arbXY = csFact.ConvertCoordinateSystemCodeToWkt("XY-M");
                mapDef = ObjectFactory.CreateMapDefinition(new Version(2, 3, 0), "Preview");
                mapDef.CoordinateSystem = arbXY;
                mapDef.SetExtents(-100000, -100000, 100000, 100000);
                var wm = ((IMapDefinition2)mapDef).AddWatermark(((IWatermarkDefinition)res));
                wm.ResourceId = resId;
            }

            var mapResId = new MgResourceIdentifier("Session:" + edSvc.SessionID + "//" + mapDef.ResourceType.ToString() + "Preview" + Guid.NewGuid() + "." + mapDef.ResourceType.ToString());
            conn.ResourceService.SetResourceXmlData(mapResId.ToString(), ObjectFactory.Serialize(mapDef));

            //MgdMap map = new MgdMap(mapResId);

            //var diag = new MapPreviewWindow(map, conn);
            //diag.ShowDialog();

            var diag = new UI.MapPreviewWindow(conn);
            diag.Init(mapResId);
            diag.ShowDialog();
        }
    }
}