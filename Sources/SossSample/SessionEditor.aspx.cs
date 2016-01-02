using System;
using System.Collections.Generic;
using System.Linq;

namespace SossSample
{
    public partial class SessionEditor : System.Web.UI.Page
    {
        protected void Page_PreRender(object sender, EventArgs e)
        {
            var items = (from string key in Session.Keys select new KeyValuePair<string, string>(key, Session[key].ToString())).ToList();

            SessionGridView.DataSource = items;
            SessionGridView.DataBind();
        }

        protected void SubmitButton_OnClick(object sender, EventArgs e)
        {
            var name = NameTextBox.Text;
            var value = ValueTextBox.Text;

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
                return;

            Session[name] = value;
        }
    }
}