using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public class PollVoteEventArgs : EventArgs
    {
        public int SelectedIndex { get; set; }
        public string OptionText { get; set; }
    }

    public class PollQuestion : Control, IPostBackEventHandler, INamingContainer
    {
        public string QuestionText
        {
            get { return (string)(ViewState["QuestionText"] ?? string.Empty); }
            set { ViewState["QuestionText"] = value; }
        }

        public string Options
        {
            get { return (string)(ViewState["Options"] ?? string.Empty); }
            set { ViewState["Options"] = value; }
        }

        public int SelectedOption
        {
            get { return (int)(ViewState["SelectedOption"] ?? -1); }
            set { ViewState["SelectedOption"] = value; }
        }

        public event EventHandler<PollVoteEventArgs> VoteSubmitted;

        protected virtual void OnVoteSubmitted(PollVoteEventArgs e)
        {
            if (VoteSubmitted != null)
            {
                VoteSubmitted(this, e);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (!string.IsNullOrEmpty(eventArgument))
            {
                int selectedIndex;
                if (int.TryParse(eventArgument, out selectedIndex))
                {
                    SelectedOption = selectedIndex;
                    string[] optionArray = Options.Split(',');
                    string optionText = selectedIndex >= 0 && selectedIndex < optionArray.Length 
                        ? optionArray[selectedIndex].Trim() 
                        : string.Empty;

                    OnVoteSubmitted(new PollVoteEventArgs 
                    { 
                        SelectedIndex = selectedIndex, 
                        OptionText = optionText 
                    });
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-question");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-question-text");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.Write(System.Web.HttpUtility.HtmlEncode(QuestionText));
            writer.RenderEndTag();

            string[] optionArray = Options.Split(',');
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-options");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            for (int i = 0; i < optionArray.Length; i++)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-option");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "radio");
                writer.AddAttribute(HtmlTextWriterAttribute.Name, UniqueID + "_option");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, i.ToString());
                writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID + "_option_" + i);
                if (SelectedOption == i)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Checked, "checked");
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.For, ClientID + "_option_" + i);
                writer.RenderBeginTag(HtmlTextWriterTag.Label);
                writer.Write(System.Web.HttpUtility.HtmlEncode(optionArray[i].Trim()));
                writer.RenderEndTag();

                writer.RenderEndTag(); // poll-option div
            }

            writer.RenderEndTag(); // poll-options div

            string postBackScript = Page.ClientScript.GetPostBackEventReference(this, "' + this.form.querySelector('input[name=\"" + UniqueID + "_option\"]:checked')?.value + '");
            
            writer.AddAttribute(HtmlTextWriterAttribute.Type, "button");
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "poll-submit-button");
            writer.AddAttribute(HtmlTextWriterAttribute.Onclick, "javascript:" + postBackScript.Replace("' + this.form.querySelector('input[name=\"" + UniqueID + "_option\"]:checked')?.value + '", "'+this.form.querySelector('input[name=\"" + UniqueID + "_option\"]:checked')?.value+'"));
            writer.RenderBeginTag(HtmlTextWriterTag.Button);
            writer.Write("Vote");
            writer.RenderEndTag();

            writer.RenderEndTag(); // poll-question div
        }
    }
}
