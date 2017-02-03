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

using ICSharpCode.Core;
using Maestro.Base.Services;
using Maestro.Editors.Generic;
using OSGeo.MapGuide.ObjectModels;

namespace Maestro.Base.Commands.Test
{
    internal class OpenResourceCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var wb = Workbench.Instance;
            var exp = wb.ActiveSiteExplorer;
            var mgr = ServiceRegistry.GetService<ServerConnectionManager>();
            var conn = mgr.GetConnection(exp.ConnectionName);

            var picker = new ResourcePicker(conn, ResourcePickerMode.OpenResource);
            if (picker.ShowDialog(wb) == System.Windows.Forms.DialogResult.OK)
            {
                MessageService.ShowMessage(picker.ResourceID);
            }
            else
            {
                MessageService.ShowMessage(Strings.Cancelled);
            }
        }
    }

    internal class OpenResourceFromStartingPointCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var wb = Workbench.Instance;
            var exp = wb.ActiveSiteExplorer;
            var mgr = ServiceRegistry.GetService<ServerConnectionManager>();
            var conn = mgr.GetConnection(exp.ConnectionName);

            var picker = new ResourcePicker(conn, ResourcePickerMode.OpenResource);
            picker.SetStartingPoint("Library://Samples/Sheboygan/"); //NOXLATE
            if (picker.ShowDialog(wb) == System.Windows.Forms.DialogResult.OK)
            {
                MessageService.ShowMessage(picker.ResourceID);
            }
            else
            {
                MessageService.ShowMessage(Strings.Cancelled);
            }
        }
    }

    internal class OpenFolderCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var wb = Workbench.Instance;
            var exp = wb.ActiveSiteExplorer;
            var mgr = ServiceRegistry.GetService<ServerConnectionManager>();
            var conn = mgr.GetConnection(exp.ConnectionName);

            var picker = new ResourcePicker(conn, ResourcePickerMode.OpenFolder);
            if (picker.ShowDialog(wb) == System.Windows.Forms.DialogResult.OK)
            {
                MessageService.ShowMessage(picker.ResourceID);
            }
            else
            {
                MessageService.ShowMessage(Strings.Cancelled);
            }
        }
    }

    internal class OpenResourceWithFilterCommand : AbstractMenuCommand
    {
        public override void Run()
        {
            var wb = Workbench.Instance;
            var exp = wb.ActiveSiteExplorer;
            var mgr = ServiceRegistry.GetService<ServerConnectionManager>();
            var conn = mgr.GetConnection(exp.ConnectionName);

            var picker = new ResourcePicker(conn, ResourceTypes.FeatureSource.ToString(), ResourcePickerMode.OpenResource);
            if (picker.ShowDialog(wb) == System.Windows.Forms.DialogResult.OK)
            {
                MessageService.ShowMessage(picker.ResourceID);
            }
            else
            {
                MessageService.ShowMessage(Strings.Cancelled);
            }
        }
    }
}