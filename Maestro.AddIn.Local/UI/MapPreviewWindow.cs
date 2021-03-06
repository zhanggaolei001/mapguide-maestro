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

using OSGeo.MapGuide;
using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.Viewer;
using OSGeo.MapGuide.Viewer.Desktop;
using System;
using System.Windows.Forms;

namespace Maestro.AddIn.Local.UI
{
    public partial class MapPreviewWindow : Form, IMapStatusBar
    {
        private IServerConnection _conn;

        public MapPreviewWindow(IServerConnection conn)
        {
            InitializeComponent();
            _conn = conn;

            var cntrl = new MapViewerController(viewer, legend, this, propertyPane, toolbar);
            this.Disposed += OnDisposed;
        }

        private MgdMap _map;

        public void Init(MgResourceIdentifier mapResId)
        {
            _map = new MgdMap(mapResId);
            var groups = _map.GetLayerGroups();
            if (groups != null && groups.GetCount() > 0)
            {
                for (int i = 0; i < groups.GetCount(); i++)
                {
                    var grp = groups.GetItem(i);
                    if (grp.LayerGroupType == MgLayerGroupType.BaseMap)
                    {
                        MessageBox.Show(Strings.TiledLayerSupportWarning);
                        break;
                    }
                }
            }
            var fact = new MgdServiceFactory();
            viewer.Init(new MgDesktopMapViewerProvider(_map));
            viewer.RefreshMap();
        }

        private void OnDisposed(object sender, EventArgs e)
        {
            if (_map != null)
            {
                _map.Dispose();
                _map = null;
            }
        }

        public void SetCursorPositionMessage(string message)
        {
            lblCoordinates.Text = message;
        }

        public void SetFeatureSelectedMessage(string message)
        {
            lblFeaturesSelected.Text = message;
        }

        public void SetMapScaleMessage(string message)
        {
            lblScale.Text = message;
        }

        public void SetMapSizeMessage(string message)
        {
            lblMapSize.Text = message;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            var diag = new ZoomScaleDialog();
            if (diag.ShowDialog() == DialogResult.OK)
            {
                viewer.ZoomToScale(diag.Value);
            }
        }
    }
}