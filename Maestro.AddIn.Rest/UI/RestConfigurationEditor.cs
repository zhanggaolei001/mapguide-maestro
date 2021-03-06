﻿#region Disclaimer / License

// Copyright (C) 2015, Jackie Ng
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
using Maestro.AddIn.Rest.Model;
using Maestro.Editors.Common;
using Maestro.Editors.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using OSGeo.MapGuide.MaestroAPI;
using OSGeo.MapGuide.ObjectModels;
using OSGeo.MapGuide.ObjectModels.LayerDefinition;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Windows.Forms;

namespace Maestro.AddIn.Rest.UI
{
    public partial class RestConfigurationEditor : Form
    {
        private RestConfigurationEditor()
        {
            InitializeComponent();
            txtJson.EnableFolding = true;
            txtJson.SetHighlighting("JavaScript"); //Close enough
        }

        private IRestClient _client;
        private IServerConnection _conn;

        public RestConfigurationEditor(IServerConnection conn, IRestClient client, string json, string uriPart, bool isNew)
            : this()
        {
            _conn = conn;
            _client = client;
            if (!isNew)
            {
                var dataLen = "data/".Length; //NOXLATE
                var configLen = "/config".Length; //NOXLATE
                txtUriPart.Text = uriPart.Substring(uriPart.IndexOf("data/") + dataLen, uriPart.Length - dataLen - configLen); //NOXLATE
            }
            txtUriPart.ReadOnly = !isNew;
            txtJson.Text = json;
        }

        public bool IsNew => !txtUriPart.ReadOnly;

        public string GetUriPart() => $"data/{txtUriPart.Text}/config"; //NOXLATE

        public string GetJson() => txtJson.Text;

        private void btnCancel_Click(object sender, EventArgs e) => this.DialogResult = DialogResult.Cancel;

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!txtUriPart.ReadOnly && string.IsNullOrEmpty(txtUriPart.Text))
            {
                MessageBox.Show(Strings.UriPartRequrired);
                txtUriPart.Focus();
                return;
            }

            var req = new RestRequest(this.GetUriPart(), Method.POST);
            req.AddFile("data", Encoding.UTF8.GetBytes(this.GetJson()), "restcfg.json"); //NOXLATE

            var resp = _client.Execute(req);
            if (resp.StatusCode != System.Net.HttpStatusCode.OK)
                MessageBox.Show(string.Format(Strings.ErrorSavingConfiguration, resp.Content));
            else
                MessageBox.Show(Strings.ConfigurationSaved);

            this.DialogResult = DialogResult.OK;
        }

        private void featureSourceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var picker = new ResourcePicker(_conn, ResourceTypes.FeatureSource.ToString(), ResourcePickerMode.OpenResource))
            {
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    dynamic conf = GetConfigurationObject();
                    dynamic source = new ExpandoObject();
                    source.Type = "MapGuide"; //NOXLATE
                    source.FeatureSource = picker.ResourceID;

                    string[] classNames = _conn.FeatureService.GetClassNames(picker.ResourceID, null);
                    string className = GenericItemSelectionDialog.SelectItem(Strings.FdoClass, Strings.SelectClassName, classNames);
                    if (className != null)
                    {
                        source.FeatureClass = className;
                        conf.Source = source;
                        txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
                    }
                }
            }
        }

        private void layerDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (var picker = new ResourcePicker(_conn, ResourceTypes.LayerDefinition.ToString(), ResourcePickerMode.OpenResource))
            {
                if (picker.ShowDialog() == DialogResult.OK)
                {
                    dynamic conf = GetConfigurationObject();
                    dynamic source = new ExpandoObject();
                    source.Type = "MapGuide"; //NOXLATE
                    source.LayerDefinition = picker.ResourceID;
                    conf.Source = source;

                    txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
                }
            }
        }

        private dynamic GetConfigurationObject()
        {
            var converter = new ExpandoObjectConverter();
            dynamic conf = JsonConvert.DeserializeObject<ExpandoObject>(txtJson.Text, converter);
            return conf;
        }

        private RestSourceContext GetSourceContext(dynamic conf)
        {
            var source = conf.Source as IDictionary<string, object>;
            if (source.ContainsKey("LayerDefinition")) //NOXLATE
            {
                string resId = conf.Source.LayerDefinition;
                ILayerDefinition ldf = (ILayerDefinition)_conn.ResourceService.GetResource(resId);
                IVectorLayerDefinition vl = ldf.SubLayer as IVectorLayerDefinition;
                if (vl == null)
                    throw new InvalidOperationException(string.Format(Strings.NotAVectorLayer, resId));

                return new RestSourceContext(_conn, new RestSource()
                {
                    FeatureSource = vl.ResourceId,
                    ClassName = vl.FeatureName
                });
            }
            else if (source.ContainsKey("FeatureSource")) //NOXLATE
            {
                string resId = conf.Source.FeatureSource;

                return new RestSourceContext(_conn, new RestSource()
                {
                    FeatureSource = resId,
                    ClassName = conf.Source.FeatureClass
                });
            }

            throw new InvalidOperationException(Strings.InvalidSourceConfiguration);
        }

        private void xmlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var conf = GetConfigurationObject();
            if (!((IDictionary<string, object>)conf).ContainsKey("Source")) //NOXLATE
            {
                MessageBox.Show(Strings.NoSourceInConfiguration);
                return;
            }
            var ctx = GetSourceContext(conf);
            if (new NewRepresentationDialog("xml", conf, ctx).ShowDialog() == DialogResult.OK) //NOXLATE
            {
                txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            }
        }

        private void geoJSONToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var conf = GetConfigurationObject();
            if (!((IDictionary<string, object>)conf).ContainsKey("Source")) //NOXLATE
            {
                MessageBox.Show(Strings.NoSourceInConfiguration);
                return;
            }
            var ctx = GetSourceContext(conf);
            if (new NewRepresentationDialog("geojson", conf, ctx).ShowDialog() == DialogResult.OK) //NOXLATE
            {
                txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            }
        }

        private void csvToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var conf = GetConfigurationObject();
            if (!((IDictionary<string, object>)conf).ContainsKey("Source")) //NOXLATE
            {
                MessageBox.Show(Strings.NoSourceInConfiguration);
                return;
            }
            var ctx = GetSourceContext(conf);
            if (new NewRepresentationDialog("csv", conf, ctx).ShowDialog() == DialogResult.OK) //NOXLATE
            {
                txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            }
        }

        private void imageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var conf = GetConfigurationObject();
            if (!((IDictionary<string, object>)conf).ContainsKey("Source")) //NOXLATE
            {
                MessageBox.Show(Strings.NoSourceInConfiguration);
                return;
            }
            var ctx = GetSourceContext(conf);
            if (new NewRepresentationDialog("image", conf, ctx).ShowDialog() == DialogResult.OK) //NOXLATE
            {
                txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            }
        }

        private void templateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var conf = GetConfigurationObject();
            if (!((IDictionary<string, object>)conf).ContainsKey("Source")) //NOXLATE
            {
                MessageBox.Show(Strings.NoSourceInConfiguration);
                return;
            }
            var ctx = GetSourceContext(conf);
            if (new NewRepresentationDialog("template", conf, ctx).ShowDialog() == DialogResult.OK) //NOXLATE
            {
                txtJson.Text = JsonConvert.SerializeObject(conf, Formatting.Indented);
            }
        }
    }
}
