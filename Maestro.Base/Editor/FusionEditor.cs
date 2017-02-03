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

using Maestro.Editors;
using Maestro.Editors.Fusion;
using OSGeo.MapGuide.ObjectModels;
using System.Drawing;
using System.Windows.Forms;

#pragma warning disable 1591

namespace Maestro.Base.Editor
{
    /// <summary>
    /// A specialized editor for Application Definition (Flexible Layout) resources.
    /// </summary>
    /// <remarks>
    /// Although public, this class is undocumented and reserved for internal use by built-in Maestro AddIns
    /// </remarks>
    public partial class FusionEditor : EditorContentBase
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FusionEditor()
        {
            InitializeComponent();
        }

        private IResource _res;
        private IEditorService _edsvc;
        private bool _init = false;

        /// <summary>
        /// Binds the specified resource to this control. This effectively initializes
        /// all the fields in this control and sets up databinding on all fields. All
        /// subclasses *must* override this method.
        ///
        /// Also note that this method may be called more than once (e.g. Returning from
        /// and XML edit of this resource). Thus subclasses must take this scenario into
        /// account when implementing
        /// </summary>
        /// <param name="service">The editor service</param>
        protected override void Bind(IEditorService service)
        {
            if (!_init)
            {
                _edsvc = service;
                _res = _edsvc.GetEditedResource();
                _init = true;
            }

            panelBody.Controls.Clear();

            var flexEditor = new FlexibleLayoutEditor();
            flexEditor.Dock = DockStyle.Fill;
            panelBody.Controls.Add(flexEditor);

            flexEditor.Bind(_edsvc);
        }

        public override Icon ViewIcon
        {
            get
            {
                return Properties.Resources.icon_fusion;
            }
        }
    }
}